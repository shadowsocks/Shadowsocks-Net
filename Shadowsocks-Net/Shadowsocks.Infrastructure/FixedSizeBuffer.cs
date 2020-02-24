/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */


using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Runtime.InteropServices;
using System.Text;
using Argument.Check;

namespace Shadowsocks.Infrastructure
{
    /// <summary>
    /// Fixed size buffer.
    /// </summary>
    public sealed unsafe class FixedSizeBuffer
    {
        /// <summary>
        /// Default buffer size in bytes.
        /// </summary>
        public const int DEFAULT_BUFFER_SIZE = 4096;
        public const int DEFAULT_POOL_SIZE = 10;
        public const int DEFAULT_POOL_INCREASE_SIZE = 2;


        byte[] _data = null;
        byte* _ptrData = (byte*)0;
        GCHandle _gch = default;
        volatile int _inThePool = 0;

        /// <summary>
        /// Don't forget SignificantLength.
        /// </summary>
        public byte[] Memory { get; private set; }
        public int SignificantLength;
        public readonly int Offset = 0;

        /// <summary>
        /// The pool it belongs to.
        /// </summary>
        public BufferPool Pool { get; private set; }

        private FixedSizeBuffer() { }

        ~FixedSizeBuffer()
        {
            _gch.Free();//return to GC.
        }

        public void EraseData()
        {
            if (SignificantLength > 0)
            {
                //Console.WriteLine($"_ptrData = {(int)this._ptrData}");
                //Span<byte> span = new Span<byte>(this.Bytes, this.Offset, this.SignificantLength);
                //span.Fill(0);
                Unsafe.InitBlock(this._ptrData, 0, (uint)this.SignificantLength);//(uint)this.Bytes.Length
                SignificantLength = 0;
            }

        }

        private static FixedSizeBuffer New(int size, BufferPool pool = null)
        {
            FixedSizeBuffer b = new FixedSizeBuffer();
            b.Pool = pool;
            b._data = new byte[size];
            b._gch = GCHandle.Alloc(b._data, GCHandleType.Pinned);
            fixed (byte* p = b._data)
            {
                b._ptrData = p;
            }

            b.Memory = b._data;
            b.SignificantLength = 0;

            //this.PtrData=_gch.AddrOfPinnedObject();

            return b;
        }

        /// <summary>
        /// BufferPool. thread-safe, lock-free.
        /// </summary>
        public sealed class BufferPool
        {

            int _bufferSize = DEFAULT_BUFFER_SIZE;
            int _poolSize = DEFAULT_POOL_SIZE;
            int _poolIncreaseSize = DEFAULT_POOL_INCREASE_SIZE;

            //HashSet
            ConcurrentStack<Infrastructure.FixedSizeBuffer> _pool = new ConcurrentStack<Infrastructure.FixedSizeBuffer>();
            public BufferPool(int bufferSize = DEFAULT_POOL_INCREASE_SIZE, int poolSize = DEFAULT_POOL_SIZE, int poolIncreaseSize = DEFAULT_POOL_INCREASE_SIZE)
            {
                _bufferSize = Throw.IfLessOrEqual(() => bufferSize, 0);
                _poolSize = Throw.IfLessOrEqual(() => poolSize, 0);
                _poolIncreaseSize = Throw.IfLessOrEqual(() => poolIncreaseSize, 0);

                Increase(_poolSize);
            }
            ~BufferPool()
            {
                Clear();
            }

            public Infrastructure.FixedSizeBuffer Rent()
            {
                FixedSizeBuffer buffer = null;
                while (!_pool.TryPop(out buffer))
                {
                    if (_pool.IsEmpty)
                    {
                        Increase(_poolIncreaseSize);
                    }
                }

                buffer.Memory = buffer._data;
                buffer._inThePool = 0;
                return buffer;
            }

            public void Return(Infrastructure.FixedSizeBuffer buffer)//in
            {
                Throw.IfNull(() => buffer);

                //To avoid returning twice.
                if (0 == Interlocked.CompareExchange(ref buffer._inThePool, 1, 0))
                {
                    //Throw.IfNotEqualsTo(() => buffer.Bytes.Length, this._bufferSize);
                    if (buffer.Memory.Length != this._bufferSize)
                    {
                        Interlocked.Exchange(ref buffer._inThePool, 0);
                        throw new InvalidOperationException("invalid buffer.");
                    }

                    buffer.EraseData();
                    buffer.Memory = null;

                    _pool.Push(buffer);
                }

            }

            public int CurrentPoolSize()
            {
                return _pool.Count;
            }

            public int BufferSize { get => _bufferSize; }

            void Clear()
            {
                _pool.Clear();
            }

            void Increase(int count = DEFAULT_POOL_INCREASE_SIZE)
            {
                for (int i = 0; i < count; i++)
                {
                    Infrastructure.FixedSizeBuffer b = Infrastructure.FixedSizeBuffer.New(_bufferSize, this);
                    this.Return(b);
                }

                Console.WriteLine($"BufferPool Increase {count}; CurrentPoolSize= {CurrentPoolSize()}.");
            }






        }

    }
}
