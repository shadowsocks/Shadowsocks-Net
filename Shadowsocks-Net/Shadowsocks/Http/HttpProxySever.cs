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
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Argument.Check;

namespace Shadowsocks.Http
{
    using Infrastructure;
    using Infrastructure.Sockets;
    using Local;

    /// <summary>
    /// Http proxy is not part of Shadowsocks, but it is very practical for client computers.
    /// </summary>
    public class HttpProxySever : IShadowsocksServer
    {
        ILogger _logger = null;
        HttpProxySeverConfig _config = null;
        TcpServer _tcpServer = null;
        IHttpHandler _httpHandler = null;

        IServerLoader _serverLoader = null;
        CancellationTokenSource _cancellationStop = null;
        public HttpProxySever(HttpProxySeverConfig httpProxySeverConfig, IServerLoader serverLoader, ILogger logger = null)
        {
            _config = Throw.IfNull(() => httpProxySeverConfig);
            _serverLoader = Throw.IfNull(() => serverLoader);
            _logger = logger;
            ServerConfig serverConfig = new ServerConfig()
            {
                BindPoint = _config.GetBindPoint(),
                MaxNumClient = Defaults.MaxNumClient
            };
            _tcpServer = new TcpServer(serverConfig, logger);

        }

        ~HttpProxySever()
        {
            Stop();
        }

        #region IShadowsocksServer

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Start()
        {
            Stop();
            _cancellationStop ??= new CancellationTokenSource();
            _httpHandler ??= new DefaultHttpHandler(_serverLoader, _logger);

            _tcpServer.Listen();

            if (_tcpServer.IsRunning)
            {
                _ = Task.Run(async () =>
                {
                    await ProcessHttp(_cancellationStop.Token);
                }, this._cancellationStop.Token);
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
            if (null != _httpHandler)
            {
                _httpHandler.Dispose();
                _httpHandler = null;
            }
        }
        #endregion

        async Task ProcessHttp(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && _tcpServer.IsRunning)
            {
                var client = await _tcpServer.Accept();
                if (null != client)
                {
                    if (cancellationToken.IsCancellationRequested) { client.Close(); return; }
                    if (null != _httpHandler)
                    {
                        _ = Task.Run(async () =>
                        {
                            await _httpHandler.HandleHttp(client, this._cancellationStop.Token);
                        }, this._cancellationStop.Token);
                    }
                }
                else
                {
                    _logger?.LogInformation("ProcessTcp null = client");
                    break;
                }
            }//end while
        }

    }
}
