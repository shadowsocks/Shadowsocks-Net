/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Argument.Check;

namespace Shadowsocks.Remote
{
    using Infrastructure;
    using Infrastructure.Sockets;
    using Infrastructure.Pipe;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// This one runs on server.
    /// </summary>
    public sealed class RemoteServer : IShadowsocksServer
    {
        ILogger _logger = null;
        CancellationTokenSource _cancellationStop = null;

        RemoteServerConfig _remoteServerConfig = null;

        TcpServer _tcpServer = null;
        UdpServer _udpServer = null;

        ISocks5Handler _socks5Handler = null;
        DnsCache _dnsCache = null;


        public RemoteServer(RemoteServerConfig remoteServerConfig, ILogger logger = null)
        {
            _remoteServerConfig = Throw.IfNull(() => remoteServerConfig);
            _logger = logger;

            ServerConfig serverConfig = new ServerConfig()
            {
                BindPoint = _remoteServerConfig.GetIPEndPoint(),
                MaxNumClient = Defaults.MaxNumClient
            };
            _tcpServer = new TcpServer(serverConfig, _logger);
            _udpServer = new UdpServer(serverConfig, _logger);

            _dnsCache = new DnsCache(_logger);
        }



        #region IShadowsocksServer

        public async Task Start()
        {
            Stop();

            _cancellationStop ??= new CancellationTokenSource();
            _socks5Handler ??= new StandardRemoteSocks5Handler(this._remoteServerConfig, _dnsCache, _logger);

            _tcpServer.Listen();
            _udpServer.Listen();

            if (_tcpServer.IsRunning)
            {
                await ProcessTcp(_cancellationStop.Token);

            }
            if (_tcpServer.IsRunning && _udpServer.IsRunning)
            {
                await ProcessUdp(_cancellationStop.Token);
            }
        }

        public void Stop()
        {
            if (null != _cancellationStop)
            {
                _cancellationStop.Cancel();
                _cancellationStop = null;
            }

            _tcpServer.StopListen();
            _udpServer.StopListen();

            if (null != _socks5Handler)
            {
                _socks5Handler.Dispose();
                _socks5Handler = null;
            }
        }
        #endregion

        async Task ProcessTcp(CancellationToken cancellationToken)
        {
            if (_tcpServer.IsRunning)
            {
                var client = await _tcpServer.Accept();
                if (null != client)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        client.Close();
                        return;
                    }
                    if (null != _socks5Handler)
                    {
                        _ = Task.Run(() =>
                          {
                              _socks5Handler.HandleTcp(client, this._cancellationStop.Token);
                          }, this._cancellationStop.Token);
                    }
                }
                else
                {
                    _logger?.LogInformation("ProcessTcp null = client");
                }

                if (cancellationToken.IsCancellationRequested) { return; }
                await Task.Run(async () =>
                {
                    await ProcessTcp(cancellationToken);
                });
            }
        }

        async Task ProcessUdp(CancellationToken cancellationToken)
        {
            if (_udpServer.IsRunning)
            {
                var client = await _udpServer.Accept();
                if (null != client)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        client.Close();
                        return;
                    }
                    var t = Task.Run(async () =>
                    {
                        await _socks5Handler.HandleUdp(client, this._cancellationStop.Token);
                    }, this._cancellationStop.Token);

                }
                if (cancellationToken.IsCancellationRequested) { return; }
                await Task.Run(async () =>
                {
                    await ProcessUdp(cancellationToken);
                });

            }

        }
    }
}
