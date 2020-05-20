using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Buffers;
using System.Buffers.Binary;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Shadowsocks.Cryptography
{
    /// <summary>
    /// Represents an instance of the XChaCha20 stream cipher.
    /// </summary>
    public sealed class XChaCha20 : ChaCha
    {
        #region Constant Members
        private const string NONCE_LENGTH_ERROR = ("nonce length must be exactly 24 bytes");
        #endregion

        #region Static Members
        /// <summary>
        /// Initializes a new instance of the <see cref="XChaCha20"/> class.
        /// </summary>
        /// <param name="key">The secret that will be used during initialization.</param>
        /// <param name="nonce">The one-time use state parameter.</param>
        /// <param name="iv">The initial value parameter.</param>
        [CLSCompliant(false)]
        public static XChaCha20 New(ReadOnlySpan<byte> key, ReadOnlySpan<byte> nonce, ulong iv)
        {
            if (nonce.Length != 24)
            {
                throw new ArgumentOutOfRangeException(actualValue: nonce.Length, message: NONCE_LENGTH_ERROR, paramName: nameof(nonce));
            }

            var derivedKey = ComputeHash(key, nonce.Slice(0, 16));
            var derivedNonce = (Span<byte>)stackalloc byte[12];

            BinaryPrimitives.WriteUInt32BigEndian(derivedNonce, ((uint)BitwiseHelpers.ExtractHigh(iv)));            
            nonce.Slice(16, 8).CopyTo(derivedNonce.Slice(4, 8));

            return new XChaCha20(derivedKey, derivedNonce, iv);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="XChaCha20"/> class.
        /// </summary>
        /// <param name="key">The secret that will be used during initialization.</param>
        /// <param name="nonce">The one-time use state parameter.</param>
        /// <param name="iv">The initial value parameter.</param>
        public static XChaCha20 New(ReadOnlySpan<byte> key, ReadOnlySpan<byte> nonce, long iv) => New(key, nonce, checked((ulong)iv));
        #endregion

        #region Instance Members
        private readonly ulong m_iv;

        private XChaCha20(ReadOnlySpan<byte> key, ReadOnlySpan<byte> nonce, ulong iv) : base(key, nonce)
        {
            m_iv = iv;
        }

        /// <summary>
        /// Creates a symmetric decryptor object with the current Key property and one-time use state parameter.
        /// </summary>
        /// <param name="key">The secret that will be used during decryption.</param>
        /// <param name="nonce">The one-time use state parameter.</param>
        public override ICryptoTransform CreateDecryptor(byte[] key, byte[] nonce)
        {
            var ivLow = ((uint)BitwiseHelpers.ExtractLow(m_iv));            
            return ChaChaTransform.New(Initialize(key, nonce, ivLow), m_iv, 20U);
        }
        #endregion
    }
}
