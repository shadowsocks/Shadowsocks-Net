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
    public class AeadChaChaPoly1305Test
    {


        [TestMethod]
        public void chacha20_ietf_poly1305_basics()
        {
            byte[] raw = new byte[1024];
            RandomNumberGenerator.Fill(raw);
            IShadowsocksAeadCipher aead = new AEAD_CHACHA20_POLY1305("password");
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
        }

        [TestMethod]
        public void chacha20_ietf_poly1305_1000Rounds()
        {
            IShadowsocksAeadCipher aead = new AEAD_CHACHA20_POLY1305("password");
            byte[] raw = new byte[1024];

            for (int i = 0; i < 1000; i++)
            {
                RandomNumberGenerator.Fill(raw);
                var c = aead.EncryptTcp(raw);
                var p = aead.DecryptTcp(c.SignificantMemory);
                Assert.IsTrue(p.SignificantMemory.Span.SequenceEqual(raw.AsSpan()));
            }

        }

        [TestMethod]
        public void xchacha20_ietf_poly1305_basics()
        {
            byte[] raw = new byte[1024];
            RandomNumberGenerator.Fill(raw);
            IShadowsocksAeadCipher aead = new AEAD_XCHACHA20_POLY1305("password");
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
        }


        [TestMethod]
        public void xchacha20_ietf_poly1305_1000Rounds()
        {
            IShadowsocksAeadCipher aead = new AEAD_XCHACHA20_POLY1305("password");
            byte[] raw = new byte[1024];

            for (int i = 0; i < 1000; i++)
            {
                RandomNumberGenerator.Fill(raw);
                var c = aead.EncryptTcp(raw);
                var p = aead.DecryptTcp(c.SignificantMemory);
                Assert.IsTrue(p.SignificantMemory.Span.SequenceEqual(raw.AsSpan()));
            }

        }

    }
}
