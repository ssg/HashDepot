// Copyright (c) 2015, 2016 Sedat Kapanoglu
// MIT License - see LICENSE file for details

using System;
using System.Runtime.CompilerServices;

namespace HashDepot
{
    public static class MurmurHash3
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void murmurRound(ref uint value, ref uint hash)
        {
            const uint c1 = 0xcc9e2d51;
            const uint c2 = 0x1b873593;

            value *= c1;
            value = Bits.RotateLeft(value, 15);
            value *= c2;
            hash ^= value;
        }

        public static unsafe uint Hash32(byte[] buffer, uint seed)
        {
            Require.NotNull(buffer, "buffer");

            const int uintSize = sizeof(uint);
            const uint final1 = 0x85ebca6b;
            const uint final2 = 0xc2b2ae35;
            const uint n = 0xe6546b64;
            const uint m = 5;

            uint hash = seed;
            int length = buffer.Length;
            int leftBytes;
            int numUInts = Math.DivRem(length, uintSize, out leftBytes);
            fixed (byte* bufPtr = buffer)
            {
                uint* pInput = (uint*)bufPtr;
                for (uint* pEnd = pInput + numUInts; pInput != pEnd; pInput++)
                {
                    uint k = *pInput;
                    murmurRound(ref k, ref hash);
                    hash = Bits.RotateLeft(hash, 13);
                    hash *= m;
                    hash += n;
                }
                if (leftBytes > 0)
                {
                    uint remaining = Bits.PartialBytesToUInt32((byte*)pInput, leftBytes);
                    murmurRound(ref remaining, ref hash);
                }
            }
            hash ^= (uint)length;

            // finalization mix
            hash ^= hash >> 16;
            hash *= final1;
            hash ^= hash >> 13;
            hash *= final2;
            hash ^= hash >> 16;
            return hash;
        }

        public static unsafe byte[] Hash128(byte[] buffer, uint seed)
        {
            const int blockSize = 16;

            const ulong c1 = 0x87c37b91114253d5UL;
            const ulong c2 = 0x4cf5ad432745937fUL;

            Require.NotNull(buffer, "buffer");

            var result = new byte[16];

            ulong h1 = seed;
            ulong h2 = seed;

            int length = buffer.Length;
            int leftBytes;
            int blockLen = Math.DivRem(length, blockSize, out leftBytes);

            fixed (byte* bufPtr = buffer)
            {
                ulong* pItem = (ulong*)bufPtr;
                ulong* pEnd = (ulong*)bufPtr + blockLen;
                while (pItem != pEnd)
                {
                    ulong ik1 = *pItem++;
                    ulong ik2 = *pItem++;

                    murmur128Round(ref ik1, ref h1, c1, c2, h2, 31, 27, 0x52dce729U);
                    murmur128Round(ref ik2, ref h2, c2, c1, h1, 33, 31, 0x38495ab5U);
                }

                byte* pTail = (byte*)pItem;
                ulong k1 = 0;
                ulong k2 = 0;

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
        private static void murmur128Round(ref ulong k, ref ulong h, ulong c1, ulong c2, ulong hn, int krot, int hrot, uint x)
        {
            k *= c1;
            k = Bits.RotateLeft(k, krot);
            k *= c2;
            h ^= k;
            h = Bits.RotateLeft(h, hrot);
            h += hn;
            h = h * 5 + x;
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