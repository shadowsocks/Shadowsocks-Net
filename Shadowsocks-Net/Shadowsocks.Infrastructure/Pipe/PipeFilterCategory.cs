/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Shadowsocks.Infrastructure.Pipe
{
    public enum PipeFilterCategory //: int
    {
        Obfuscation = 1,
        Cipher = 2,
        Encapsulation = 3,
        Custom = 4
    }

}
