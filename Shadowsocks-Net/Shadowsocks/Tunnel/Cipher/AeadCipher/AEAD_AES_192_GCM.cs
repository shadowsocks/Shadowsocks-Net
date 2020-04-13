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
    [Cipher("aes-192-gcm")]
    public class AEAD_AES_192_GCM : AeadAesGcm
    {
        public AEAD_AES_192_GCM(string password, ILogger logger = null)
            : base(password, new ValueTuple<int, int>(24, 24), logger)
        {

        }
    }
}
