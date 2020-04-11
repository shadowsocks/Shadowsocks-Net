/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Buffers;
using System.Linq;
using System.IO;
using Microsoft.Extensions.Logging;
using Argument.Check;
using System.Text;
using Shadowsocks.Infrastructure.Sockets;

namespace Shadowsocks.Http
{
    using Infrastructure;
    using Infrastructure.Sockets;
    using Infrastructure.Pipe;
    using Cipher;
    using Local;


    public class DefaultHttpHandler : IHttpHandler
    {
        ILogger _logger = null;

        List<DuplexPipe> _pipes = null;
        object _pipesReadWriteLock = new object();

        IServerLoader _serverLoader = null;

        //HTTP/1.0 200 Connection established
        static readonly byte[] RESPONSE_200_Connection_Established = Encoding.ASCII.GetBytes("HTTP/1.1 200 Connection established\r\n\r\n");
        static readonly byte[] RESPONSE_400 = Encoding.ASCII.GetBytes("http/1.1 400 bad request\r\n\r\n");

        static readonly byte[] RESPONSE_500 = Encoding.ASCII.GetBytes("http/1.1 503 service temporarily unavailable\r\n\r\n");

        public DefaultHttpHandler(IServerLoader serverLoader, ILogger logger = null)
        {
            _pipes = new List<DuplexPipe>();
            _serverLoader = Throw.IfNull(() => serverLoader);

            _logger = logger;
        }
        ~DefaultHttpHandler()
        {
            Cleanup();
        }


        public async Task HandleHttp(IClient client, CancellationToken cancellationToken)
        {
            if (null == client) { return; }
            using (SmartBuffer clientRequest = SmartBuffer.Rent(2048))
            {
                clientRequest.SignificantLength = await client.ReadAsync(clientRequest.Memory, cancellationToken);
                if (10 >= clientRequest.SignificantLength) { goto GOODBYE503; }


                if (HttpProxyHeaderResolver.TryResolve(clientRequest.SignificantMemory,
                   out HttpProxyHeaderResolver.Verb httpVerb, out Uri targetHost, out byte[] normalHttpRequestHeader))
                {
                    //_logger?.LogInformation($"HttpRoxyServer Verb={httpVerb.ToString()}.");
                    var server = _serverLoader.Load(null);
                    if (null == server)
                    {
                        _logger?.LogInformation($"HttpRoxyServer Proxy server not found.");
                        goto GOODBYE503;
                    }
                    var serverAddr = await server.GetIPEndPoint();
                    if (null == serverAddr)
                    {
                        _logger?.LogInformation($"HttpRoxyServer Unable to get proxy server address.");
                        goto GOODBYE503;
                    }

                    var relayClient = await TcpClient1.ConnectAsync(serverAddr, _logger);//A. connect ss-remote
                    if (null == relayClient)//unable to connect ss-remote
                    {
                        _logger?.LogInformation($"HttpRoxyServer Unable to connect ss-remote:[{serverAddr.ToString()}]");
                        goto GOODBYE503;
                    }

                    using (var relayRequst = SmartBuffer.Rent(HttpProxyHeaderResolver.Verb.CONNECT == httpVerb ? 300 : normalHttpRequestHeader.Length + 300))
                    {
                        if (ShadowsocksAddress.TryParse(targetHost, out Tuple<byte, ushort, byte[]> ssaddr))//B. construct sssaddr
                        {
                            //_logger?.LogInformation($"HttpRoxyServer ATYP={ssaddr.Item1}, port={ssaddr.Item2}");
                            ShadowsocksAddress.TrySerailizeTo(ssaddr.Item1, ssaddr.Item3, ssaddr.Item2, relayRequst.Memory, out int written);
                            relayRequst.SignificantLength = written;

                            if (HttpProxyHeaderResolver.Verb.CONNECT != httpVerb)
                            {
                                normalHttpRequestHeader.CopyTo(relayRequst.FreeMemory);
                                relayRequst.SignificantLength += normalHttpRequestHeader.Length;
                            }

                            var cipher = server.CreateCipher(_logger);
                            DuplexPipe pipe = new DuplexPipe(client, relayClient, Defaults.ReceiveBufferSize, _logger);
                            Cipher.TcpCipherFilter cipherFilter = new Cipher.TcpCipherFilter(relayClient, cipher, _logger);
                            pipe.AddClientFilter(cipherFilter);

                            var writeResult = await pipe.Writer[relayClient].Write(relayRequst.SignificantMemory, cancellationToken);//C. send target addr (& http header) to ss-remote.
                            _logger?.LogInformation($"Send target addr {writeResult.Written} bytes. {writeResult.Result}.");

                            if (HttpProxyHeaderResolver.Verb.CONNECT == httpVerb)
                            {
                                await client.WriteAsync(RESPONSE_200_Connection_Established, cancellationToken);
                            }
                            PipeClient(pipe, cancellationToken);//D. start piping.

                            return;
                        }
                        else { _logger?.LogInformation($"HttpRoxyServer parse host URL failed."); goto GOODBYE503; }
                    }

                }
                else { _logger?.LogInformation($"HttpRoxyServer transform http header failed."); goto GOODBYE503; }

            }

        GOODBYE503:
            {
                _logger?.LogInformation($"HttpRoxyServer closing.........");

                await client.WriteAsync(RESPONSE_500, cancellationToken);
                client.Close();
                return;
            }
        }


        void PipeClient(DuplexPipe pipe, CancellationToken cancellationToken)
        {
           
            pipe.OnBroken += this.Pipe_OnBroken;
            lock (_pipesReadWriteLock)
            {
                this._pipes.Add(pipe);
            }
            pipe.StartPipe(cancellationToken);
        }


        private void Pipe_OnBroken(object sender, PipeBrokenEventArgs e)
        {
            var p = e.Pipe as DuplexPipe;
            p.OnBroken -= this.Pipe_OnBroken;

            _logger?.LogInformation($"HttpRoxyServer Pipe_OnBroken" +
                $" A={p.ClientA.EndPoint.ToString()}, B={p.ClientB.EndPoint.ToString()}, Cause={Enum.GetName(typeof(PipeBrokenCause), e.Cause)}");

            p.StopPipe();
            p.ClientA.Close();
            p.ClientB.Close();

            lock (_pipesReadWriteLock)
            {
                this._pipes.Remove(p);
            }

        }


        public void Dispose()
        {
            Cleanup();
        }
        void Cleanup()
        {
            lock (_pipesReadWriteLock)
            {
                foreach (var p in this._pipes)
                {
                    p.StopPipe();
                    p.ClientA.Close();
                    p.ClientB.Close();
                }

                this._pipes.Clear();
            }

        }
    }
}
