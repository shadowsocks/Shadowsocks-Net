/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Buffers;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Argument.Check;
using Shadowsocks.Infrastructure;

namespace Shadowsocks.Cipher.StreamCihper
{
    public abstract class ShadowsocksStreamCipher : ShadowsocksCipher, IShadowsocksStreamCipher
    {
        public ShadowsocksStreamCipher(string password)
            : base(password)
        {

        }

        public SmartBuffer DecryptTcp(ReadOnlyMemory<byte> cipher)
        {
            throw new NotImplementedException();
        }

        public SmartBuffer DecryptUdp(ReadOnlyMemory<byte> cipher)
        {
            throw new NotImplementedException();
        }

        public SmartBuffer EncryptTcp(ReadOnlyMemory<byte> plain)
        {
            throw new NotImplementedException();
        }

        public SmartBuffer EncryptUdp(ReadOnlyMemory<byte> plain)
        {
            throw new NotImplementedException();
        }
    }
}
