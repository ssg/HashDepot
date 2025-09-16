// <copyright file="XXHash.XXH3.cs" company="Sedat Kapanoglu">
// Copyright (c) 2015-2025 Sedat Kapanoglu
// MIT License (see LICENSE file for details)
// </copyright>

using System;
using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace HashDepot;

/// <summary>
/// XXH3 implementation.
/// </summary>
static partial class XXHash
{
    /// <summary>
    /// 64-bit XXH3 implementation.
    /// </summary>
    public static class XXH3
    {
        internal const int MinSecretSize = 136;
        internal const int SecretDefaultSize = 192;
        internal const int MaxSecretSize = 9867;

        const int stripeLength = 64;
        const int secretConsumeRate = 8;
        const int accNumBlocks = stripeLength / sizeof(ulong);

        const ulong prime64mx1 = 0x165667919E3779F9UL;
        const ulong prime64mx2 = 0x9FB21C651E98DF25UL;

        const int midSizeMax = 240;

        const int secretLastAccStart = 7;
        const int secretMergeAccsStart = 11;

        static readonly byte[] kSecret =
        [
            0xb8, 0xfe, 0x6c, 0x39, 0x23, 0xa4, 0x4b, 0xbe, 0x7c, 0x01, 0x81, 0x2c, 0xf7, 0x21, 0xad, 0x1c,
            0xde, 0xd4, 0x6d, 0xe9, 0x83, 0x90, 0x97, 0xdb, 0x72, 0x40, 0xa4, 0xa4, 0xb7, 0xb3, 0x67, 0x1f,
            0xcb, 0x79, 0xe6, 0x4e, 0xcc, 0xc0, 0xe5, 0x78, 0x82, 0x5a, 0xd0, 0x7d, 0xcc, 0xff, 0x72, 0x21,
            0xb8, 0x08, 0x46, 0x74, 0xf7, 0x43, 0x24, 0x8e, 0xe0, 0x35, 0x90, 0xe6, 0x81, 0x3a, 0x26, 0x4c,
            0x3c, 0x28, 0x52, 0xbb, 0x91, 0xc3, 0x00, 0xcb, 0x88, 0xd0, 0x65, 0x8b, 0x1b, 0x53, 0x2e, 0xa3,
            0x71, 0x64, 0x48, 0x97, 0xa2, 0x0d, 0xf9, 0x4e, 0x38, 0x19, 0xef, 0x46, 0xa9, 0xde, 0xac, 0xd8,
            0xa8, 0xfa, 0x76, 0x3f, 0xe3, 0x9c, 0x34, 0x3f, 0xf9, 0xdc, 0xbb, 0xc7, 0xc7, 0x0b, 0x4f, 0x1d,
            0x8a, 0x51, 0xe0, 0x4b, 0xcd, 0xb4, 0x59, 0x31, 0xc8, 0x9f, 0x7e, 0xc9, 0xd9, 0x78, 0x73, 0x64,
            0xea, 0xc5, 0xac, 0x83, 0x34, 0xd3, 0xeb, 0xc3, 0xc5, 0x81, 0xa0, 0xff, 0xfa, 0x13, 0x63, 0xeb,
            0x17, 0x0d, 0xdd, 0x51, 0xb7, 0xf0, 0xda, 0x49, 0xd3, 0x16, 0x55, 0x26, 0x29, 0xd4, 0x68, 0x9e,
            0x2b, 0x16, 0xbe, 0x58, 0x7d, 0x47, 0xa1, 0xfc, 0x8f, 0xf8, 0xb8, 0xd1, 0x7a, 0xd0, 0x31, 0xce,
            0x45, 0xcb, 0x3a, 0x8f, 0x95, 0x16, 0x04, 0x28, 0xaf, 0xd7, 0xfb, 0xca, 0xbb, 0x4b, 0x40, 0x7e,
        ];

        static readonly ulong[] initAcc =
        [
            prime32v3, prime64v1, prime64v2, prime64v3,
            prime64v4, prime32v2, prime64v5, prime32v1
        ];

