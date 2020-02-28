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
using Argument.Check;


namespace Shadowsocks.Cipher.AeadCipher
{
    using Infrastructure;

    public class AeadAesGcm : ShadowosocksAeadCipher
    {
        protected ConcurrentStack<AesGcm> _cipherPool = null;

        public AeadAesGcm(string password, ValueTuple<int, int> key_salt_size, ILogger logger = null)
            : base(password, key_salt_size, logger)
        {
            _cipherPool = new ConcurrentStack<AesGcm>();
        }
        ~AeadAesGcm()
        {
            Cleanup();
        }
        protected override SmartBuffer EncryptChunk(ReadOnlyMemory<byte> raw, ReadOnlySpan<byte> key, ReadOnlySpan<byte> nonce, ReadOnlySpan<byte> add = default)
        {
            AesGcm aes = null;
            if (!_cipherPool.TryPop(out aes))
            {
                aes = new AesGcm(key);
            }

            SmartBuffer cipherPacket = SmartBuffer.Rent(raw.Length + LEN_TAG);

            var cipherSpan = cipherPacket.Memory.Span;
            try
            {
                aes.Encrypt(nonce, raw.Span, cipherSpan.Slice(0, raw.Length), cipherSpan.Slice(raw.Length, LEN_TAG), add);
                cipherPacket.SignificantLength = raw.Length + LEN_TAG;
            }
            catch (Exception ex)
            {
                cipherPacket.SignificantLength = 0;
                _logger?.LogError(ex, "AeadAesGcm EncryptChunk failed.");
            }
            finally { _cipherPool.Push(aes); }


            return cipherPacket;
        }
        protected override SmartBuffer DecryptChunk(ReadOnlyMemory<byte> cipher, ReadOnlySpan<byte> key, ReadOnlySpan<byte> nonce, ReadOnlySpan<byte> add = default)
        {
            AesGcm aes = null;
            if (!_cipherPool.TryPop(out aes))
            {
                aes = new AesGcm(key);
            }
            SmartBuffer plainPacket = SmartBuffer.Rent(cipher.Length);

            var plainSpan = plainPacket.Memory.Span;
            try
            {
                aes.Decrypt(nonce, cipher.Span.Slice(0, cipher.Length - LEN_TAG), cipher.Span.Slice(cipher.Length - LEN_TAG), plainSpan.Slice(0, cipher.Length - LEN_TAG), add);
                plainPacket.SignificantLength = cipher.Length - LEN_TAG;
            }
            catch (Exception ex)
            {
                plainPacket.SignificantLength = 0;
                _logger?.LogError(ex, "AeadAesGcm DecryptChunk failed.");
            }
            finally { _cipherPool.Push(aes); }

            return plainPacket;
        }

        protected void Cleanup()
        {
            AesGcm aes = null;
            while (_cipherPool.Count > 0)
            {
                while (!_cipherPool.TryPop(out aes))
                {
                }
                aes.Dispose();
            }
        }

    }
}
