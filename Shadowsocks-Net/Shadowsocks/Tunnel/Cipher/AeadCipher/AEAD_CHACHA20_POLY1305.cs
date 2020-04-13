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

namespace Shadowsocks.Cipher.AeadCipher
{
    [Cipher("chacha20-ietf-poly1305")]
    public class AEAD_CHACHA20_POLY1305 : AeadChaChaPoly1305
    {
        public AEAD_CHACHA20_POLY1305(string password, ILogger logger = null)
           : base(password, new ValueTuple<int, int>(32, 32), NonceLength.LEN_12, logger)
        {
        }

        protected override Cryptography.AeadChaChaPoly1305 CreateCipher(ReadOnlySpan<byte> key, ReadOnlySpan<byte> nonce, ReadOnlySpan<byte> ad)
        {
            return Cryptography.AeadChaCha20Poly1305.New(key, nonce, ad);
        }
    }
}
