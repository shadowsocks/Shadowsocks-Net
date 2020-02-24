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

    /// <summary>
    /// This one runs on server.
    /// </summary>
    public sealed class RemoteServer : IShadowsocksServer
    {
        ILogger _logger = null;
        RemoteServerConfig _remoteServerConfig = null;

        TcpServer _tcpServer = null;
        UdpServer _udpServer = null;

        CancellationTokenSource _cancellationStop = null;

        ISocks5Handler _socks5Handler = null;


        //server 拉闸，结束accept和所有Tunnel双向数据流
        public RemoteServer(RemoteServerConfig remoteServerConfig, ILogger<RemoteServer> logger = null)
        {
            this._remoteServerConfig = Throw.IfNull(() => remoteServerConfig);
            this._logger = logger;
        }



        #region IShadowsocksServer
        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
