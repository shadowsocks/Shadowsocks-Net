/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */


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
using Microsoft.Extensions.Logging;
using Argument.Check;

namespace Shadowsocks.Infrastructure
{
#pragma warning disable CA1063 // Implement IDisposable Correctly
    public sealed class RecyclableByteArray : IDisposable
#pragma warning restore CA1063 // Implement IDisposable Correctly
    {
        public byte[] Array => _arr;

        public int SignificantLength;


        byte[] _arr = null;

        private RecyclableByteArray(byte[] arr, int significantLength = 0)
        {
            _arr = Throw.IfNull(() => arr);
            SignificantLength = significantLength;
        }

        ~RecyclableByteArray()
        {
            Dispose();
        }


        public void Dispose()
        {
            if (null != _arr)
            {
                ArrayPool<byte>.Shared.Return(_arr, false);
                _arr = null;

                SignificantLength = 0;
            }
        }

        /// <summary>
        /// Retrieves a buffer that is at least the requested length.
        /// </summary>
        /// <param name="minimumLength"></param>
        /// <returns></returns>
        public static RecyclableByteArray Rent(int minimumLength)
        {
            var a = ArrayPool<byte>.Shared.Rent(minimumLength);
            return new RecyclableByteArray(a);
        }

    }
}
