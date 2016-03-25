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

            unchecked
            {
                value *= c1;
                value = Bits.RotateLeft(value, 15);
                value *= c2;
                hash ^= value;
            }
        }

        public static uint Hash32(byte[] buffer, uint seed)
        {
            const int uintSize = sizeof(uint);
            const uint final1 = 0x85ebca6b;
            const uint final2 = 0xc2b2ae35;
            const uint n = 0xe6546b64;
            const uint m = 5;

            uint hash = seed;
            int length = buffer.Length;
            int end = length - length % uintSize;
            for (int i = 0; i < end; i += uintSize)
            {
                uint k = BitConverter.ToUInt32(buffer, i);
                murmurRound(ref k, ref hash);
                hash = Bits.RotateLeft(hash, 13);
                hash *= m;
                hash += n;
            }
            int left = length - end;
            if (left > 0)
            {
                uint remaining = Bits.PartialBytesToUInt32(buffer, end, left);
                murmurRound(ref remaining, ref hash);
            }

            hash ^= (uint)length;

            // finalization mix
            hash ^= hash >> 16;
            hash = unchecked(hash * final1);
            hash ^= hash >> 13;
            hash = unchecked(hash * final2);
            hash ^= hash >> 16;
            return hash;
        }
    }
}