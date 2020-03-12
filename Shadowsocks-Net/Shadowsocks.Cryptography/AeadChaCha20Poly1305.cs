using System;

namespace Shadowsocks.Cryptography
{
    /// <summary>
    /// Represents an instance of the <a href="https://tools.ietf.org/html/rfc7539">ChaCha20-Poly1305 AEAD algorithm</a>.
    /// </summary>
    public class AeadChaCha20Poly1305 : AeadChaChaPoly1305
    {
        #region Static Members
        /// <summary>
        /// Initializes a new instance of the <see cref="AeadChaCha20Poly1305"/> class.
        /// </summary>
        /// <param name="key">The secret that will be used during initialization.</param>
        /// <param name="nonce">The one-time use state parameter.</param>
        /// <param name="aad">The arbitrary length additional authenticated data parameter.</param>
        public static AeadChaCha20Poly1305 New(ReadOnlySpan<byte> key, ReadOnlySpan<byte> nonce, ReadOnlySpan<byte> aad) {
            var chaCha20 = ChaCha20.New(key, nonce, 1U);
            var poly1305 = Poly1305.New(GenerateOneTimeKey(chaCha20.Key, chaCha20.IV, 20U).AsSpan(0, Poly1305.KeyLength).ToArray());

            return new AeadChaCha20Poly1305(aad.ToArray(), chaCha20, poly1305);
        }
        #endregion

        #region Instance Members
        private AeadChaCha20Poly1305(byte[] aad, ChaCha chaCha, Poly1305 poly1305) : base(aad, chaCha, poly1305) { }
        #endregion
    }
}
