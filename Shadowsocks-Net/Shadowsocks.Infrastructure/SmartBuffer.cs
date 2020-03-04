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
    /// <summary>
    /// 
    /// </summary>
    public sealed class SmartBuffer : IDisposable
    {
        /// <summary>
        /// Don't forget SignificantLength.
        /// </summary>
        public Memory<byte> Memory => _memoryOwner.Memory;
       
        public int SignificantLength;
        public Memory<byte> SignificantMemory => _memoryOwner.Memory.Slice(0, SignificantLength);
        public int FreeSpace => _memoryOwner.Memory.Length - SignificantLength;
        public Memory<byte> FreeMemory => _memoryOwner.Memory.Slice(SignificantLength);


        IMemoryOwner<byte> _memoryOwner = null;

        private SmartBuffer(IMemoryOwner<byte> memoryOwner, int significantLength = 0)
        {
            _memoryOwner = Throw.IfNull(() => memoryOwner);
            SignificantLength = significantLength;
        }
        ~SmartBuffer()
        {
            Dispose();
        }


        public void Dispose()
        {
            if (null != _memoryOwner)
            {
                _memoryOwner.Dispose();
                _memoryOwner = null;

                SignificantLength = 0;
            }
        }

        public void Erase()
        {
            if (null != _memoryOwner)
            {
                Memory.Span.Fill(0);
            }

            SignificantLength = 0;
        }

        public void CopyFromStream(Stream stream)
        {
            stream.Position = 0;

            using (var arr = RecyclableByteArray.Rent(1024))
            {
                int total = 0, read = 0;
                while (total < stream.Length)
                {
                    read = stream.Read(arr.Array, 0, arr.Array.Length);
                    if (read > 0)
                    {
                        arr.Array.AsMemory().Slice(0, read).CopyTo(this.Memory.Slice(total, read));
                    }
                    total += read;
                }
            }
        }

        /// <summary>
        /// Returns a SmartBuffer capable of holding at least minBufferSize bytes.
        /// </summary>
        /// <param name="minBufferSize"></param>
        /// <returns></returns>
        public static SmartBuffer Rent(int minBufferSize)
        {
            if (minBufferSize > MemoryPool<byte>.Shared.MaxBufferSize)
            {
                throw new NotSupportedException("Too large to allocate.");
            }
            var mem = MemoryPool<byte>.Shared.Rent(minBufferSize);
            return new SmartBuffer(mem);
        }
    }
}
