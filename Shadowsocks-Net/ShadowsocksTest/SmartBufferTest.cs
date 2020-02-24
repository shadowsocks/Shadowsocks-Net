using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace ShadowsocksTest
{
    using Shadowsocks.Infrastructure;

    [TestClass]
    public class SmartBufferTest
    {
        [TestMethod]
        public void TestMemoryPool1()
        {
            var buf1 = SmartBuffer.Rent(16);

            System.Security.Cryptography.RandomNumberGenerator.Fill(buf1.Memory.Span);
            Trace.TraceInformation(BitConverter.ToString(buf1.Memory.ToArray()).Replace("-", ""));
            buf1.Dispose();

            var buf2 = SmartBuffer.Rent(10);
            Trace.TraceInformation(BitConverter.ToString(buf2.Memory.ToArray()).Replace("-", ""));
            buf2.Dispose();

            var buf3 = SmartBuffer.Rent(20);
            Trace.TraceInformation(BitConverter.ToString(buf3.Memory.ToArray()).Replace("-", ""));
            buf3.Dispose();
        }
    }

}
