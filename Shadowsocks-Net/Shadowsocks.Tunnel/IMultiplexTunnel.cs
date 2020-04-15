/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

namespace Shadowsocks.Tunnel
{
    using Infrastructure;
    using Infrastructure.Sockets;
    using Infrastructure.Pipe;
    public interface IMultiplexTunnel : ITunnel
    {
        //ConnectionPool<>
    }
}
