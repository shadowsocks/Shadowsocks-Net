using System;

namespace Shadowsocks.Cryptography
{
    /// <summary>
    /// Represents an instance of the XChaCha20-Poly1305 AEAD algorithm.
    /// </summary>
    public class AeadXChaCha20Poly1305 : AeadChaChaPoly1305
    {
        #region Static Members
        /// <summary>
        /// Initializes a new instance of the <see cref="AeadXChaCha20Poly1305"/> class.
        /// </summary>
        /// <param name="key">The secret that will be used during initialization.</param>
        /// <param name="nonce">The one-time use state parameter.</param>
        /// <param name="aad">The arbitrary length additional authenticated data parameter.</param>
        public static AeadXChaCha20Poly1305 New(ReadOnlySpan<byte> key, ReadOnlySpan<byte> nonce, ReadOnlySpan<byte> aad) {
            var xChaCha20 = XChaCha20.New(key, nonce, 1UL);
            var poly1305 = Poly1305.New(GenerateOneTimeKey(xChaCha20.Key, xChaCha20.IV, 20U).AsSpan(0, Poly1305.KeyLength).ToArray());

            return new AeadXChaCha20Poly1305(aad.ToArray(), xChaCha20, poly1305);
        }
        #endregion

        #region Instance Members
        private AeadXChaCha20Poly1305(byte[] aad, ChaCha chaCha, Poly1305 poly1305) : base(aad, chaCha, poly1305) { }
        #endregion
    }
}
