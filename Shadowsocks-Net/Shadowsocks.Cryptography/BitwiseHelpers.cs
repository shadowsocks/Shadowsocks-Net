using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Buffers;
using System.Buffers.Binary;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Runtime.CompilerServices;

namespace Shadowsocks.Cryptography
{
    /// <summary>
    /// A collection of static methods that implement <a href="https://en.wikipedia.org/wiki/Bitwise_operation">bitwise operations</a>.
    /// </summary>
    /// <remarks>
    /// Our definition of a "bitwise" function slightly differs from the accepted norm.
    /// </remarks>
    public static class BitwiseHelpers
    {
        /// <summary>
        /// Permutes a value using by executing a circular shift to the left.
        /// </summary>
        /// <param name="value">The integer that will have its bits rotated.</param>
        /// <param name="count">The amount of times the permutation will be performed.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RotateLeft(uint value, int count) => ((value << count) | (value >> ((-count) & ((sizeof(uint) * 8) - 1))));
        /// <summary>
        /// Permutes a value using by executing a circular shift to the left.
        /// </summary>
        /// <param name="value">The integer that will have its bits rotated.</param>
        /// <param name="count">The amount of times the permutation will be performed.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong RotateLeft(ulong value, int count) => ((value << count) | (value >> ((-count) & ((sizeof(ulong) * 8) - 1))));


        /// <summary>
        /// Extracts the least significant bits of the integer.
        /// </summary>
        /// <param name="value">The integer that bits will be extracted from.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ExtractLow(ulong value) => ((uint)value);
        /// <summary>
        /// Extracts the most significant bits of the integer.
        /// </summary>
        /// <param name="value">The integer that bits will be extracted from.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ExtractHigh(ulong value) => (value >> 32);


        /// <summary>
        /// Returns an unsigned 32-bit integer by concatenating the bit patterns of two unsigned 16-bit integers.
        /// </summary>
        /// <param name="highPart">The most significant half of the integer.</param>
        /// <param name="lowPart">The least significant half of the integer.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ConcatBits(ushort highPart, ushort lowPart) => ((((uint)highPart) << 16) | lowPart);
        /// <summary>
        /// Returns an unsigned 64-bit integer by concatenating the bit patterns of two unsigned 32-bit integers.
        /// </summary>
        /// <param name="highPart">The most significant half of the integer.</param>
        /// <param name="lowPart">The least significant half of the integer.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ConcatBits(uint highPart, uint lowPart) => ((((ulong)highPart) << 32) | lowPart);

        /// <summary>
        /// Adds two 32-bit unsigned integers into a 64-bit result.
        /// </summary>
        /// <param name="x">The first integer that will be added.</param>
        /// <param name="y">The second integer that will be added.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Add(uint x, uint y) => (((ulong)x) + y);


        /// <summary>
        /// Multiplies two 32-bit unsigned integers into a 64-bit result.
        /// </summary>
        /// <param name="x">The first integer that will be multiplied.</param>
        /// <param name="y">The second integer that will be multiplied.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Multiply(uint x, uint y) => (((ulong)x) * y);
        /// <summary>
        /// Multiplies two 64-bit unsigned integers into a pair of 64-bit numbers that represent the high and low portions of the full 128-bit result.
        /// </summary>
        /// <param name="x">The first integer that will be multiplied.</param>
        /// <param name="y">The second integer that will be multiplied.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (ulong highPart, ulong lowPart) Multiply(ulong x, ulong y)
        {
            var x0 = ExtractLow(x);
            var x1 = ExtractHigh(x);
            var y0 = ExtractLow(y);
            var y1 = ExtractHigh(y);

            var xLyL = (x0 * y0);
            var xLyH = (x0 * y1);
            var xHyL = (x1 * y0);
            var xHyH = (x1 * y1);

            var carry = ExtractHigh(ExtractHigh(xLyL) + ExtractLow(xLyH) + xHyL);

            return (
                highPart: (ExtractHigh(xLyH) + xHyH + carry),
                lowPart: (x * y)
            );
        }

    }
}
