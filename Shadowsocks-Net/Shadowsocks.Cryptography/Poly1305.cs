using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Shadowsocks.Cryptography
{
    /// <summary>
    /// Represents an instance of the <a href="https://en.wikipedia.org/wiki/Poly1305">Poly1305 message authentication code generator</a>.
    /// </summary>
    /// <remarks>
    /// http://loup-vaillant.fr/tutorials/poly1305-design
    /// https://cr.yp.to/mac.html
    /// https://github.com/floodyberry/poly1305-donna/blob/master/poly1305-donna-32.h
    /// https://github.com/bcgit/bc-csharp/blob/master/crypto/src/crypto/macs/Poly1305.cs
    /// https://tools.ietf.org/html/rfc7539#section-2.5
    /// </remarks>
    public class Poly1305 : KeyedHashAlgorithm
    {
        #region Constant Members
        private const string KEY_LENGTH_ERROR = ("key length must be exactly 32 bytes");

        /// <summary>
        /// The length of a Poly1305 block (in bytes).
        /// </summary>
        public const int BlockLength = (NumWordsPerBlock * WordLength);
        /// <summary>
        /// The length of a Poly1305 key (in bytes).
        /// </summary>
        public const int KeyLength = 32;
        /// <summary>
        /// The number of words in a single Poly1305 block.
        /// </summary>
        public const int NumWordsPerBlock = 4;
        /// <summary>
        /// The length of a Poly1305 word (in bytes).
        /// </summary>
        public const int WordLength = sizeof(uint);
        #endregion

        #region Static Members
        /// <summary>
        /// Initializes a new instance of the <see cref="Poly1305"/> class.
        /// </summary>
        /// <param name="key">The secret that will be used during initialization.</param>
        public static Poly1305 New(ReadOnlySpan<byte> key) => new Poly1305(key);
        #endregion

        #region Instance Members
        private readonly uint m_k0;
        private readonly uint m_k1;
        private readonly uint m_k2;
        private readonly uint m_k3;
        private readonly uint m_r0;
        private readonly uint m_r1;
        private readonly uint m_r2;
        private readonly uint m_r3;
        private readonly uint m_r4;
        private readonly uint m_s0;
        private readonly uint m_s1;
        private readonly uint m_s2;
        private readonly uint m_s3;

        private uint m_h0;
        private uint m_h1;
        private uint m_h2;
        private uint m_h3;
        private uint m_h4;

        private Poly1305(ReadOnlySpan<byte> key) {
            if (key.Length != KeyLength) {
                throw new ArgumentOutOfRangeException(actualValue: key.Length, message: KEY_LENGTH_ERROR, paramName: nameof(key));
            }

            var keyAsUInt = MemoryMarshal.Cast<byte, uint>(key);
            var t0 = (keyAsUInt[0] & 0x0FFFFFFFU);
            var t1 = (keyAsUInt[1] & 0x0FFFFFFCU);
            var t2 = (keyAsUInt[2] & 0x0FFFFFFCU);
            var t3 = (keyAsUInt[3] & 0x0FFFFFFCU);

            var r0 = (t0 & 0x03FFFFFFU);
            t0 >>= 26;
            t0 |= (t1 << 6);
            var r1 = (t0 & 0x03FFFF03U);
            t1 >>= 20;
            t1 |= (t2 << 12);
            var r2 = (t1 & 0x03FFC0FFU);
            t2 >>= 14;
            t2 |= (t3 << 18);
            var r3 = (t2 & 0x03F03FFFU);
            t3 >>= 8;
            var r4 = (t3 & 0x00FFFFFFU);

            m_k0 = keyAsUInt[4];
            m_k1 = keyAsUInt[5];
            m_k2 = keyAsUInt[6];
            m_k3 = keyAsUInt[7];
            m_r0 = r0;
            m_r1 = r1;
            m_r2 = r2;
            m_r3 = r3;
            m_r4 = r4;
            m_s0 = (r1 * 5U);
            m_s1 = (r2 * 5U);
            m_s2 = (r3 * 5U);
            m_s3 = (r4 * 5U);

            HashSizeValue = (BlockLength << 3);
            KeyValue = key.ToArray();

            Initialize();
        }

        private void Compress(ReadOnlySpan<byte> data, uint finalizer) {
            var dataAsUInt = MemoryMarshal.Cast<byte, uint>(data);
            var h0 = m_h0;
            var h1 = m_h1;
            var h2 = m_h2;
            var h3 = m_h3;
            var h4 = m_h4;
            var r0 = m_r0;
            var r1 = m_r1;
            var r2 = m_r2;
            var r3 = m_r3;
            var r4 = m_r4;
            var s0 = m_s0;
            var s1 = m_s1;
            var s2 = m_s2;
            var s3 = m_s3;
            var t0 = dataAsUInt[0];
            var t1 = dataAsUInt[1];
            var t2 = dataAsUInt[2];
            var t3 = dataAsUInt[3];

            h0 += (t0 & 0x3FFFFFFU);
            h1 += ((uint)((BitwiseHelpers.ConcatBits(t1, t0) >> 26) & 0x3FFFFFFU));
            h2 += ((uint)((BitwiseHelpers.ConcatBits(t2, t1) >> 20) & 0x3FFFFFFU));
            h3 += ((uint)((BitwiseHelpers.ConcatBits(t3, t2) >> 14) & 0x3FFFFFFU));
            h4 += ((uint)BitwiseHelpers.Add((t3 >> 8), finalizer));

            var d0 = (BitwiseHelpers.Multiply(h0, r0) + BitwiseHelpers.Multiply(h1, s3) + BitwiseHelpers.Multiply(h2, s2) + BitwiseHelpers.Multiply(h3, s1) + BitwiseHelpers.Multiply(h4, s0));
            var d1 = (BitwiseHelpers.Multiply(h0, r1) + BitwiseHelpers.Multiply(h1, r0) + BitwiseHelpers.Multiply(h2, s3) + BitwiseHelpers.Multiply(h3, s2) + BitwiseHelpers.Multiply(h4, s1));
            var d2 = (BitwiseHelpers.Multiply(h0, r2) + BitwiseHelpers.Multiply(h1, r1) + BitwiseHelpers.Multiply(h2, r0) + BitwiseHelpers.Multiply(h3, s3) + BitwiseHelpers.Multiply(h4, s2));
            var d3 = (BitwiseHelpers.Multiply(h0, r3) + BitwiseHelpers.Multiply(h1, r2) + BitwiseHelpers.Multiply(h2, r1) + BitwiseHelpers.Multiply(h3, r0) + BitwiseHelpers.Multiply(h4, s3));
            var d4 = (BitwiseHelpers.Multiply(h0, r4) + BitwiseHelpers.Multiply(h1, r3) + BitwiseHelpers.Multiply(h2, r2) + BitwiseHelpers.Multiply(h3, r1) + BitwiseHelpers.Multiply(h4, r0));
            var c = (d0 >> 26);

            h0 = ((uint)(d0 & 0x3FFFFFFU));
            d1 += c;
            c = (d1 >> 26);
            h1 = ((uint)(d1 & 0x3FFFFFFU));
            d2 += c;
            c = (d2 >> 26);
            h2 = ((uint)(d2 & 0x3FFFFFFU));
            d3 += c;
            c = (d3 >> 26);
            h3 = ((uint)(d3 & 0x3FFFFFFU));
            d4 += c;
            c = (d4 >> 26);
            h4 = ((uint)(d4 & 0x3FFFFFFU));
            h0 += ((uint)(c * 5UL));
            c = (h0 >> 26);
            h0 = (h0 & 0x3FFFFFFU);
            h1 += ((uint)c);

            m_h0 = h0;
            m_h1 = h1;
            m_h2 = h2;
            m_h3 = h3;
            m_h4 = h4;
        }

        /// <summary>
        /// Routes data written to the object into the Poly1305 algorithm for computing the hash value.
        /// </summary>
        /// <param name="data">The input data.</param>
        /// <param name="dataOffset">The offset into the byte array from which to begin using data.</param>
        /// <param name="count">The number of bytes in the array to use as data.</param>
        protected override void HashCore(byte[] data, int dataOffset, int count) {
            // compress data blocks
            while (BlockLength < count) {
                Compress(data.AsSpan(dataOffset, BlockLength), (1U << 24));

                count -= BlockLength;
                dataOffset += BlockLength;
            }

            // read last data chunk
            if (0 < count) {
                var buffer = (Span<byte>)stackalloc byte[BlockLength];

                data.AsSpan(dataOffset, count).CopyTo(buffer);

                if (BlockLength > count) {
                    buffer[count++] = 1;

                    Compress(buffer, 0U);
                }
                else {
                    Compress(buffer, (1U << 24));
                }
            }
        }
        /// <summary>
        /// Finalizes the hash computation after the last data is processed by the cryptographic stream object.
        /// </summary>
        protected override byte[] HashFinal() {
            var h0 = m_h0;
            var h1 = m_h1;
            var h2 = m_h2;
            var h3 = m_h3;
            var h4 = m_h4;
            var k0 = m_k0;
            var k1 = m_k1;
            var k2 = m_k2;
            var k3 = m_k3;

            h1 += (h0 >> 26);
            h0 &= 0x3FFFFFFU;
            h2 += (h1 >> 26);
            h1 &= 0x3FFFFFFU;
            h3 += (h2 >> 26);
            h2 &= 0x3FFFFFFU;
            h4 += (h3 >> 26);
            h3 &= 0x3FFFFFFU;
            h0 += ((h4 >> 26) * 5U);
            h4 &= 0x3FFFFFFU;
            h1 += (h0 >> 26);
            h0 &= 0x3FFFFFFU;

            uint g0;
            uint g1;
            uint g2;
            uint g3;
            uint g4;
            uint x;
            uint y;

            g0 = (h0 + 5U);
            x = (g0 >> 26);
            g0 &= 0x3FFFFFFU;
            g1 = (h1 + x);
            x = (g1 >> 26);
            g1 &= 0x3FFFFFFU;
            g2 = (h2 + x);
            x = (g2 >> 26);
            g2 &= 0x3FFFFFFU;
            g3 = (h3 + x);
            x = (g3 >> 26);
            g3 &= 0x3FFFFFFU;
            g4 = ((h4 + x) - (1 << 26));

            x = ((g4 >> ((sizeof(uint) * 8) - 1)) - 1U);
            y = ~x;

            h0 = ((h0 & y) | (g0 & x));
            h1 = ((h1 & y) | (g1 & x));
            h2 = ((h2 & y) | (g2 & x));
            h3 = ((h3 & y) | (g3 & x));
            h4 = ((h4 & y) | (g4 & x));

            var f0 = BitwiseHelpers.Add(((h0) | (h1 << 26)), k0);
            var f1 = BitwiseHelpers.Add(((h1 >> 6) | (h2 << 20)), k1);
            var f2 = BitwiseHelpers.Add(((h2 >> 12) | (h3 << 14)), k2);
            var f3 = BitwiseHelpers.Add(((h3 >> 18) | (h4 << 8)), k3);

            m_h0 = ((uint)f0);
            f1 += (f0 >> 32);
            m_h1 = ((uint)f1);
            f2 += (f1 >> 32);
            m_h2 = ((uint)f2);
            f3 += (f2 >> 32);
            m_h3 = ((uint)f3);
            m_h4 = 0U;

            var result = new byte[BlockLength];
            var resultAsUInt = MemoryMarshal.Cast<byte, uint>(result);

            resultAsUInt[0] = m_h0;
            resultAsUInt[1] = m_h1;
            resultAsUInt[2] = m_h2;
            resultAsUInt[3] = m_h3;

            return result;
        }
        /// <summary>
        /// Initializes internal state using the Poly1305 algorithm.
        /// </summary>
        public override void Initialize() {
            m_h0 = 0U;
            m_h1 = 0U;
            m_h2 = 0U;
            m_h3 = 0U;
            m_h4 = 0U;
        }
        /// <summary>
        /// Verifies a message authentication code by comparing it against a derived cryptographic value; returns false if the values are not equal.
        /// </summary>
        /// <param name="mac">The message authentication code that will be verified.</param>
        /// <param name="data">The data what will be hashed; usually a 64-bit counter</param>
        public bool Validate(ReadOnlySpan<byte> mac, byte[] data) => mac.CompareInConstantTime(ComputeHash(data));
        /// <summary>
        /// Verifies a message authentication code by comparing it against a derived cryptographic value; returns false if the values are not equal.
        /// </summary>
        /// <param name="mac">The message authentication code that will be verified.</param>
        /// <param name="data">The data what will be hashed; usually a 64-bit counter</param>
        /// <param name="offset">The offset into the data array from which to begin using data.</param>
        /// <param name="count">The number of bytes in the data array that will be used.</param>
        public bool Validate(ReadOnlySpan<byte> mac, byte[] data, int offset, int count) => mac.CompareInConstantTime(ComputeHash(data, offset, count));
        /// <summary>
        /// Verifies a message authentication code by comparing it against a derived cryptographic value; returns false if the values are not equal.
        /// </summary>
        /// <param name="mac">The message authentication code that will be verified.</param>
        /// <param name="data">The data what will be hashed; usually a 64-bit counter</param>
        public bool Validate(ReadOnlySpan<byte> mac, Stream data) => mac.CompareInConstantTime(ComputeHash(data));
        #endregion
    }
}
