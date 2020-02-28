/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shadowsocks
{
    public interface IShadowsocksServer
    {
        void Start();
        void Stop();
    }
}
