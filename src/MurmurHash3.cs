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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void murmur128Round(ref uint kn, ref uint hn, uint hm, uint cn, uint cm, uint n, int kr, int hr)
        {
            const uint m = 5;

            kn *= cn;
            kn = Bits.RotateLeft(kn, kr);
            kn *= cm;
            hn ^= kn;
            hn = Bits.RotateLeft(hn, hr);
            hn += hm;
            hn = hn * m + n;
        }

        public static byte[] Hash128(byte[] buffer, uint seed)
        {
            const int outputLength = 16;
            const int blockSize = 16;
            const uint c1 = 0x239b961b;
            const uint c2 = 0xab0e9789;
            const uint c3 = 0x38b34ae5;
            const uint c4 = 0xa1e38b93;

            const uint n1 = 0x561ccd1b;
            const uint n2 = 0x0bcaa747;
            const uint n3 = 0x96cd1c35;
            const uint n4 = 0x32ac3b17;

            var result = new byte[outputLength];

            uint h1 = seed;
            uint h2 = seed;
            uint h3 = seed;
            uint h4 = seed;

            int length = buffer.Length;
            int end = length - length % blockSize;

            for (int i = 0; i < end; i+= blockSize)
            {
                int offset = i * 4;
                uint k1 = BitConverter.ToUInt32(buffer, offset);
                uint k2 = BitConverter.ToUInt32(buffer, offset + 4);
                uint k3 = BitConverter.ToUInt32(buffer, offset + 8);
                uint k4 = BitConverter.ToUInt32(buffer, offset + 12);

                murmur128Round(ref k1, ref h1, h2, c1, c2, n1, 15, 19);
                murmur128Round(ref k2, ref h2, h3, c2, c3, n2, 16, 17);
                murmur128Round(ref k3, ref h3, h4, c3, c4, n3, 17, 15);
                murmur128Round(ref k4, ref h4, h1, c4, c1, n4, 18, 13);
            }
            /// not finished
            return result;
        }
    }
}