using System;
using System.Buffers.Binary;
using System.IO;
using System.Security.Cryptography;

namespace Shadowsocks.Cryptography
{
    using Infrastructure;

    /// <summary>
    /// Represents an instance of the ChaCha-Poly1305 family of AEAD algorithms.
    /// </summary>
    public abstract class AeadChaChaPoly1305
    {
        #region Constant Members
        private const string MESSAGE_AUTHENTICATION_ERROR = "failed to authenticate stream data, unable to decrypt";
        private const string SAME_STREAM_ERROR = "source and destination must be different streams";
        private const int TAG_LENGTH_IN_BYTES = 16;
        #endregion

        #region Static Members
        /// <summary>
        /// Generates a one-time use key for a ChaCha-Poly1305 authenticator. 
        /// </summary>
        /// <param name="key">The secret that will be used during initialization.</param>
        /// <param name="nonce">The one-time use state parameter.</param>
        /// <param name="numRounds"></param>
        [CLSCompliant(false)]
        public static byte[] GenerateOneTimeKey(ReadOnlySpan<byte> key, ReadOnlySpan<byte> nonce, uint numRounds)
        {
            var ivLow = 0U;
            var ivHigh = BinaryPrimitives.ReadUInt32LittleEndian(nonce);
            var oneTimeKey = new byte[ChaChaTransform.BlockLength];

            ChaChaTransform.WriteKeyStreamBlock(ChaCha.Initialize(key, nonce, ivLow), BitwiseHelpers.ConcatBits(ivHigh, ivLow), numRounds, oneTimeKey);

            return oneTimeKey;
        }
        #endregion

        #region Instance Members
        private readonly byte[] m_aad;
        private readonly ChaCha m_chaCha;
        private readonly Poly1305 m_poly1305;

        /// <summary>
        /// Initializes a new instance of the <see cref="AeadChaChaPoly1305"/> class.
        /// </summary>
        protected AeadChaChaPoly1305(byte[] aad, ChaCha chaCha, Poly1305 poly1305)
        {
            m_aad = aad;
            m_chaCha = chaCha;
            m_poly1305 = poly1305;
        }


        private byte[] ComputeTag(Stream source, Stream destination, byte[] aad)
        {
            if (source == destination)
            {
                throw new ArgumentException(message: SAME_STREAM_ERROR, paramName: nameof(source));
            }
            var destinationOffset = destination.Position;

            ulong len_aad = 0;
            //aad [+padding]
            {
                if (null != aad && aad.Length > 0)
                {
                    destination.Write(aad, 0, aad.Length);
                    len_aad += (ulong)aad.Length;

                    int pad = aad.Length % 16;
                    if (0 != pad)
                    {
                        pad = aad.Length < 16 ? 16 - aad.Length : 16 - pad;
                        var padding = new byte[pad];
                        destination.Write(padding, 0, padding.Length);
                    }
                }
            }

            ulong len_cipher = (ulong)source.Length;
            //cipher text [+padding]
            {
                source.CopyTo(destination);
                ulong pad = len_cipher % 16UL;
                if (0 != pad)
                {
                    pad = len_cipher < 16UL ? 16UL - len_cipher : 16UL - pad;
                    var padding = new byte[pad];
                    destination.Write(padding, 0, padding.Length);
                }
            }
            //len of aad & len of cipher (64-bit little - endian integer)
            {
                //var len = stackalloc byte[sizeof(UInt64)];
                var lenBytes = new byte[sizeof(UInt64) * 2];

                BinaryPrimitives.WriteUInt64LittleEndian(lenBytes, len_aad);
                BinaryPrimitives.WriteUInt64LittleEndian(lenBytes.AsSpan().Slice(sizeof(UInt64)), len_cipher);

                destination.Write(lenBytes, 0, lenBytes.Length);
            }

            destination.Position = destinationOffset;

            return m_poly1305.ComputeHash(destination);
        }

        /// <summary>
        /// Attempts to validate a tag and then decrypts the data stream if validation was successful.
        /// </summary>
        /// <param name="source">The input data stream (ciphertext).</param>
        /// <param name="destination">The output data stream (plaintext).</param>
        /// <param name="work">The work data stream (mac).</param>
        /// <param name="tag">The tag used to authenticate the source stream before decrypting into the destination stream.</param>
        public void Decrypt(Stream source, Stream destination, Stream work, ReadOnlySpan<byte> tag)
        {
            if (null == source)
            {
                throw new ArgumentNullException(paramName: nameof(source));
            }

            if (null == destination)
            {
                throw new ArgumentNullException(paramName: nameof(destination));
            }

            if (null == work)
            {
                throw new ArgumentNullException(paramName: nameof(work));
            }

            ComputeTag(source, work, m_aad);
            source.Position = 0L;
            work.Position = 0L;
            
            if (tag.CompareInConstantTime(m_poly1305.ComputeHash(work)))
            {
                m_chaCha.Transform(source, destination);
            }
            else
            {
                throw new CryptographicException(message: MESSAGE_AUTHENTICATION_ERROR);
            }
        }
        /// <summary>
        /// Attempts to validate a tag and then decrypts the data array if validation was successful.
        /// </summary>
        /// <param name="data">The data that will be decrypted.</param>
        /// <param name="tag">The tag used to authenticate the data before decrypting it.</param>
        public void Decrypt(byte[] data, ReadOnlySpan<byte> tag)
        {
            using (var dataStream = new MemoryStream(data, true))
            using (var workStream = new MemoryStream())
            {
                Decrypt(dataStream, dataStream, workStream, tag);
            }
        }
        /// <summary>
        /// Encrypts a data stream and then generates a message authentication tag from the resulting cipher.
        /// </summary>
        /// <param name="source">The input data stream (plaintext).</param>
        /// <param name="destination">The output data stream (ciphertext).</param>
        /// <param name="work">The work data stream (mac).</param>
        public byte[] Encrypt(Stream source, Stream destination, Stream work)
        {
            if (null == source)
            {
                throw new ArgumentNullException(paramName: nameof(source));
            }

            if (null == destination)
            {
                throw new ArgumentNullException(paramName: nameof(destination));
            }

            if (null == work)
            {
                throw new ArgumentNullException(paramName: nameof(work));
            }

            var destinationOffset = destination.Position;

            m_chaCha.Transform(source, destination);
            destination.Position = destinationOffset;

            return ComputeTag(destination, work, m_aad);
        }
        /// <summary>
        /// Encrypts a data array and then generates a message authentication tag from the resulting cipher.
        /// </summary>
        /// <param name="data">The data that will be encrypted.</param>
        public byte[] Encrypt(byte[] data)
        {
            using (var dataStream = new MemoryStream(data, true))
            using (var workStream = new MemoryStream())
            {
                return Encrypt(dataStream, dataStream, workStream);
            }
        }
        #endregion
    }
}
