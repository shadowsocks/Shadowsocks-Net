using System;
using System.Buffers;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Shadowsocks.Cryptography
{

    /// <summary>
    /// A collection of extension methods that directly or indirectly augment the <see cref="Span{T}"/> class.
    /// </summary>
    public static class SpanExtensions
    {
        /// <summary>
        /// Returns a new value containing the contents of both specified spans.
        /// </summary>
        /// <typeparam name="T">The type that is encapsulated by each span.</typeparam>
        /// <param name="x">The first span that will be copied.</param>
        /// <param name="y">The second span that will be copied.</param>
        public static ReadOnlySpan<T> Append<T>(this ReadOnlySpan<T> x, ReadOnlySpan<T> y)
        {
            var spanLength = x.Length;
            var buffer = new T[(spanLength + y.Length)];
            var bufferSpan = buffer.AsSpan();

            x.CopyTo(bufferSpan.Slice(0, spanLength));
            y.CopyTo(bufferSpan.Slice(spanLength));

            return new ReadOnlySpan<T>(buffer);
        }
        /// <summary>
        /// Compares the contents of two spans for equality in constant time.
        /// </summary>
        /// <param name="x">The first span that will be compared.</param>
        /// <param name="y">The second span that will be compared.</param>
        public static bool CompareInConstantTime(this ReadOnlySpan<byte> x, ReadOnlySpan<byte> y)
        {
            var max = ((x.Length < y.Length) ? y.Length : x.Length);
            var min = ((x.Length > y.Length) ? y.Length : x.Length);
            var offset = 0;
            var result = 0;
            var z = (Span<byte>)stackalloc byte[] { byte.MinValue, byte.MaxValue };

            if (Vector.IsHardwareAccelerated)
            {
                var vectorX = MemoryMarshal.Cast<byte, Vector<byte>>(x.Slice(0, min));
                var vectorY = MemoryMarshal.Cast<byte, Vector<byte>>(y.Slice(0, min));
                var vectorCount = vectorX.Length;
                var vectorResult = Vector<byte>.Zero;

                for (var i = offset; (i < vectorCount); i++)
                {
                    vectorResult |= (vectorX[i] ^ vectorY[i]);
                }

                offset = (Vector<byte>.Count * vectorCount);
                result |= ((Vector<byte>.Zero == vectorResult) ? byte.MinValue : byte.MaxValue);
            }

            for (var i = offset; (i < min); i++)
            {
                result |= (x[i] ^ y[i]);
            }

            for (var i = min; (i < max); i++)
            {
                result |= (z[0] ^ z[1]);
            }

            return (0 == result);
        }
        /// <summary>
        /// Performs an in-place exclusive or operation between the elements of two spans; only the second span is modified.
        /// </summary>
        /// <param name="x">The first span; contents will remain unchanged.</param>
        /// <param name="y">The second span; contents will potentially be modified.</param>
        public static ReadOnlySpan<byte> Xor(this ReadOnlySpan<byte> x, Span<byte> y)
        {
            var count = (y.Length < x.Length ? y.Length : x.Length);
            var offset = 0;

            if (Vector.IsHardwareAccelerated)
            {
                var vectorDestination = MemoryMarshal.Cast<byte, Vector<byte>>(y.Slice(0, count));
                var vectorSource = MemoryMarshal.Cast<byte, Vector<byte>>(x.Slice(0, count));
                var vectorCount = vectorDestination.Length;

                for (var i = offset; (i < vectorCount); i++)
                {
                    vectorDestination[i] ^= vectorSource[i];
                }

                offset = (Vector<byte>.Count * vectorCount);
            }

            for (var i = offset; (i < count); i++)
            {
                y[i] ^= x[i];
            }

            return y;
        }
    }
}
