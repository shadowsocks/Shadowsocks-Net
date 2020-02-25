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
    public class AEADCipherTest
    {
        [TestMethod]
        public void AesGcmTest()
        {
            byte[] raw = new byte[1024];
            RandomNumberGenerator.Fill(raw);
            IShadowsocksAeadCipher aes = new AEAD_AES_128_GCM("password");
            {
                var c = aes.EncryptUdp(raw);
                var p = aes.DecryptUdp(c.Memory.Slice(0, c.SignificantLength));
                Assert.IsTrue(p.Memory.Slice(0, p.SignificantLength).Span.SequenceEqual(raw.AsSpan()));
            }
            {
                RandomNumberGenerator.Fill(raw);
                var c = aes.EncryptTcp(raw);
                var p = aes.DecryptTcp(c.Memory.Slice(0, c.SignificantLength));
                Assert.IsTrue(p.Memory.Slice(0, p.SignificantLength).Span.SequenceEqual(raw.AsSpan()));

                RandomNumberGenerator.Fill(raw);
                 c = aes.EncryptTcp(raw);
                 p = aes.DecryptTcp(c.Memory.Slice(0, c.SignificantLength));
                Assert.IsTrue(p.Memory.Slice(0, p.SignificantLength).Span.SequenceEqual(raw.AsSpan()));

                RandomNumberGenerator.Fill(raw);
                c = aes.EncryptTcp(raw);
                p = aes.DecryptTcp(c.Memory.Slice(0, c.SignificantLength));
                Assert.IsTrue(p.Memory.Slice(0, p.SignificantLength).Span.SequenceEqual(raw.AsSpan()));
            }


            aes = new AEAD_AES_192_GCM("password");
            {
                RandomNumberGenerator.Fill(raw);
                var c = aes.EncryptUdp(raw);
                var p = aes.DecryptUdp(c.Memory.Slice(0, c.SignificantLength));
                Assert.IsTrue(p.Memory.Slice(0, p.SignificantLength).Span.SequenceEqual(raw.AsSpan()));
            }
            {
                RandomNumberGenerator.Fill(raw);
                var c = aes.EncryptTcp(raw);
                var p = aes.DecryptTcp(c.Memory.Slice(0, c.SignificantLength));
                Assert.IsTrue(p.Memory.Slice(0, p.SignificantLength).Span.SequenceEqual(raw.AsSpan()));

                RandomNumberGenerator.Fill(raw);
                c = aes.EncryptTcp(raw);
                p = aes.DecryptTcp(c.Memory.Slice(0, c.SignificantLength));
                Assert.IsTrue(p.Memory.Slice(0, p.SignificantLength).Span.SequenceEqual(raw.AsSpan()));

                RandomNumberGenerator.Fill(raw);
                c = aes.EncryptTcp(raw);
                p = aes.DecryptTcp(c.Memory.Slice(0, c.SignificantLength));
                Assert.IsTrue(p.Memory.Slice(0, p.SignificantLength).Span.SequenceEqual(raw.AsSpan()));
            }

            aes = new AEAD_AES_256_GCM("password");
            {
                RandomNumberGenerator.Fill(raw);
                var c = aes.EncryptUdp(raw);
                var p = aes.DecryptUdp(c.Memory.Slice(0, c.SignificantLength));
                Assert.IsTrue(p.Memory.Slice(0, p.SignificantLength).Span.SequenceEqual(raw.AsSpan()));
            }
            {
                RandomNumberGenerator.Fill(raw);
                var c = aes.EncryptTcp(raw);
                var p = aes.DecryptTcp(c.Memory.Slice(0, c.SignificantLength));
                Assert.IsTrue(p.Memory.Slice(0, p.SignificantLength).Span.SequenceEqual(raw.AsSpan()));

                RandomNumberGenerator.Fill(raw);
                c = aes.EncryptTcp(raw);
                p = aes.DecryptTcp(c.Memory.Slice(0, c.SignificantLength));
                Assert.IsTrue(p.Memory.Slice(0, p.SignificantLength).Span.SequenceEqual(raw.AsSpan()));

                RandomNumberGenerator.Fill(raw);
                c = aes.EncryptTcp(raw);
                p = aes.DecryptTcp(c.Memory.Slice(0, c.SignificantLength));
                Assert.IsTrue(p.Memory.Slice(0, p.SignificantLength).Span.SequenceEqual(raw.AsSpan()));
            }

        }


        [TestMethod]
        public void AesGcmTest2()
        {
            IShadowsocksAeadCipher aes = new AEAD_AES_128_GCM("password");
            byte[] raw = new byte[1024];

            for (int i = 0; i < 1000; i++)
            {
                RandomNumberGenerator.Fill(raw);
                var c = aes.EncryptTcp(raw);
                var p = aes.DecryptTcp(c.Memory.Slice(0, c.SignificantLength));
                Assert.IsTrue(p.Memory.Slice(0, p.SignificantLength).Span.SequenceEqual(raw.AsSpan()));
            }
        }
    }
}
