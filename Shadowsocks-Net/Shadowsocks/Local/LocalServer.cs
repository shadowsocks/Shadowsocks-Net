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

namespace Shadowsocks.Local
{
    using Infrastructure;
    using Infrastructure.Sockets;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// This one runs on client device.
    /// </summary>
    public sealed class LocalServer : IShadowsocksServer
    {
        ILogger<LocalServer> _logger = null;

        LocalServerConfig _localServerConfig = null;
        TcpServer _tcpServer = null;
        UdpServer _udpServer = null;
        CancellationTokenSource _cancellationStop = null;

        ISocks5Handler _socks5Handler = null;

        public LocalServer(LocalServerConfig localServerConfig, ILogger<LocalServer> logger = null)
        {
            this._localServerConfig = Throw.IfNull(() => localServerConfig);
            this._logger = logger;

            //TODO config map/load.
        }

        #region IShadowsocksServer

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Start()
        {
            Stop();

            _cancellationStop ??= new CancellationTokenSource();
           // _socks5Handler ??= new StandardLocalSocks5Handler();//TODO

            _tcpServer.Listen();
            _udpServer.Listen();

            if (_tcpServer.IsRunning)
            {
                Task.Run(async () =>
                {
                    await ProcessTcp(_cancellationStop.Token);
                });
            }
            if (_tcpServer.IsRunning && _udpServer.IsRunning)
            {
                Task.Run(async () =>
                {
                    await ProcessUdp(_cancellationStop.Token);
                });
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
                        var t = Task.Run(async () =>
                        {
                            await _socks5Handler.HandleTcp(client, this._cancellationStop.Token);
                        }, this._cancellationStop.Token);
                    }
                }
                else
                {
                    _logger?.LogInformation("ProcessTcp null = client");
                }

                if (cancellationToken.IsCancellationRequested) { return; }
                await ProcessTcp(cancellationToken);
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
                await ProcessUdp(cancellationToken);
            }

        }


    }
}
