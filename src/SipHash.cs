// Copyright (c) 2015, 2016 Sedat Kapanoglu
// MIT License - see LICENSE file for details

using System;
using System.Runtime.CompilerServices;

namespace HashDepot
{
    public class SipHash
    {
        private const int keyLength = 16;
        private const int ulongSize = sizeof(ulong);

        private const int cRounds = 2;
        private const int dRounds = 4;
        private const ulong initv0 = 0x736f6d6570736575U;
        private const ulong initv1 = 0x646f72616e646f6dU;
        private const ulong initv2 = 0x6c7967656e657261U;
        private const ulong initv3 = 0x7465646279746573U;

        /// <summary>
        /// Calculate 64-bit SipHash-2-4 algorithm using the given key and the input.
        /// </summary>
        /// <param name="buffer">Input buffer</param>
        /// <param name="key">16-byte key</param>
        /// <returns>64-bit hash value</returns>
        public static ulong Hash64(byte[] buffer, byte[] key)
        {
            validateArguments(buffer, key);

            const ulong finalVectorXor = 0xFF;

            ulong k0 = BitConverter.ToUInt64(key, 0);
            ulong k1 = BitConverter.ToUInt64(key, 8);

            ulong v0 = initv0 ^ k0;
            ulong v1 = initv1 ^ k1;
            ulong v2 = initv2 ^ k0;
            ulong v3 = initv3 ^ k1;

            int length = buffer.Length;
            int end = length - length % 8;

            for (int n = 0; n < end; n += ulongSize)
            {
                ulong m = BitConverter.ToUInt64(buffer, n);
                v3 ^= m;
                sipRound(cRounds, ref v0, ref v1, ref v2, ref v3);
                v0 ^= m;
            }

            ulong lastWord = (((ulong)length) << 56)
                | partialBytesToUint64(buffer, end, length & 7);

            v3 ^= lastWord;
            sipRound(cRounds, ref v0, ref v1, ref v2, ref v3);
            v0 ^= lastWord;

            v2 ^= finalVectorXor;
            sipRound(dRounds, ref v0, ref v1, ref v2, ref v3);
            return v0 ^ v1 ^ v2 ^ v3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong rotateLeft(ulong value, byte bits)
        {
            return (value << bits) | (value >> (64 - bits));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void sipRound(int rounds, ref ulong v0, ref ulong v1, ref ulong v2, ref ulong v3)
        {
            // trying to modify rounds itself disables inlining
            // that's why we use a separate local variable
            for (int i = 0; i < rounds; i++)
            {
                v0 += v1;
                v1 = rotateLeft(v1, 13);
                v1 ^= v0;
                v0 = rotateLeft(v0, 32);

                v2 += v3;
                v3 = rotateLeft(v3, 16);
                v3 ^= v2;

                v2 += v1;
                v1 = rotateLeft(v1, 17);
                v1 ^= v2;
                v2 = rotateLeft(v2, 32);

                v0 += v3;
                v3 = rotateLeft(v3, 21);
                v3 ^= v0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong partialBytesToUint64(byte[] buffer, int offset, int leftBytes)
        {
            ulong result = 0;
            // trying to modify leftBytes would invalidate inlining
            // need to use local variable instead
            for (int i = leftBytes - 1; i >= 0; --i)
            {
                result |= ((ulong)buffer[offset + i]) << (i << 3);
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void validateArguments(byte[] buffer, byte[] key)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            if (key.Length != keyLength)
            {
                throw new ArgumentException("key must be 16-bytes long", "key");
            }
        }
    }
}