// <copyright file="MurmurHash3x64.cs" company="Sedat Kapanoglu">
// Copyright (c) 2015-2019 Sedat Kapanoglu
// MIT License (see LICENSE file for details)
// </copyright>

namespace HashDepot
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// x64 platform implementation of MurmurHash3 algorithm.
    /// </summary>
    public static class MurmurHash3x64
    {
        /// <summary>
        /// Calculate 128-bit MurmurHash3 hash value using 64-bit version of the algorithm.
        /// </summary>
        /// <param name="stream">Input stream.</param>
        /// <param name="seed">Seed value.</param>
        /// <returns>128-bit hash value in a Span.</returns>
        public static unsafe byte[] Hash128(Stream stream, uint seed)
        {
            Require.NotNull(stream, nameof(stream));
            const int ulongSize = sizeof(ulong);
            const int blockSize = ulongSize * 2;

            const ulong c1 = 0x87c37b91114253d5UL;
            const ulong c2 = 0x4cf5ad432745937fUL;

            ulong h1 = seed;
            ulong h2 = seed;
            var buffer = new byte[blockSize];
            int length = 0;
            fixed (byte* bufPtr = buffer)
            {
                int readBytes;
                while ((readBytes = stream.Read(buffer, 0, blockSize)) == blockSize)
                {
                    ulong ik1 = BitConverter.ToUInt64(buffer, 0);
                    ulong ik2 = BitConverter.ToUInt64(buffer, ulongSize);

                    round(ref ik1, ref h1, c1, c2, h2, 31, 27, 0x52dce729U);
                    round(ref ik2, ref h2, c2, c1, h1, 33, 31, 0x38495ab5U);

                    length += blockSize;
                }

                ulong k1;
                ulong k2;

                if (readBytes > ulongSize)
                {
                    k2 = Bits.PartialBytesToUInt64(bufPtr + ulongSize, readBytes - ulongSize);
                    tailRound(ref k2, ref h2, c2, c1, 33);
                    readBytes = ulongSize;
                    length += ulongSize;
                }

                if (readBytes > 0)
                {
                    k1 = Bits.PartialBytesToUInt64(bufPtr, readBytes);
                    tailRound(ref k1, ref h1, c1, c2, 31);
                    length += readBytes;
                }
            }

            h1 ^= (ulong)length;
            h2 ^= (ulong)length;

            h1 += h2;
            h2 += h1;

            fmix64(ref h1);
            fmix64(ref h2);

            h1 += h2;
            h2 += h1;

            var result = new byte[16];
            fixed (byte* outputPtr = result)
            {
                ulong* pOutput = (ulong*)outputPtr;
                pOutput[0] = h1;
                pOutput[1] = h2;
            }

            return result;
        }

        /// <summary>
        /// Calculate 128-bit MurmurHash3 hash value using x64 version of the algorithm.
        /// </summary>
        /// <param name="buffer">Input buffer.</param>
        /// <param name="seed">Seed value.</param>
        /// <returns>128-bit hash value as a Span of bytes.</returns>
        public static unsafe Span<byte> Hash128(ReadOnlySpan<byte> buffer, uint seed)
        {
            const int blockSize = 16;

            const ulong c1 = 0x87c37b91114253d5UL;
            const ulong c2 = 0x4cf5ad432745937fUL;

            var result = new byte[16];

            ulong h1 = seed;
            ulong h2 = seed;

            int length = buffer.Length;
            int blockLen = Math.DivRem(length, blockSize, out int leftBytes);

            fixed (byte* bufPtr = buffer)
            {
                ulong* pItem = (ulong*)bufPtr;
                ulong* pEnd = (ulong*)bufPtr + blockLen;
                while (pItem != pEnd)
                {
                    ulong ik1 = *pItem++;
                    ulong ik2 = *pItem++;

                    round(ref ik1, ref h1, c1, c2, h2, 31, 27, 0x52dce729U);
                    round(ref ik2, ref h2, c2, c1, h1, 33, 31, 0x38495ab5U);
                }

                byte* pTail = (byte*)pItem;
                ulong k1;
                ulong k2;

                if (leftBytes > 8)
                {
                    k2 = Bits.PartialBytesToUInt64(pTail + 8, leftBytes - 8);
                    tailRound(ref k2, ref h2, c2, c1, 33);
                    leftBytes = 8;
                }

                if (leftBytes > 0)
                {
                    k1 = Bits.PartialBytesToUInt64(pTail, leftBytes);
                    tailRound(ref k1, ref h1, c1, c2, 31);
                }

                h1 ^= (ulong)length;
                h2 ^= (ulong)length;

                h1 += h2;
                h2 += h1;

                fmix64(ref h1);
                fmix64(ref h2);

                h1 += h2;
                h2 += h1;

                fixed (byte* outputPtr = result)
                {
                    ulong* pOutput = (ulong*)outputPtr;
                    pOutput[0] = h1;
                    pOutput[1] = h2;
                }

                return result;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void round(
            ref ulong k,
            ref ulong h,
            ulong c1,
            ulong c2,
            ulong hn,
            int krot,
            int hrot,
            uint x)
        {
            k *= c1;
            k = Bits.RotateLeft(k, krot);
            k *= c2;
            h ^= k;
            h = Bits.RotateLeft(h, hrot);
            h += hn;
            h = (h * 5) + x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void fmix64(ref ulong h)
        {
            h ^= h >> 33;
            h *= 0xff51afd7ed558ccdUL;
            h ^= h >> 33;
            h *= 0xc4ceb9fe1a85ec53UL;
            h ^= h >> 33;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void tailRound(ref ulong k, ref ulong h, ulong c1, ulong c2, int rot)
        {
            k *= c1;
            k = Bits.RotateLeft(k, rot);
            k *= c2;
            h ^= k;
        }
    }
}