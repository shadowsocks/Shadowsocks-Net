/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Buffers;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
//using Microsoft.IO;
using Argument.Check;

namespace Shadowsocks.Infrastructure
{
    public static class MemoryEx
    {
        public static int CopyFromStream(this Memory<byte> memory, Stream stream)
        {
            Throw.IfNull(() => stream);
            if (memory.IsEmpty || memory.Length < stream.Length || !stream.CanRead) { throw new ArgumentOutOfRangeException("invalid args"); }

            stream.Position = 0;

            using (var arr = RecyclableByteArray.Rent(1024))
            {
                int total = 0, read = 0;
                while (total < stream.Length)
                {
                    read = stream.Read(arr.Array, 0, arr.Array.Length);
                    if (read > 0)
                    {
                        arr.Array.AsMemory().Slice(0, read).CopyTo(memory.Slice(total, read));
                    }
                    total += read;
                }                
                return total;
            }
        }

        public static int CopyToStream(this ReadOnlyMemory<byte> memory, Stream stream)
        {
            Throw.IfNull(() => stream);
            if (memory.IsEmpty || !stream.CanWrite) { throw new ArgumentOutOfRangeException("invalid args"); }

            stream.Position = 0;
            const int BUFF_LEN = 1024;
            using (var arr = RecyclableByteArray.Rent(BUFF_LEN))
            {
                ReadOnlyMemoryReader reader = new ReadOnlyMemoryReader(memory);
                while (reader.Position < reader.Length)
                {
                    var slc = reader.Read(BUFF_LEN);
                    slc.CopyTo(arr.Array.AsMemory());
                    stream.Write(arr.Array, 0, slc.Length);
                }                
                return reader.Position;
            }
        }
    }
}
