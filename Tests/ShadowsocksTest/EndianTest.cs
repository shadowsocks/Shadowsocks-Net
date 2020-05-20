using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Buffers;
using System.Buffers.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShadowsocksTest
{
    using Shadowsocks.Infrastructure;

    [TestClass]
    public class EndianTest
    {
        [TestMethod]
        public void TestEndian()
        {
            Assert.IsTrue(BitConverter.IsLittleEndian);

            {
                short s1 = 123;
                ushort us1 = 123;
                Span<byte> buf1 = new byte[10];
                Span<byte> buf2 = new byte[10];

                BitConverter.GetBytes(IPAddress.HostToNetworkOrder(s1)).CopyTo(buf1);
                BinaryPrimitives.TryWriteInt16BigEndian(buf2, s1);
                Assert.IsTrue(buf1.SequenceEqual(buf2));
                buf1.Clear();
                buf2.Clear();


                BitConverter.GetBytes(s1).CopyTo(buf1);
                BinaryPrimitives.TryWriteInt16LittleEndian(buf2, s1);
                Assert.IsTrue(buf1.SequenceEqual(buf2));
                buf1.Clear();
                buf2.Clear();


                BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)us1)).CopyTo(buf1);
                BinaryPrimitives.TryWriteUInt16BigEndian(buf2, us1);
                Assert.IsTrue(buf1.SequenceEqual(buf2));
                buf1.Clear();
                buf2.Clear();

                BitConverter.GetBytes(us1).CopyTo(buf1);
                BinaryPrimitives.TryWriteUInt16LittleEndian(buf2, us1);
                Assert.IsTrue(buf1.SequenceEqual(buf2));
                buf1.Clear();
                buf2.Clear();

            }


            {
                int i = 123;
                uint ui = 123;
                Span<byte> buf1 = new byte[10];
                Span<byte> buf2 = new byte[10];

                BitConverter.GetBytes(IPAddress.HostToNetworkOrder(i)).CopyTo(buf1);
                BinaryPrimitives.TryWriteInt32BigEndian(buf2, i);
                Assert.IsTrue(buf1.SequenceEqual(buf2));
                buf1.Clear();
                buf2.Clear();

                BitConverter.GetBytes((uint)IPAddress.HostToNetworkOrder((int)ui)).CopyTo(buf1);
                BinaryPrimitives.TryWriteUInt32BigEndian(buf2, ui);
                Assert.IsTrue(buf1.SequenceEqual(buf2));
                buf1.Clear();
                buf2.Clear();
            }

        }
    }
}
