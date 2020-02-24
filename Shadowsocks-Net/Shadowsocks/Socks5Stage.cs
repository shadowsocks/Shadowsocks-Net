using System;
using System.Collections.Generic;
using System.Text;

namespace Shadowsocks.Local.Socks5
{
    public enum Socks5Stage
    {
        Handshake,
        Authentication,
        TcpConnect,
        TcpBind,
        UdpAssociate,
        TcpTransmission,
        UdpTransmission
    }
}
