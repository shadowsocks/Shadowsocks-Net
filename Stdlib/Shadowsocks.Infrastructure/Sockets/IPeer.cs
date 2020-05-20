/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Shadowsocks.Infrastructure.Sockets
{
    public interface IPeer
    {
        IPEndPoint EndPoint { get; }
    }
}
