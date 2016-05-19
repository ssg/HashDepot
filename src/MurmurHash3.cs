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
    }
}