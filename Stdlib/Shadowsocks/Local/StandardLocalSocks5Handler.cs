﻿/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Buffers;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Argument.Check;


namespace Shadowsocks.Local
{
    using Infrastructure;
    using Infrastructure.Sockets;
    using Infrastructure.Pipe;
    using Cipher;
    sealed class StandardLocalSocks5Handler : ISocks5Handler
    {

        ILogger _logger = null;

        List<DuplexPipe> _pipes = null;
        object _pipesReadWriteLock = new object();

        IServerLoader _serverLoader = null;

        public StandardLocalSocks5Handler(IServerLoader serverLoader, ILogger logger = null)
        {
            _pipes = new List<DuplexPipe>();
            _serverLoader = Throw.IfNull(() => serverLoader);

            _logger = logger;
        }
        ~StandardLocalSocks5Handler()
        {
            Cleanup();
        }

        public async Task HandleTcp(IClient client, CancellationToken cancellationToken)
        {
            if (null == client) { return; }
            if (!await Handshake(client, cancellationToken)) { return; } //Handshake            
            using (SmartBuffer request = SmartBuffer.Rent(300), response = SmartBuffer.Rent(300))
            {
                request.SignificantLength = await client.ReadAsync(request.Memory, cancellationToken);
                if (5 >= request.SignificantLength) { client.Close(); return; }
                #region socks5
                /*
                +----+-----+-------+------+----------+----------+
                |VER | CMD |  RSV  | ATYP | DST.ADDR | DST.PORT |
                +----+-----+-------+------+----------+----------+
                | 1  |  1  | X'00' |  1   | Variable |    2     |
                +----+-----+-------+------+----------+----------+

                 Where:

                      o  VER    protocol version: X'05'
                      o  CMD
                         o  CONNECT X'01'
                         o  BIND X'02'
                         o  UDP ASSOCIATE X'03'
                      o  RSV    RESERVED
                      o  ATYP   address type of following address
                         o  IP V4 address: X'01'
                         o  DOMAINNAME: X'03'
                         o  IP V6 address: X'04'
                      o  DST.ADDR       desired destination address
                      o  DST.PORT desired destination port in network octet
                         order


                +----+-----+-------+------+----------+----------+
                |VER | REP |  RSV  | ATYP | BND.ADDR | BND.PORT |
                +----+-----+-------+------+----------+----------+
                | 1  |  1  | X'00' |  1   | Variable |    2     |
                +----+-----+-------+------+----------+----------+

                 Where:

                      o  VER    protocol version: X'05'
                      o  REP    Reply field:
                         o  X'00' succeeded
                         o  X'01' general SOCKS server failure
                         o  X'02' connection not allowed by ruleset
                         o  X'03' Network unreachable
                         o  X'04' Host unreachable
                         o  X'05' Connection refused
                         o  X'06' TTL expired
                         o  X'07' Command not supported
                         o  X'08' Address type not supported
                         o  X'09' to X'FF' unassigned
                      o  RSV    RESERVED
                      o  ATYP   address type of following address
                         o  IP V4 address: X'01'
                         o  DOMAINNAME: X'03'
                         o  IP V6 address: X'04'
                      o  BND.ADDR       server bound address
                      o  BND.PORT       server bound port in network octet order
                 */

                #endregion                

                switch (request.Memory.Span[1])
                {
                    case 0x1://connect //TODO validate addr                             
                        {
                            var server = _serverLoader.Load(null);
                            if (null == server)
                            {
                                _logger?.LogInformation($"proxy server not found.");
                                client.Close();
                                break;
                            }
                            var serverAddr = await server.GetIPEndPoint();
                            if (null == serverAddr)
                            {
                                _logger?.LogInformation($"unable to get proxy server address.");
                                client.Close();
                                break;
                            }

                            var relayClient = await TcpClient1.ConnectAsync(serverAddr, _logger);//A. connect ss-remote
                            if (null == relayClient)//unable to connect ss-remote
                            {
                                _logger?.LogInformation($"unable to connect ss-remote:[{serverAddr.ToString()}]");
                                await client.WriteAsync(NegotiationResponse.CommandConnectFailed, cancellationToken);
                                client.Close();
                                break;
                            }

                            var clientRequest = request;
                            if (ShadowsocksAddress.TryResolve(clientRequest.Memory.Slice(3), out ShadowsocksAddress ssaddr))//B.resove target addr.
                            {
                                var cipher = server.CreateCipher(_logger);

                                DuplexPipe pipe = new DuplexPipe(client, relayClient, Defaults.ReceiveBufferSize, _logger);
                                IClientFilter cipherFilter = new Cipher.TcpCipherFilter(cipher, _logger);
                                pipe.AddFilter(relayClient, cipherFilter);

                                var writeResult = await pipe.GetWriter(relayClient).Write(ssaddr.RawMemory, cancellationToken);//C. send target addr to ss-remote.
                                _logger?.LogInformation($"Send target addr {writeResult.Written} bytes. {writeResult.Result}.");

                                await client.WriteAsync(NegotiationResponse.CommandConnectOK, cancellationToken);//D. notify client to send data.

                                PipeClient(pipe, cancellationToken);//E. start piping.

                            }
                            else
                            {
                                _logger?.LogWarning("resolve target addr failed.");
                                client.Close();
                            }
                        }
                        break;
                    case 0x2://bind
                        {
                            request.SignificantMemory.CopyTo(response.Memory);
                            response.Memory.Span[1] = 0x7;
                            await client.WriteAsync(response.Memory.Slice(0, request.SignificantLength), cancellationToken);
                            client.Close();
                        }
                        break;
                    case 0x3://udp assoc
                        {
                            if (ShadowsocksAddress.TrySerailizeTo(
                                (byte)(AddressFamily.InterNetworkV6 == client.LocalEndPoint.AddressFamily ? 0x4 : 0x1),
                                client.LocalEndPoint.Address.GetAddressBytes(),
                                (ushort)client.LocalEndPoint.Port,
                                response.Memory,
                                out int written))
                            {
                                response.SignificantLength = written;
                                await client.WriteAsync(response.SignificantMemory, cancellationToken);
                            }

                            //TODO
                            client.Closing += this.Client_Closing;
                        }
                        break;
                    default:
                        break;
                }

            }

        }

