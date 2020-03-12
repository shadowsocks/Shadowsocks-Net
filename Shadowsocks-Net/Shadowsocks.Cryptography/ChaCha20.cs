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
    /// Represents an instance of the ChaCha20 stream cipher.
    /// </summary>
    public sealed class ChaCha20 : ChaCha
    {
        #region Static Members
        /// <summary>
        /// Initializes a new instance of the <see cref="ChaCha20"/> class.
        /// </summary>
        /// <param name="key">The secret that will be used during initialization.</param>
        /// <param name="nonce">The one-time use state parameter.</param>
        /// <param name="iv">The initial value parameter.</param>
        [CLSCompliant(false)]
        public static ChaCha20 New(ReadOnlySpan<byte> key, ReadOnlySpan<byte> nonce, uint iv) => new ChaCha20(key, nonce, iv);
        /// <summary>
        /// Initializes a new instance of the <see cref="ChaCha20"/> class.
        /// </summary>
        /// <param name="key">The secret that will be used during initialization.</param>
        /// <param name="nonce">The one-time use state parameter.</param>
        /// <param name="iv">The initial value parameter.</param>
        [CLSCompliant(false)]
        public static ChaCha20 New(ReadOnlySpan<byte> key, ReadOnlySpan<byte> nonce, int iv) => New(key, nonce, checked((uint)iv));
        #endregion

        #region Instance Members
        private readonly uint m_iv;

        private ChaCha20(ReadOnlySpan<byte> key, ReadOnlySpan<byte> nonce, uint iv) : base(key, nonce)
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
            var ivLow = m_iv;
            var ivHigh = BinaryPrimitives.ReadUInt32LittleEndian(nonce.AsSpan(0, ChaChaTransform.WordLength));

            return ChaChaTransform.New(Initialize(key, nonce, ivLow), BitwiseHelpers.ConcatBits(ivHigh, ivLow), 20U);            
        }
        #endregion
    }
}
