/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Shadowsocks
{
    public interface IShadowsocksServer
    {
        void Start();
        void Stop();
    }
}
