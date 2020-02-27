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

namespace Shadowsocks.Infrastructure.Http
{
    /// <summary>
    /// Http proxy is not part of Shadowsocks, but it is very practical for client computers.
    /// </summary>
    public class HttpProxySever
    {
        ILogger _logger = null;
        HttpProxySeverConfig _config = null;

        public HttpProxySever(HttpProxySeverConfig httpProxySeverConfig, ILogger logger = null)
        {
            _config = Throw.IfNull(() => httpProxySeverConfig);
            _logger = logger;
        }
    }
}
