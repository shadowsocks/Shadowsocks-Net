/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Buffers;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Argument.Check;


namespace Shadowsocks.Remote
{
    using Infrastructure;
    using Infrastructure.Sockets;
    using Infrastructure.Pipe;


#pragma warning disable CA1063 // Implement IDisposable Correctly
    public class StandardRemoteSocks5Handler : ISocks5Handler
#pragma warning restore CA1063 // Implement IDisposable Correctly
    {
        ILogger _logger = null;

        RemoteServerConfig _remoteServerConfig = null;
        DnsCache _dnsCache = null;

        List<DefaultPipe> _pipes = null;
        object _pipesReadWriteLock = new object();

        public StandardRemoteSocks5Handler(RemoteServerConfig remoteServerConfig, DnsCache dnsCache, ILogger logger = null)
        {
            _remoteServerConfig = Throw.IfNull(() => remoteServerConfig);
            _dnsCache = Throw.IfNull(() => dnsCache);
            _logger = logger;

            _pipes = new List<DefaultPipe>();
        }
        ~StandardRemoteSocks5Handler()
        {
            Cleanup();
        }


        public async Task HandleTcp(IClient client, CancellationToken cancellationToken = default)
        {
            if (null == client) { return; }

            var cipher = _remoteServerConfig.CreateCipher(_logger);
            Cipher.TcpCipherFilter cipherFilter = new Cipher.TcpCipherFilter(client, cipher, _logger);


            DefaultPipe pipe = new DefaultPipe(Defaults.ReceiveBufferSize, _logger);
            pipe.ClientA = client;
            pipe.ApplyFilter(cipherFilter);

            var readClientResult = await pipe.Reader[client].Read(cancellationToken);//A. Read target addr (and payload).
            if (readClientResult.Result != ClientReadWriteResult.Succeeded) { client.Close(); return; }

            if (readClientResult.Read <= 0)
            {
                _logger?.LogWarning($"This should not happen. [{client.EndPoint.ToString()}]");
                ////decrypt failed? available options: 1.leave it. 2.close connection. 3.add to blocklist.
                client.Close();
                return;
            }
            IPAddress targetIP = IPAddress.Any; //TODO target address check
            if (ShadowsocksAddress.TryResolve(readClientResult.Memory.SignificantMemory, out ShadowsocksAddress ssaddr))//B. Resolve target addr.
            {
                _logger?.LogWarning($"Reading target addr. ATYP={ssaddr.ATYP}, client=[{client.EndPoint.ToString()}]");
                IPEndPoint ipeTarget = await ssaddr.ToIPEndPoint();
                if (IPAddress.Any == ipeTarget.Address)//an empty IP.
                {
                    _logger?.LogWarning($"Invalid target addr. client=[{client.EndPoint.ToString()}]");
                    client.Close();
                    return;
                }

                _logger?.LogInformation($"Resolved target address:[{ipeTarget.ToString()}]. Connecting...");
                var targetClient = await TcpClient1.ConnectAsync(ipeTarget, _logger);//C. Connect target
                if (null == targetClient)//connect target failed.
                {
                    _logger?.LogInformation($"Unable to connect target [{ipeTarget.ToString()}]. client=[{client.EndPoint.ToString()}]");
                    client.Close();
                    return;
                }
                _logger?.LogInformation($"Connected to [{ipeTarget.ToString()}]");

                pipe.ClientB = targetClient;


                if (readClientResult.Memory.SignificantLength > ssaddr.RawMemory.Length)//have some payload
                {
                    _logger?.LogInformation($"Writing payload before piping...");
                    //await targetClient.WriteAsync(readClientResult.Memory.SignificantMemory.Slice(ssaddr.RawMemory.Length));
                    await pipe.Writer[targetClient].Write(readClientResult.Memory.SignificantMemory.Slice(ssaddr.RawMemory.Length), cancellationToken);
                }
                _logger?.LogInformation($"Start piping...");
                PipeClient(pipe, cancellationToken);//D. start piping.

            }
            else//invalid socks5 addr
            {
                _logger?.LogWarning($"Resolve target addr failed. client=[{client.EndPoint.ToString()}]");
                client.Close();
                return;
            }

            await Task.CompletedTask;
        }



        /*

            # shadowsocks UDP Request (before encrypted)
            # +------+----------+----------+----------+
            # | ATYP | DST.ADDR | DST.PORT |   DATA   |
            # +------+----------+----------+----------+
            # |  1   | Variable |    2     | Variable |
            # +------+----------+----------+----------+

            # shadowsocks UDP Response (before encrypted)
            # +------+----------+----------+----------+
            # | ATYP | DST.ADDR | DST.PORT |   DATA   |
            # +------+----------+----------+----------+
            # |  1   | Variable |    2     | Variable |
            # +------+----------+----------+----------+    
 */
        public async Task HandleUdp(IClient client, CancellationToken cancellationToken)
        {
            if (null == client) { return; }


            await HandleClient(client, Defaults.ReceiveBufferSize,
                (p) =>
                {
                    var cipher = _remoteServerConfig.CreateCipher(_logger);
                    Cipher.UdpCipherFilter cipherFilter = new Cipher.UdpCipherFilter(client, cipher, _logger);
                    p.ClientA = client;
                    p.ApplyFilter(cipherFilter);
                },
                async (request, p, targetClient) =>
                {
                    p.ClientB = targetClient;
                    UdpRelayEncapsulationFilter filterTarget1 = new UdpRelayEncapsulationFilter(targetClient, _logger);
                    p.ApplyFilter(filterTarget1);

                    _logger?.LogInformation($"Writing payload before piping...");
                    await p.Writer[targetClient].Write(request.SignificantMemory, cancellationToken);

                },
                cancellationToken);

            await Task.CompletedTask;

        }


        async Task HandleClient(IClient client, int pipeBufferSize,
            Action<DefaultPipe> pipeCreated,
            Action<SmartBuffer, DefaultPipe, IClient> targetClientConnected, CancellationToken cancellationToken)
        {

            DefaultPipe pipe = new DefaultPipe(pipeBufferSize, _logger);

            pipeCreated(pipe);


            var readClientResult = await pipe.Reader[client].Read(cancellationToken);//A. Read target addr (and payload).
            if (readClientResult.Result != ClientReadWriteResult.Succeeded) { client.Close(); return; }

            if (readClientResult.Read <= 0)
            {
                _logger?.LogWarning($"This should not happen. [{client.EndPoint.ToString()}]");
                ////decrypt failed? available options: 1.leave it. 2.close connection. 3.add to blocklist.
                client.Close();
                return;
            }
            IPAddress targetIP = IPAddress.Any; //TODO target address check
            if (ShadowsocksAddress.TryResolve(readClientResult.Memory.SignificantMemory, out ShadowsocksAddress ssaddr))//B. Resolve target addr.
            {
                _logger?.LogWarning($"Reading target addr. ATYP={ssaddr.ATYP}, client=[{client.EndPoint.ToString()}]");
                IPEndPoint ipeTarget = await ssaddr.ToIPEndPoint();
                if (IPAddress.Any == ipeTarget.Address)//an empty IP.
                {
                    _logger?.LogWarning($"Invalid target addr. client=[{client.EndPoint.ToString()}]");
                    client.Close();
                    return;
                }

                _logger?.LogInformation($"Resolved target address:[{ipeTarget.ToString()}]. Connecting...");
                var targetClient = await TcpClient1.ConnectAsync(ipeTarget, _logger);//C. Connect target
                if (null == targetClient)//connect target failed.
                {
                    _logger?.LogInformation($"Unable to connect target [{ipeTarget.ToString()}]. client=[{client.EndPoint.ToString()}]");
                    client.Close();
                    return;
                }
                _logger?.LogInformation($"Connected to [{ipeTarget.ToString()}]");

                targetClientConnected(readClientResult.Memory, pipe, targetClient);

                _logger?.LogInformation($"Start piping...");
                PipeClient(pipe, cancellationToken);//D. start piping.

            }
            else//invalid socks5 addr
            {
                _logger?.LogWarning($"Resolve target addr failed. client=[{client.EndPoint.ToString()}]");
                client.Close();
                return;
            }

            await Task.CompletedTask;
        }


        void PipeClient(DefaultPipe p, CancellationToken cancellationToken, params ClientFilter[] addFilters)
        {
            p.OnBroken += Pipe_OnBroken;
            p.ApplyFilter(addFilters);
            lock (_pipesReadWriteLock)
            {
                this._pipes.Add(p);
            }
            p.Pipe(cancellationToken);
        }



        private void Pipe_OnBroken(object sender, PipeBrokenEventArgs e)
        {
            var p = e.Pipe as DefaultPipe;
            p.OnBroken -= this.Pipe_OnBroken;
            p.UnPipe();
            p.ClientA.Close();
            p.ClientB.Close();

            lock (_pipesReadWriteLock)
            {
                this._pipes.Remove(p);
            }
        }

        void Cleanup()
        {
            lock (_pipesReadWriteLock)
            {
                foreach (var p in this._pipes)
                {
                    p.UnPipe();
                    p.ClientA.Close();
                    p.ClientB.Close();
                }

                this._pipes.Clear();
            }

        }
        public void Dispose()
        {
            Cleanup();
        }
    }
}
