﻿/*
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
    public sealed class MemoryWriter //: Stream//, IDisposable
    {
        public ReadOnlyMemory<byte> Memory { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return _mem; } }
        public int Length { [MethodImpl(MethodImplOptions.AggressiveInlining)]get { return _mem.Length; } }
        public int FreeSapce { [MethodImpl(MethodImplOptions.AggressiveInlining)]get { return _mem.Length - _pos; } }


        public int Position
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _pos; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { if (value >= 0 && value < _mem.Length) { _pos = (int)value; } }
        }


        int _pos = 0;

        Memory<byte> _mem = default;
        public MemoryWriter(Memory<byte> memory)
        {
            ResetMemory(memory);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(Memory<byte> buffer)
        {
            if (_mem.IsEmpty) { throw new NullReferenceException("_mem"); ; }
            if (_pos >= _mem.Length) { throw new EndOfStreamException("end of stream"); }

            int w = Math.Min(buffer.Length, _mem.Length - _pos);

            buffer.CopyTo(_mem.Slice(_pos, w));
            _pos += w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ReadOnlyMemory<byte> buffer)
        {
            if (_mem.IsEmpty) { throw new NullReferenceException("_mem"); ; }
            if (_pos >= _mem.Length) { throw new EndOfStreamException("end of stream"); }

            int w = Math.Min(buffer.Length, _mem.Length - _pos);

            buffer.CopyTo(_mem.Slice(_pos, w));
            _pos += w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(Span<byte> buffer)
        {
            if (_mem.IsEmpty) { throw new NullReferenceException("_mem"); ; }
            if (_pos >= _mem.Length) { throw new EndOfStreamException("end of stream"); }

            int w = Math.Min(buffer.Length, _mem.Length - _pos);
            buffer.CopyTo(_mem.Slice(_pos, w).Span);
            _pos += w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ReadOnlySpan<byte> buffer)
        {
            if (_mem.IsEmpty) { throw new NullReferenceException("_mem"); ; }
            if (_pos >= _mem.Length) { throw new EndOfStreamException("end of stream"); }

            int w = Math.Min(buffer.Length, _mem.Length - _pos);
            buffer.CopyTo(_mem.Slice(_pos, w).Span);
            _pos += w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteByte(byte @byte)
        {
            if (_mem.IsEmpty) { throw new NullReferenceException("_mem"); ; }
            if (_pos >= _mem.Length) { throw new EndOfStreamException("end of stream"); }

            _mem.Span[_pos] = @byte;
            ++_pos;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteByte(params byte[] bytes)
        {
            if (_mem.IsEmpty) { throw new NullReferenceException("_mem"); ; }
            if (_pos >= bytes.Length) { throw new EndOfStreamException("end of stream"); }

            int w = Math.Min(bytes.Length, _mem.Length - _pos);
            bytes.AsSpan().CopyTo(_mem.Slice(_pos, w).Span);
            _pos += w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResetMemory(Memory<byte> memory)
        {
            this._mem = memory;
            this._pos = 0;
        }

    }
}
