using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System.Buffers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace ShadowsocksTest
{
    using Shadowsocks.Infrastructure;

    [TestClass]
    public class MemorySteram2Test
    {
        [TestMethod]
        public void TestReadWrite()
        {
            int len = 16;
            byte[] buff = new byte[len];

            MemoryWriter ms = new MemoryWriter(buff);
            Assert.IsTrue(ms.Length == len);
            Assert.IsTrue(0 == ms.Position);

            byte[] a2 = { 0x12, 0x34, 0x56, 0xab };
            ms.Write(a2.AsMemory());
            Assert.IsTrue(a2.Length == ms.Position);
            Assert.IsTrue(ms.Memory.Slice(0, a2.Length).Span.SequenceEqual(a2));

            byte[] a3 = { 0xcd, 0xef };
            ms.Write(a3.AsSpan());
            byte[] a = { 0x12, 0x34, 0x56, 0xab, 0xcd, 0xef };
            Assert.IsTrue(ms.Memory.Slice(0, ms.Position).Span.SequenceEqual(a));
            Assert.IsTrue(a2.Length + a3.Length == ms.Position);
        }
    }
}
