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
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using Argument.Check;

namespace Shadowsocks.Cipher.AeadCipher
{
    using Infrastructure;
    using Cryptography;
    public abstract class AeadChaChaPoly1305 : ShadowosocksAeadCipher
    {
        //TODO chacha memory optimization.
        protected static RecyclableMemoryStreamManager MemoryStreamManager = null;
        public AeadChaChaPoly1305(string password, ValueTuple<int, int> key_salt_size, NonceLength nonceLength = NonceLength.LEN_12, ILogger logger = null)
          : base(password, key_salt_size, nonceLength, logger)
        {
            if (null == MemoryStreamManager)
            {
                int blockSize = 1024;
                int largeBufferMultiple = 2048 * 16;
                int maxBufferSize = 16 * largeBufferMultiple;

                var manager = new RecyclableMemoryStreamManager(blockSize,
                                                                largeBufferMultiple,
                                                                maxBufferSize);

                //manager.GenerateCallStacks = true;
                manager.AggressiveBufferReturn = true;
                manager.MaximumFreeLargePoolBytes = maxBufferSize * 4 * 5;
                manager.MaximumFreeSmallPoolBytes = blockSize * 1024 * 5;

                MemoryStreamManager = manager;
            }
        }
        protected abstract Cryptography.AeadChaChaPoly1305 CreateCipher(ReadOnlySpan<byte> key, ReadOnlySpan<byte> nonce, ReadOnlySpan<byte> aad);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="raw">[Plain]</param>
        /// <param name="key"></param>
        /// <param name="nonce"></param>
        /// <param name="aad"></param>
        /// <returns>[Cipher][Tag]</returns>
        protected override SmartBuffer EncryptChunk(ReadOnlyMemory<byte> raw, ReadOnlySpan<byte> key, ReadOnlySpan<byte> nonce, ReadOnlySpan<byte> aad = default)
        {
            var aead = this.CreateCipher(key, nonce, aad);
            using (MemoryStream src = MemoryStreamManager.GetStream(), work = MemoryStreamManager.GetStream())
            {
                raw.CopyToStream(src);
                src.Position = 0;

                var tag = aead.Encrypt(src, src, work);
                src.Position = 0;

                var rt = SmartBuffer.Rent((int)raw.Length + tag.Length);
                rt.SignificantLength = rt.Memory.CopyFromStream(src);

                tag.AsMemory().CopyTo(rt.FreeMemory);
                rt.SignificantLength += tag.Length;

                return rt;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cipher">[Cipher][Tag]</param>
        /// <param name="key"></param>
        /// <param name="nonce"></param>
        /// <param name="aad"></param>
        /// <returns>[Plain]</returns>
        protected override SmartBuffer DecryptChunk(ReadOnlyMemory<byte> cipher, ReadOnlySpan<byte> key, ReadOnlySpan<byte> nonce, ReadOnlySpan<byte> aad = default)
        {
            var rt = SmartBuffer.Rent(cipher.Length - LEN_TAG);

            var aead = this.CreateCipher(key, nonce, aad);
            using (MemoryStream src = MemoryStreamManager.GetStream(), work = MemoryStreamManager.GetStream())
            {
                cipher.Slice(0, cipher.Length - LEN_TAG).CopyToStream(src);
                src.Position = 0;

                try
                {
                    aead.Decrypt(src, src, work, cipher.Slice(cipher.Length - LEN_TAG).Span);
                    src.Position = 0;

                    rt.SignificantLength += rt.Memory.CopyFromStream(src);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"AeadChaChaPoly1305 DecryptChunk failed. {ex.Message}");
                    _logger?.LogWarning($"AeadChaChaPoly1305 DecryptChunk failed. {ex.Message}");
                    rt.SignificantLength = 0;
                }

                return rt;
            }

        }

    }
}
