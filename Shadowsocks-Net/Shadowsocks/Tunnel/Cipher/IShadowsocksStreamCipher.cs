/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Shadowsocks.Cipher
{
    using Infrastructure;
    public interface IShadowsocksStreamCipher
    {
        SmartBuffer EncryptTcp(ReadOnlyMemory<byte> plain);
        SmartBuffer DecryptTcp(ReadOnlyMemory<byte> cipher);
        SmartBuffer EncryptUdp(ReadOnlyMemory<byte> plain);
        SmartBuffer DecryptUdp(ReadOnlyMemory<byte> cipher);

    }
}
