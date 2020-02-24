using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShadowsocksTest
{
    using Shadowsocks.Infrastructure;

    [TestClass]
    public class FixedSizeBufferTest
    {

        FixedSizeBuffer.BufferPool pool5Bytes = new FixedSizeBuffer.BufferPool(5, 2, 2);
        FixedSizeBuffer.BufferPool pool6Bytes = new FixedSizeBuffer.BufferPool(6, 3, 2);

        [TestMethod]
        public void TestBufferPool()
        {
            Assert.IsTrue(2 == pool5Bytes.CurrentPoolSize());
            Assert.IsTrue(3 == pool6Bytes.CurrentPoolSize());

            var buf1 = pool5Bytes.Rent();
            Assert.AreEqual(5, buf1.Memory.Length);
            Assert.AreEqual(0, buf1.SignificantLength);
            Assert.AreEqual(buf1.Pool, pool5Bytes);
            Assert.AreEqual(1, pool5Bytes.CurrentPoolSize());

            //------------
            buf1.Memory[0] = 1;
            buf1.Memory[1] = 2;

            Assert.AreEqual(2, buf1.Memory[1]);
            buf1.SignificantLength = 2;
            buf1.EraseData();
            Assert.AreEqual(0, buf1.Memory[1]);
            Assert.AreEqual(0, buf1.SignificantLength);

            buf1.Pool.Return(buf1);
            Assert.AreEqual(2, pool5Bytes.CurrentPoolSize());
            Assert.IsNull(buf1.Memory);
        }

        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void TestReturn()
        {
            var b1 = pool5Bytes.Rent();
            pool6Bytes.Return(b1);
        }
    }
}
