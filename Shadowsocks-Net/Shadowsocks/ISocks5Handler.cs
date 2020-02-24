/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Shadowsocks
{
    using Infrastructure;
    using Infrastructure.Sockets;
    public interface ISocks5Handler
    {
        void HandleTcp(IClient tcpClient);
        void HandelUdp(IClient udpClient);
    }
}
