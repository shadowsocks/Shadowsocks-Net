using System;
using System.Collections.Generic;
using System.Text;

namespace Shadowsocks.Local
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
