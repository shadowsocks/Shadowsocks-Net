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

    /// <summary>
    /// This one runs on client device.
    /// </summary>
    public sealed class LocalServer : IShadowsocksServer
    {
        private readonly ILogger _logger;

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
        public void Start()
        {
            Stop();

            if (null == _cancellationStop)
            {
                _cancellationStop = new CancellationTokenSource();
                _tcpServer.Listen();
                _udpServer.Listen();

                if (_tcpServer.IsRunning)
                {
                    Task.Run(async () =>
                    {
                        await ProcessTcpClient(_cancellationStop.Token);
                    });                    
                }
                if (_udpServer.IsRunning)
                {
                    Task.Run(async () =>
                    {
                        await ProcessUdpClient(_cancellationStop.Token);
                    });
                }
            }
        }

        public void Stop()
        {
            if (null != _cancellationStop)
            {
                _cancellationStop.Cancel();
                _cancellationStop = null;
            }

            if (_tcpServer.IsRunning)
            {
                _tcpServer.StopListen();//client->closing
            }

            if (_udpServer.IsRunning)
            {
                _udpServer.StopListen();//TODO client cleanup 
            }
        }
        #endregion


        async Task ProcessTcpClient(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) { return; }

            if (_tcpServer.IsRunning)
            {
                var client = await _tcpServer.Accept();
                var t = Task.Run(async () =>//TODO
                {
                    await _socks5Handler.HandleTcp(client);
                });
                await ProcessTcpClient(cancellationToken);
            }


        }

        async Task ProcessUdpClient(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) { return; }

            if (_udpServer.IsRunning)
            {
                var client = await _udpServer.Accept();
                var t = Task.Run(async () =>//TODO
                {
                    await _socks5Handler.HandelUdp(client);
                });
                await ProcessUdpClient(cancellationToken);
            }

        }


    }
}