        public async Task HandleUdp(IClient client, CancellationToken cancellationToken)
        {
            if (null == client) { return; }

            //authentication //TODO udp assoc

            var server = _serverLoader.Load(null);
            if (null == server)
            {
                _logger?.LogInformation($"proxy server not found.");
                client.Close();
                return;
            }
            var serverAddr = await server.GetIPEndPoint();
            if (null == serverAddr)
            {
                _logger?.LogInformation($"unable to get proxy server address.");
                client.Close();
                return;
            }


            var relayClient = await UdpClient1.ConnectAsync(serverAddr, _logger);
            if (null == relayClient)
            {
                _logger?.LogInformation($"unable to relay udp");
                client.Close();
                return;
            }

            DuplexPipe pipe = new DuplexPipe(client, relayClient, 1500, _logger);
            IClientFilter filter = new Cipher.UdpCipherFilter(server.CreateCipher(_logger), _logger);
            IClientFilter filter2 = new UdpEncapsulationFilter(_logger);
            pipe.AddFilter(relayClient, filter)
                 .AddFilter(relayClient, filter2);

            PipeClient(pipe, cancellationToken);

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
            p.StopPipe();

            _logger?.LogInformation($"Pipe_OnBroken" +
                $" A={p.ClientA.EndPoint.ToString()}, B={p.ClientB.EndPoint.ToString()}, Cause={Enum.GetName(typeof(PipeBrokenCause), e.Cause)}");

            p.ClientA.Close();
            p.ClientB.Close();

            lock (_pipesReadWriteLock)
            {
                this._pipes.Remove(p);
            }

        }

        private void Client_Closing(object sender, ClientEventArgs e)
        {
            e.Client.Closing -= this.Client_Closing;

            //tcp lost->remove udp
            //TODO disassoc udp
        }


        async Task<bool> Handshake(IClient client, CancellationToken cancellationToken)
        {
            using (var request = SmartBuffer.Rent(300))
            {
                request.SignificantLength = await client.ReadAsync(request.Memory, cancellationToken);
                if (2 < request.SignificantLength)
                {
                    if (request.Memory.Span[0] != 0x5)//accept socks5 only.
                    {
                        await client.WriteAsync(NegotiationResponse.HandshakeReject, cancellationToken);
                        client.Close();
                        return false;
                    }
                    else
                    {
                        return 2 == await client.WriteAsync(NegotiationResponse.HandshakeAccept, cancellationToken);
                    }
                }
                client.Close();
                return false;
            }

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
        public void Dispose()
        {
            Cleanup();
        }
        class NegotiationResponse
        {
            public static readonly byte[] HandshakeAccept = { 0x5, 0x0 };
            public static readonly byte[] HandshakeReject = { 0x5, 0xFF };
            public static readonly byte[] CommandConnectOK = { 5, 0, 0, 1, 0, 0, 0, 0, 0, 0 };
            public static readonly byte[] CommandConnectFailed = { 5, 1, 0, 1, 0, 0, 0, 0, 0, 0 };
        }
    }
}
