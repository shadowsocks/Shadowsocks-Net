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

namespace Shadowsocks
{
    using Infrastructure;
    using Infrastructure.Sockets;

    /// <summary>
    /// More like a module
    /// </summary>
    public interface ISocks5Handler : IDisposable
    {
        ValueTask HandleTcp(IClient tcpClient, CancellationToken cancellationToken = default);
        ValueTask HandleUdp(IClient udpClient, CancellationToken cancellationToken = default);
    }
}
