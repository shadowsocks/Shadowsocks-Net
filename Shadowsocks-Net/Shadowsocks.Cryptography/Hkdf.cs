using System;
using System.Security.Cryptography;

namespace Shadowsocks.Cryptography
{
    /// <summary>
    /// Represents an instance of the <a href="https://en.wikipedia.org/wiki/HKDF">hash-based key derivation function</a>.
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc5869
    /// </remarks>
    public sealed class Hkdf : DeriveBytes
    {
        #region Constant Members
        private const string KEY_LENGTH_ERROR = ("key length must be an integer in the inclusive range [1-{0}]");
        #endregion

        #region Static Members
        /// <summary>
        /// Derives a fixed-length pseudo-random key using the specified hash algorithm.
        /// </summary>
        /// <param name="keyedHashAlgorithm">The class that will be used to perform key derivation.</param>
        /// <param name="key">The key that a new value will be derived from.</param>
        public static byte[] Extract(KeyedHashAlgorithm keyedHashAlgorithm, byte[] key) => keyedHashAlgorithm.ComputeHash(key);
        /// <summary>
        /// Initializes a new instance of the <see cref="Hkdf"/> class.
        /// </summary>
        /// <param name="keyedHashAlgorithm">The class that will be used to perform key derivation.</param>
        /// <param name="data">The additional input data.</param>
        public static Hkdf New(KeyedHashAlgorithm keyedHashAlgorithm, ReadOnlySpan<byte> data) => new Hkdf(keyedHashAlgorithm, data);
        #endregion

        #region Instance Members
        private readonly byte[] m_buffer;
        private readonly int m_hashSizeInBytes;
        private readonly KeyedHashAlgorithm m_keyedHashAlgorithm;

        private Hkdf(KeyedHashAlgorithm keyedHashAlgorithm, ReadOnlySpan<byte> data) {
            if (null == keyedHashAlgorithm) {
                throw new ArgumentNullException(paramName: nameof(keyedHashAlgorithm));
            }

            var hashSizeInBytes = (keyedHashAlgorithm.HashSize >> 3);
            var buffer = new byte[((hashSizeInBytes + data.Length) + 1)];

            data.CopyTo(buffer.AsSpan(hashSizeInBytes));

            m_buffer = buffer;
            m_hashSizeInBytes = hashSizeInBytes;
            m_keyedHashAlgorithm = keyedHashAlgorithm;
        }

        /// <summary>
        /// Derives a cryptographic key value using the Hkdf algorithm.
        /// </summary>
        /// <param name="keyLength">The length of the derived key value (in bytes).</param>
        public override byte[] GetBytes(int keyLength) {
            var hashSizeInBytes = m_hashSizeInBytes;
            var maxKeyLength = (byte.MaxValue * hashSizeInBytes);

            if ((1 > keyLength) || (maxKeyLength < keyLength)) {
                throw new ArgumentOutOfRangeException(actualValue: keyLength, message: string.Format(KEY_LENGTH_ERROR, maxKeyLength), paramName: nameof(keyLength));
            }

            var remainder = (keyLength % hashSizeInBytes);
            var remainderIsZero = (0 == remainder);
            var count = ((keyLength / hashSizeInBytes) + (remainderIsZero ? 0 : 1));
            var buffer = m_buffer;
            var hashAlgorithm = m_keyedHashAlgorithm;
            var result = new byte[keyLength];

            buffer[(buffer.Length - 1)] = 0x01;

            hashAlgorithm.ComputeHash(buffer, hashSizeInBytes, (buffer.Length - hashSizeInBytes)).CopyTo(buffer.AsSpan());

            for (var i = 2; (i <= count); i++) {
                Buffer.BlockCopy(buffer, 0, result, ((i - 2) * hashSizeInBytes), hashSizeInBytes);
                buffer[(buffer.Length - 1)] = ((byte)(0x01 * i));
                hashAlgorithm.ComputeHash(buffer).CopyTo(buffer.AsSpan());
            }

            Buffer.BlockCopy(buffer, 0, result, ((count - 1) * hashSizeInBytes), (remainderIsZero ? hashSizeInBytes : remainder));

            return result;
        }
        /// <summary>
        /// Resets the internal state.
        /// </summary>
        public override void Reset() => m_keyedHashAlgorithm.Initialize();
        /// <summary>
        /// Verifies a key by comparing it against a derived cryptographic value; returns false if the values are not equal.
        /// </summary>
        /// <param name="key">The key that will be verified.</param>
        /// <param name="keyLength">The length of the derived key value (in bytes).</param>
        public bool Validate(ReadOnlySpan<byte> key, int keyLength) => key.CompareInConstantTime(GetBytes(keyLength));
        #endregion
    }
}
