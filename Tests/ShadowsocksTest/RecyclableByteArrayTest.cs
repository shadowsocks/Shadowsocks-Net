using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace ShadowsocksTest
{
    using Shadowsocks.Infrastructure;

    [TestClass]
    public class RecyclableByteArrayTest
    {
        [TestMethod]
        public void TestArrayPool()
        {
            byte[] arr1 = new byte[16];
            System.Security.Cryptography.RandomNumberGenerator.Fill(arr1);
            Trace.TraceInformation(BitConverter.ToString(arr1).Replace("-", ""));

            var ra1 = RecyclableByteArray.Rent(16);
            arr1.AsSpan().CopyTo(ra1.Array.AsSpan());
            Trace.TraceInformation(BitConverter.ToString(ra1.Array).Replace("-", ""));

            ra1.Dispose();

            var ra2 = RecyclableByteArray.Rent(10);

            Trace.TraceInformation(BitConverter.ToString(ra2.Array).Replace("-", ""));
            Assert.IsTrue(ra2.Array.Length == 16);
            Assert.IsTrue(ra2.Array.AsSpan().SequenceEqual(arr1.AsSpan()));

        }
    }

}