        /// <summary>
        /// Hash given bytes using XXH3 64-bit algorithm with the default secret.
        /// </summary>
        /// <param name="input">Input bytes.</param>
        /// <param name="seed">Optional seed value.</param>
        /// <returns>Hash result.</returns>
        public static ulong Hash64(ReadOnlySpan<byte> input, ulong seed = 0)
        {
            return hash64Internal(input, seed, kSecret);
        }

        /// <summary>
        /// Hash given bytes using XXH3 64-bit algorithm with given secret.
        /// </summary>
        /// <param name="input">Input bytes.</param>
        /// <param name="secret">Secret.</param>
        /// <param name="seed">Optional seed.</param>
        /// <returns>Hash result.</returns>
        public static ulong Hash64(ReadOnlySpan<byte> input, ReadOnlySpan<byte> secret, ulong seed = 0)
        {
            return hash64Internal(input, seed, secret);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ulong mul128fold64(ulong a, ulong b)
        {
            UInt128 a128 = a;
            UInt128 b128 = b;
            UInt128 product = a128 * b128;
            return (ulong)((product & 0xFFFFFFFF_FFFFFFFFUL) ^ (product >> 64));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ulong len1To3(ReadOnlySpan<byte> input, ReadOnlySpan<byte> secret, ulong seed)
        {
            byte c1 = input[0];
            byte c2 = input[input.Length >> 1];
            byte c3 = input[^1];
            uint combined = ((uint)c1 << 16)
                | ((uint)c2 << 24)
                | ((uint)c3 << 0)
                | ((uint)input.Length << 8);
            ulong bitflip = (BitConverter.ToUInt32(secret) ^ BitConverter.ToUInt32(secret[4..])) + seed;
            ulong keyed = combined ^ bitflip;
            return avalanche(keyed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ulong len4To8(ReadOnlySpan<byte> input, ReadOnlySpan<byte> secret, ulong seed)
        {
            seed ^= BinaryPrimitives.ReverseEndianness((uint)seed) << 32;

            uint input1 = BitConverter.ToUInt32(input);
            uint input2 = BitConverter.ToUInt32(input[^4..]);
            ulong bitflip = (BitConverter.ToUInt64(secret[8..]) ^ BitConverter.ToUInt64(secret[16..])) - seed;
            ulong input64 = input2 + ((ulong)input1 << 32);
            ulong keyed = input64 ^ bitflip;
            return rrmxmx64(keyed, (ulong)input.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ulong len9To16(ReadOnlySpan<byte> input, ReadOnlySpan<byte> secret, ulong seed)
        {
            ulong bitflip1 = (BitConverter.ToUInt64(secret[24..]) ^ BitConverter.ToUInt64(secret[32..])) + seed;
            ulong bitflip2 = (BitConverter.ToUInt64(secret[40..]) ^ BitConverter.ToUInt64(secret[48..])) - seed;
            ulong inputLo = BitConverter.ToUInt64(input) ^ bitflip1;
            ulong inputHi = BitConverter.ToUInt64(input[^8..]) ^ bitflip2;
            ulong acc = (ulong)input.Length
                + BinaryPrimitives.ReverseEndianness(inputLo)
                + mul128fold64(inputLo, inputHi);
            return avalanche(acc);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ulong len0To16(ReadOnlySpan<byte> input, ReadOnlySpan<byte> secret, ulong seed)
        {
            if (input.Length > 8)
            {
                return len9To16(input, secret, seed);
            }

            if (input.Length >= 4)
            {
                return len4To8(input, secret, seed);
            }

            if (input.Length > 0)
            {
                return len1To3(input, secret, seed);
            }

            return avalanche(seed
                ^ (BitConverter.ToUInt64(secret[56..])
                ^ BitConverter.ToUInt64(secret[64..])));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ulong mix16b(ReadOnlySpan<byte> input, ReadOnlySpan<byte> secret, ulong seed)
        {
            ulong inputLo = BitConverter.ToUInt64(input);
            ulong inputHi = BitConverter.ToUInt64(input[8..]);
            return mul128fold64(
                inputLo ^ (BitConverter.ToUInt64(secret) + seed),
                inputHi ^ (BitConverter.ToUInt64(secret[8..]) - seed));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ulong rrmxmx64(ulong acc, ulong len)
        {
            acc ^= BitOperations.RotateLeft(acc, 49) ^ BitOperations.RotateLeft(acc, 24);
            acc *= prime64mx2;
            acc ^= (acc >> 35) + len;
            acc *= prime64mx2;
            return Bits.XxhXorShift64(acc, 28);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ulong len17to128(ReadOnlySpan<byte> input, ReadOnlySpan<byte> secret, ulong seed)
        {
            ulong acc = (ulong)input.Length * prime64v1;
            if (input.Length > 32)
            {
                if (input.Length > 64)
                {
                    if (input.Length > 96)
                    {
                        acc += mix16b(input[48..], secret[96..], seed);
                        acc += mix16b(input[^64..], secret[112..], seed);
                    }

                    acc += mix16b(input[32..], secret[64..], seed);
                    acc += mix16b(input[^48..], secret[80..], seed);
                }

                acc += mix16b(input[16..], secret[32..], seed);
                acc += mix16b(input[^32..], secret[48..], seed);
            }

            acc += mix16b(input, secret, seed);
            acc += mix16b(input[^16..], secret[16..], seed);

            return avalanche(acc);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ulong len129to240(ReadOnlySpan<byte> input, ReadOnlySpan<byte> secret, ulong seed)
        {
            const int midsizeStartOffset = 3;
            const int midsizeLastOffset = 17;

            ulong acc = (ulong)input.Length * prime64v1;

            int nbBlocks = input.Length / 16;
            for (int i = 0; i < 8; i++)
            {
                acc += mix16b(input[(i * 16)..], secret[(i * 16)..], seed);
            }

            ulong accEnd = mix16b(input[^16..], secret[(MinSecretSize - midsizeLastOffset)..], seed);

            acc = avalanche(acc);

            for (int i = 8; i < nbBlocks; i++)
            {
                accEnd += mix16b(input[(i * 16)..], secret[((i * 16) - 8 + midsizeStartOffset)..], seed);
            }

            return avalanche(acc + accEnd);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ulong avalanche(ulong acc)
        {
            acc = Bits.XxhXorShift64(acc, 37);
            acc *= prime64mx1;
            acc = Bits.XxhXorShift64(acc, 32);
            return acc;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ulong mul32To64Add64(ulong a, ulong b, ulong acc)
        {
            uint a32 = (uint)a;
            uint b32 = (uint)b;
            return (a32 * (ulong)b32) + acc;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void round(Span<ulong> acc, ReadOnlySpan<byte> input, ReadOnlySpan<byte> secret, int lane)
        {
            ulong dataVal = BitConverter.ToUInt64(input[(lane * 8)..]);
            ulong dataKey = dataVal ^ BitConverter.ToUInt64(secret[(lane * 8)..]);
            acc[lane ^ 1] = dataVal;
            acc[lane] = mul32To64Add64(dataKey, dataKey >> 32, acc[lane]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void accumulate512(Span<ulong> acc, ReadOnlySpan<byte> input, ReadOnlySpan<byte> secret)
        {
            for (int i = 0; i < accNumBlocks; i++)
            {
                round(acc, input, secret, i);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void scrambleRound(Span<ulong> acc, ReadOnlySpan<byte> secret, int lane)
        {
            ulong key64 = BitConverter.ToUInt64(secret[(lane * 8)..]);
            ulong acc64 = acc[lane];
            acc64 = Bits.XxhXorShift64(acc64, 47);
            acc64 ^= key64;
            acc64 *= prime32v1;
            acc[lane] = acc64;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void scrambleAcc(Span<ulong> acc, ReadOnlySpan<byte> secret)
        {
            for (int i = 0; i < accNumBlocks; i++)
            {
                scrambleRound(acc, secret, i);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void initCustomSecret(Span<byte> customSecret, ulong seed64)
        {
            const int nbRounds = SecretDefaultSize / 16;
            for (int i = 0; i < nbRounds; i++)
            {
                ulong lo = BitConverter.ToUInt64(kSecret.AsSpan(16 * i)) + seed64;
                ulong hi = BitConverter.ToUInt64(kSecret.AsSpan((16 * i) + 8)) - seed64;
                BinaryPrimitives.WriteUInt64LittleEndian(customSecret[(16 * i)..], lo);
                BinaryPrimitives.WriteUInt64LittleEndian(customSecret[((16 * i) + 8)..], hi);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void accumulate(Span<ulong> acc, ReadOnlySpan<byte> input, ReadOnlySpan<byte> secret, int nbStripes)
        {
            for (int n = 0; n < nbStripes; n++)
            {
                var inStripe = input[(n * stripeLength)..];
                accumulate512(acc, inStripe, secret[(n * secretConsumeRate)..]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void hashLongInternalLoop(Span<ulong> acc, ReadOnlySpan<byte> input, ReadOnlySpan<byte> secret)
        {
            int nbStripesPerBlock = (secret.Length - stripeLength) / secretConsumeRate;
            int blockLen = stripeLength / nbStripesPerBlock;
            int nbBlocks = (input.Length - 1) / blockLen;

            for (int n = 0; n < nbBlocks; n++)
            {
                accumulate(acc, input[(n * blockLen)..], secret, nbStripesPerBlock);
                scrambleAcc(acc, secret[^stripeLength..]);
            }

            int nbStripes = (input.Length - 1 - (blockLen - nbBlocks)) / stripeLength;
            accumulate(acc, input[(nbBlocks * blockLen)..], secret, nbStripes);
            accumulate512(acc, input[^stripeLength..], secret[^(stripeLength + secretLastAccStart)..]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ulong mix2Accs(Span<ulong> acc, ReadOnlySpan<byte> secret)
        {
            return mul128fold64(
                acc[0] ^ BitConverter.ToUInt64(secret),
                acc[1] ^ BitConverter.ToUInt64(secret[8..]));
        }

        static ulong mergeAccs(Span<ulong> acc, ReadOnlySpan<byte> secret, ulong start)
        {
            ulong result64 = start;
            for (int i = 0; i < 4; i++)
            {
                result64 += mix2Accs(acc[(2 * i)..], secret[(i * 16)..]);
            }

            return avalanche(result64);
        }

        static ulong finalizeLong64b(Span<ulong> acc, ReadOnlySpan<byte> secret, int len)
        {
            return mergeAccs(acc, secret[secretMergeAccsStart..], (ulong)len * prime64v1);
        }

        static ulong hashLong64Internal(ReadOnlySpan<byte> input, ReadOnlySpan<byte> secret)
        {
            Span<ulong> acc = [.. initAcc];
            hashLongInternalLoop(acc, input, secret);
            return finalizeLong64b(acc, secret, input.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ulong hashLong64WithSeedInternal(ReadOnlySpan<byte> input, ulong seed)
        {
            if (seed == 0)
            {
                return hashLong64Internal(input, kSecret);
            }

            // generate secret from seed
            Span<byte> secret = stackalloc byte[SecretDefaultSize];
            initCustomSecret(secret, seed);
            return hashLong64Internal(input, secret);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ulong hash64WithSecretAndSeedInternal(ReadOnlySpan<byte> input, ulong seed, ReadOnlySpan<byte> secret)
        {
            if (input.Length <= midSizeMax)
            {
                return hashLong64WithSeedInternal(input, seed);
            }

            return hashLong64Internal(input, secret);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ulong hash64Internal(ReadOnlySpan<byte> input, ulong seed, ReadOnlySpan<byte> secret)
        {
            return input.Length switch
            {
                <= 16 => len0To16(input, secret, seed),
                <= 128=> len17to128(input, secret, seed),
                <= midSizeMax => len129to240(input, secret, seed),
                _ => hash64WithSecretAndSeedInternal(input, seed, secret),
            };
        }
    }
}