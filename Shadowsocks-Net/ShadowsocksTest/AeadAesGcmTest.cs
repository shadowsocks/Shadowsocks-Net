using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Security.Cryptography;

namespace ShadowsocksTest
{
    using Shadowsocks.Infrastructure;
    using Shadowsocks.Cipher;
    using Shadowsocks.Cipher.AeadCipher;

    [TestClass]
    public class AeadAesGcmTest
    {
        [TestMethod]
        public void AesGcmBasics()
        {
            byte[] raw = new byte[1024];
            RandomNumberGenerator.Fill(raw);
            IShadowsocksAeadCipher aead = new AEAD_AES_128_GCM("password");
            {
                var c = aead.EncryptUdp(raw);
                var p = aead.DecryptUdp(c.SignificantMemory);                
                Assert.IsTrue(p.SignificantMemory.Span.SequenceEqual(raw.AsSpan()));
            }
            {
                RandomNumberGenerator.Fill(raw);
                var c = aead.EncryptTcp(raw);
                var p = aead.DecryptTcp(c.SignificantMemory);
                Assert.IsTrue(p.SignificantMemory.Span.SequenceEqual(raw.AsSpan()));

                RandomNumberGenerator.Fill(raw);
                c = aead.EncryptTcp(raw);
                p = aead.DecryptTcp(c.SignificantMemory);
                Assert.IsTrue(p.SignificantMemory.Span.SequenceEqual(raw.AsSpan()));

                RandomNumberGenerator.Fill(raw);
                c = aead.EncryptTcp(raw);
                p = aead.DecryptTcp(c.SignificantMemory);
                Assert.IsTrue(p.SignificantMemory.Span.SequenceEqual(raw.AsSpan()));
            }


            aead = new AEAD_AES_192_GCM("password");
            {
                RandomNumberGenerator.Fill(raw);
                var c = aead.EncryptUdp(raw);
                var p = aead.DecryptUdp(c.SignificantMemory);
                Assert.IsTrue(p.SignificantMemory.Span.SequenceEqual(raw.AsSpan()));
            }
            {
                RandomNumberGenerator.Fill(raw);
                var c = aead.EncryptTcp(raw);
                var p = aead.DecryptTcp(c.SignificantMemory);
                Assert.IsTrue(p.SignificantMemory.Span.SequenceEqual(raw.AsSpan()));

                RandomNumberGenerator.Fill(raw);
                c = aead.EncryptTcp(raw);
                p = aead.DecryptTcp(c.SignificantMemory);
                Assert.IsTrue(p.SignificantMemory.Span.SequenceEqual(raw.AsSpan()));

                RandomNumberGenerator.Fill(raw);
                c = aead.EncryptTcp(raw);
                p = aead.DecryptTcp(c.SignificantMemory);
                Assert.IsTrue(p.SignificantMemory.Span.SequenceEqual(raw.AsSpan()));
            }

            aead = new AEAD_AES_256_GCM("password");
            {
                RandomNumberGenerator.Fill(raw);
                var c = aead.EncryptUdp(raw);
                var p = aead.DecryptUdp(c.SignificantMemory);
                Assert.IsTrue(p.SignificantMemory.Span.SequenceEqual(raw.AsSpan()));
            }
            {
                RandomNumberGenerator.Fill(raw);
                var c = aead.EncryptTcp(raw);
                var p = aead.DecryptTcp(c.SignificantMemory);
                Assert.IsTrue(p.SignificantMemory.Span.SequenceEqual(raw.AsSpan()));

                RandomNumberGenerator.Fill(raw);
                c = aead.EncryptTcp(raw);
                p = aead.DecryptTcp(c.SignificantMemory);
                Assert.IsTrue(p.SignificantMemory.Span.SequenceEqual(raw.AsSpan()));

                RandomNumberGenerator.Fill(raw);
                c = aead.EncryptTcp(raw);
                p = aead.DecryptTcp(c.SignificantMemory);
                Assert.IsTrue(p.SignificantMemory.Span.SequenceEqual(raw.AsSpan()));
            }

        }


        [TestMethod]
        public void AesGcm1000Rounds()
        {
            IShadowsocksAeadCipher aes = new AEAD_AES_128_GCM("password");
            byte[] raw = new byte[1024];

            for (int i = 0; i < 1000; i++)
            {
                RandomNumberGenerator.Fill(raw);
                var c = aes.EncryptTcp(raw);
                var p = aes.DecryptTcp(c.SignificantMemory);
                Assert.IsTrue(p.SignificantMemory.Span.SequenceEqual(raw.AsSpan()));
            }

            aes = new AEAD_AES_192_GCM("password2");
            for (int i = 0; i < 1000; i++)
            {
                RandomNumberGenerator.Fill(raw);
                var c = aes.EncryptTcp(raw);
                var p = aes.DecryptTcp(c.SignificantMemory);
                Assert.IsTrue(p.SignificantMemory.Span.SequenceEqual(raw.AsSpan()));
            }

            aes = new AEAD_AES_256_GCM("password3");
            for (int i = 0; i < 1000; i++)
            {
                RandomNumberGenerator.Fill(raw);
                var c = aes.EncryptTcp(raw);
                var p = aes.DecryptTcp(c.SignificantMemory);
                Assert.IsTrue(p.SignificantMemory.Span.SequenceEqual(raw.AsSpan()));
            }
        }

        [TestMethod]
        public void AesGcmBigBlock()
        {
            IShadowsocksAeadCipher aes = new AEAD_AES_128_GCM("password");
            byte[] raw = new byte[ShadowosocksAeadCipher.LEN_TCP_MAX_CHUNK * 3 + 100];

            {
                RandomNumberGenerator.Fill(raw);
                var c = aes.EncryptTcp(raw);
                var p = aes.DecryptTcp(c.SignificantMemory);
                Assert.IsTrue(p.SignificantMemory.Span.SequenceEqual(raw.AsSpan()));

                RandomNumberGenerator.Fill(raw);
                c = aes.EncryptTcp(raw);
                p = aes.DecryptTcp(c.SignificantMemory);
                Assert.IsTrue(p.SignificantMemory.Span.SequenceEqual(raw.AsSpan()));

                RandomNumberGenerator.Fill(raw);
                c = aes.EncryptTcp(raw);
                p = aes.DecryptTcp(c.SignificantMemory);
                Assert.IsTrue(p.SignificantMemory.Span.SequenceEqual(raw.AsSpan()));
            }
        }

    }
}
