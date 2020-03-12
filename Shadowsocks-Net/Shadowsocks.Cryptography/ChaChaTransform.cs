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
    public sealed class ChaChaTransform : ICryptoTransform
    {
        #region Constant Members
        /// <summary>
        /// The length of a ChaCha block (in bytes).
        /// </summary>
        public const int BlockLength = (NumWordsPerBlock * WordLength);
        /// <summary>
        /// The number of words in a single ChaCha block.
        /// </summary>
        public const int NumWordsPerBlock = 16;
        /// <summary>
        /// The length of a ChaCha word (in bytes).
        /// </summary>
        public const int WordLength = sizeof(int);
        #endregion

        #region Static Members
        /// <summary>
        /// Executes eight QuarterRound operations (four "column-rounds" and four "row-rounds").
        /// </summary>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DoubleRound(
            ref uint s0, ref uint s1, ref uint s2, ref uint s3,
            ref uint s4, ref uint s5, ref uint s6, ref uint s7,
            ref uint s8, ref uint s9, ref uint sA, ref uint sB,
            ref uint sC, ref uint sD, ref uint sE, ref uint sF
        )
        {
            QuarterRound(ref s0, ref s4, ref s8, ref sC);
            QuarterRound(ref s1, ref s5, ref s9, ref sD);
            QuarterRound(ref s2, ref s6, ref sA, ref sE);
            QuarterRound(ref s3, ref s7, ref sB, ref sF);
            QuarterRound(ref s0, ref s5, ref sA, ref sF);
            QuarterRound(ref s1, ref s6, ref sB, ref sC);
            QuarterRound(ref s2, ref s7, ref s8, ref sD);
            QuarterRound(ref s3, ref s4, ref s9, ref sE);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ChaChaTransform"/> class.
        /// </summary>
        /// <param name="state">The state used during initialization.</param>
        /// <param name="iv">The initial value parameter.</param>
        /// <param name="numRounds">The number of rounds to perform per block.</param>
        [CLSCompliant(false)]
        public static ChaChaTransform New(ReadOnlySpan<uint> state, ulong iv, uint numRounds) => new ChaChaTransform(state, iv, numRounds);
        /// <summary>
        /// Executes four SingleRound operations.
        /// </summary>
        /// <param name="w">The w state parameter.</param>
        /// <param name="x">The x state parameter.</param>
        /// <param name="y">The y state parameter.</param>
        /// <param name="z">The z state parameter.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void QuarterRound(ref uint w, ref uint x, ref uint y, ref uint z)
        {
            SingleRound(ref w, ref z, x, 16);
            SingleRound(ref y, ref x, z, 12);
            SingleRound(ref w, ref z, x, 8);
            SingleRound(ref y, ref x, z, 7);
        }
        /// <summary>
        /// Executes the basic operation of the ChaCha algorithm which "mixes" three state variables per invocation.
        /// </summary>
        /// <param name="x">The x state parameter.</param>
        /// <param name="z">The y state parameter.</param>
        /// <param name="y">The z state parameter.</param>
        /// <param name="count">The number of rotations.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SingleRound(ref uint x, ref uint y, uint z, int count)
        {
            y = BitwiseHelpers.RotateLeft((y ^ (x += z)), count);
            //((value << count) | (value >> ((-count) & ((sizeof(uint) * 8) - 1))))
        }
        /// <summary>
        /// Writes a key stream block to a span using the specified parameters.
        /// </summary>
        /// <param name="state">The ChaCha state parameter.</param>
        /// <param name="iv">The initial value parameter.</param>
        /// <param name="numRounds">The number of rounds to perform per block.</param>
        /// <param name="destination">The destination span.</param>
        [CLSCompliant(false)]
        public static void WriteKeyStreamBlock(ReadOnlySpan<uint> state, ulong iv, uint numRounds, Span<byte> destination)
        {
            var destinationAsUInt = MemoryMarshal.Cast<byte, uint>(destination);
            var ivLow = ((uint)BitwiseHelpers.ExtractLow(iv));
            var ivHigh = ((uint)BitwiseHelpers.ExtractHigh(iv));
            var t0 = state[0];
            var t1 = state[1];
            var t2 = state[2];
            var t3 = state[3];
            var t4 = state[4];
            var t5 = state[5];
            var t6 = state[6];
            var t7 = state[7];
            var t8 = state[8];
            var t9 = state[9];
            var tA = state[10];
            var tB = state[11];
            var tC = ivLow;
            var tD = ivHigh;
            var tE = state[14];
            var tF = state[15];

            numRounds = (numRounds >> 1);

            for (var i = 0U; (i < numRounds); i++)
            {
                DoubleRound(
                    ref t0, ref t1, ref t2, ref t3,
                    ref t4, ref t5, ref t6, ref t7,
                    ref t8, ref t9, ref tA, ref tB,
                    ref tC, ref tD, ref tE, ref tF
                );
            }

            destinationAsUInt[0] = (t0 + state[0]);
            destinationAsUInt[1] = (t1 + state[1]);
            destinationAsUInt[2] = (t2 + state[2]);
            destinationAsUInt[3] = (t3 + state[3]);
            destinationAsUInt[4] = (t4 + state[4]);
            destinationAsUInt[5] = (t5 + state[5]);
            destinationAsUInt[6] = (t6 + state[6]);
            destinationAsUInt[7] = (t7 + state[7]);
            destinationAsUInt[8] = (t8 + state[8]);
            destinationAsUInt[9] = (t9 + state[9]);
            destinationAsUInt[10] = (tA + state[10]);
            destinationAsUInt[11] = (tB + state[11]);
            destinationAsUInt[12] = (tC + ivLow);
            destinationAsUInt[13] = (tD + ivHigh);
            destinationAsUInt[14] = (tE + state[14]);
            destinationAsUInt[15] = (tF + state[15]);
        }
        #endregion

        #region Instance Members
        private readonly uint m_numRounds;
        private readonly uint[] m_state;

        private ulong m_iv;

        /// <summary>
        /// Gets a value indicating whether multiple blocks can be transformed.
        /// </summary>
        public bool CanTransformMultipleBlocks => true;
        /// <summary>
        /// Gets a value indicating whether the current transform can be reused.
        /// </summary>
        public bool CanReuseTransform => true;
        /// <summary>
        /// Gets the input block size (in bytes).
        /// </summary>
        public int InputBlockSize => BlockLength;
        /// <summary>
        ///  Gets the output block size (in bytes).
        /// </summary>
        public int OutputBlockSize => BlockLength;

        private ChaChaTransform(ReadOnlySpan<uint> state, ulong iv, uint numRounds)
        {
            m_iv = iv;
            m_numRounds = numRounds;
            m_state = state.ToArray();
        }

        /// <summary>
        /// Releases all resources used by this <see cref="ChaChaTransform"/> instance.
        /// </summary>
        public void Dispose() { }
        /// <summary>
        /// Transforms the specified region of the data array and copies the resulting transform to the specified region of the destination array.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataOffset"></param>
        /// <param name="dataCount"></param>
        /// <param name="destination"></param>
        /// <param name="destinationOffset"></param>
        public int TransformBlock(byte[] data, int dataOffset, int dataCount, byte[] destination, int destinationOffset)
        {
            var dataBuffer = data.AsSpan(dataOffset);
            var destinationBuffer = destination.AsSpan(destinationOffset);
            var keyStreamBuffer = (Span<byte>)stackalloc byte[BlockLength];
            var keyStreamPosition = m_iv;
            var numBytesTransformed = 0;

            while (BlockLength <= dataCount)
            {
                var destinationBufferSlice = destinationBuffer.Slice(numBytesTransformed, BlockLength);

                dataBuffer.Slice(numBytesTransformed, BlockLength).CopyTo(destinationBufferSlice);
                WriteKeyStreamBlock(m_state, checked(keyStreamPosition++), m_numRounds, keyStreamBuffer);
                ((ReadOnlySpan<byte>)keyStreamBuffer.Slice(0, BlockLength)).Xor(destinationBufferSlice);

                dataCount -= BlockLength;
                numBytesTransformed += BlockLength;
            }

            if (0 < dataCount)
            {
                var destinationBufferSlice = destinationBuffer.Slice(numBytesTransformed, dataCount);

                dataBuffer.Slice(numBytesTransformed, dataCount).CopyTo(destinationBufferSlice);
                WriteKeyStreamBlock(m_state, checked(keyStreamPosition++), m_numRounds, keyStreamBuffer);
                ((ReadOnlySpan<byte>)keyStreamBuffer.Slice(0, dataCount)).Xor(destinationBufferSlice);

                numBytesTransformed += dataCount;
            }

            m_iv = keyStreamPosition;

            return numBytesTransformed;
        }
        /// <summary>
        /// Transforms the specified region of the specified array.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataOffset"></param>
        /// <param name="dataCount"></param>
        public byte[] TransformFinalBlock(byte[] data, int dataOffset, int dataCount)
        {
            var result = new byte[dataCount];

            TransformBlock(data, dataOffset, dataCount, result, 0);

            return result;
        }
        #endregion
    }
}
