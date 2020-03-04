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
                if (0x3 == ssaddr.ATYP)//a domain name
                {
                    var ips = await _dnsCache.ResolveHost(Encoding.UTF8.GetString(ssaddr.Address.ToArray()));
                    if (ips != null && ips.Length > 0) { targetIP = ips[0]; }
                }
                else//IPv4/v6
                {
                    targetIP = new IPAddress(ssaddr.Address.Span);
                }
                if (IPAddress.Any == targetIP)//an empty IP.
                {
                    _logger?.LogWarning($"Invalid target addr. client=[{client.EndPoint.ToString()}]");
                    client.Close();
                    return;
                }
                IPEndPoint ipeTarget = new IPEndPoint(targetIP, ssaddr.Port);
                _logger?.LogInformation($"Resolved target address:[{ipeTarget.ToString()}]");
                _logger?.LogInformation($"Connecting to [{ipeTarget.ToString()}]...");
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
                PipeTcp(pipe, cancellationToken);//D. start piping.

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
        public async Task HandleUdp(IClient client, CancellationToken cancellationToken = default)
        {
            if (null == client) { return; }
            using (SmartBuffer localRequestCipher = SmartBuffer.Rent(1500))
            {
                localRequestCipher.SignificantLength = await client.ReadAsync(localRequestCipher.Memory, cancellationToken);//A. read a packet
                if (0 == localRequestCipher.SignificantLength)
                {
                    _logger?.LogWarning($"HandleUdp an empty udp packet received, client=[{client.EndPoint.ToString()}]");
                    client.Close();
                    return;
                }
                var cipher = _remoteServerConfig.CreateCipher(_logger);
                using (var localReqest = cipher.DecryptUdp(localRequestCipher.SignificantMemory)) //B. decrypt
                {
                    if (null == localReqest || 0 == localReqest.SignificantLength)//decrypt failed, available options: 1.leave it. 2.close connection. 3.add to blocklist.
                    {
                        _logger?.LogWarning($"HandleUdp decrypt failed, client=[{client.EndPoint.ToString()}]");
                        client.Close();//->local pipe broken-> local pipe close.
                        return;
                    }
                    IPAddress targetIP = IPAddress.Any; //TODO target address check
                    if (ShadowsocksAddress.TryResolve(localReqest.SignificantMemory, out ShadowsocksAddress ssaddr))//C. resolve target address
                    {
                        if (0x3 == ssaddr.ATYP)//a domain name
                        {
                            var ips = await _dnsCache.ResolveHost(Encoding.UTF8.GetString(ssaddr.Address.ToArray()));
                            if (ips != null && ips.Length > 0) { targetIP = ips[0]; }
                        }
                        else
                        {
                            targetIP = new IPAddress(ssaddr.Address.Span);
                        }
                        if (IPAddress.Any != targetIP)//D. target addr resolved.
                        {
                            IPEndPoint targetEndPoint = new IPEndPoint(targetIP, ssaddr.Port);
                            var targetClient = await UdpClient1.ConnectAsync(targetEndPoint, _logger);//E. connect to target 
                            if (null == targetClient)
                            {
                                _logger?.LogInformation($"HandleUdp unable to connect target [{targetEndPoint.ToString()}]. client=[{client.EndPoint.ToString()}]");
                                client.Close();
                                return;
                            }
                            await targetClient.WriteAsync(localReqest.Memory.Slice(ssaddr.RawMemory.Length), cancellationToken);//F. send payload .

                            PipeUdp(client, targetClient, cipher, cancellationToken);//G. piping
                        }
                        else//resolve target address failed.
                        {
                            _logger?.LogWarning($"HandleUdp invalid target addr. client=[{client.EndPoint.ToString()}]");
                            client.Close();
                            return;
                        }
                    }
                    else
                    {
                        _logger?.LogWarning($"HandleUdp resolve target addr failed. client=[{client.EndPoint.ToString()}]");
                        client.Close();
                        return;
                    }
                }
            }//end using            
        }


        void PipeTcp(DefaultPipe p, CancellationToken cancellationToken, params ClientFilter[] addFilters)
        {
            p.OnBroken += Pipe_OnBroken;
            p.ApplyFilter(addFilters);
            lock (_pipesReadWriteLock)
            {
                this._pipes.Add(p);
            }
            p.Pipe(cancellationToken);
        }

        void PipeUdp(IClient localClient, IClient targetClient, Cipher.IShadowsocksStreamCipher cipher, CancellationToken cancellationToken, params ClientFilter[] addFilters)
        {
            DefaultPipe p = new DefaultPipe(targetClient, localClient, 1500, _logger);
            p.OnBroken += Pipe_OnBroken;

            Cipher.UdpCipherFilter filterLocal1 = new Cipher.UdpCipherFilter(localClient, cipher, _logger);
            UdpRelayEncapsulationFilter filterTarget1 = new UdpRelayEncapsulationFilter(targetClient, _logger);

            p.ApplyFilter(filterLocal1).ApplyFilter(filterTarget1);
            if (addFilters.Length > 0) { p.ApplyFilter(addFilters); }

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
