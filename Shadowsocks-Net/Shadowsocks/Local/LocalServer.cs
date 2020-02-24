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

        public LocalServer(LocalServerConfig localServerConfig, ILogger<LocalServer> logger = null)
        {
            this._localServerConfig = Throw.IfNull(() => localServerConfig);
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
