/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.IO;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using Argument.Check;

namespace Shadowsocks.Infrastructure
{
    public class ReadOnlyMemoryReader
    {
        public ReadOnlyMemory<byte> Memory => _mem;
        public int Length => _mem.Length;

        public int Position => _pos;

        ReadOnlyMemory<byte> _mem = default;

        int _pos = 0;
        public ReadOnlyMemoryReader(ReadOnlyMemory<byte> readOnlyMemory)
        {
            ; ResetMemory(readOnlyMemory);
        }

        public ReadOnlyMemory<byte> Read(int length)
        {
            if (_mem.IsEmpty) { throw new NullReferenceException("_mem"); ; }
            if (_pos >= _mem.Length) { throw new EndOfStreamException("end of stream"); }
            int remain = _mem.Length - _pos;
            var slc = _mem.Slice(_pos, Math.Min(remain, length));
            _pos += slc.Length;
            return slc;
        }

        public void ResetMemory(ReadOnlyMemory<byte> readOnlyMemory)
        {
            _mem = readOnlyMemory;
            _pos = 0;
        }
    }
}
