// Copyright (c) 2015, 2016 Sedat Kapanoglu
// MIT License - see LICENSE file for details

using System;
using System.Runtime.CompilerServices;

namespace HashDepot
{
    /// <summary>
    /// SipHash 2-4 algorithm
    /// </summary>
    public static class SipHash
    {
        private const int keyLength = 16;

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
        public static unsafe ulong Hash64(byte[] buffer, byte[] key)
        {
            const ulong finalVectorXor = 0xFF;

            Require.NotNull(buffer, "buffer");
            Require.NotNull(key, "key");

            if (key.Length != keyLength)
            {
                throw new ArgumentException("key must be 16-bytes long", "key");
            }

            ulong k0;
            ulong k1;
            fixed (byte* keyPtr = key)
            {
                ulong* pKey = (ulong*)keyPtr;
                k0 = *pKey++;
                k1 = *pKey;
            }

            ulong v0 = initv0 ^ k0;
            ulong v1 = initv1 ^ k1;
            ulong v2 = initv2 ^ k0;
            ulong v3 = initv3 ^ k1;

            int length = buffer.Length;
            ulong lastWord = (ulong)length << 56;
            int left;
            int numUlongs = Math.DivRem(length, sizeof(ulong), out left);

            fixed (byte* bufPtr = buffer)
            {
                ulong* pInput = (ulong*)bufPtr;
                ulong* pEnd = pInput + numUlongs;
                while (pInput != pEnd)
                {
                    ulong m = *pInput++;
                    v3 ^= m;
                    sipRoundC(ref v0, ref v1, ref v2, ref v3);
                    v0 ^= m;
                }
                if (left > 0)
                {
                    lastWord |= Bits.PartialBytesToUInt64((byte*)pInput, left);
                }

                v3 ^= lastWord;
                sipRoundC(ref v0, ref v1, ref v2, ref v3);
                v0 ^= lastWord;

                v2 ^= finalVectorXor;
                sipRoundD(ref v0, ref v1, ref v2, ref v3);
                return v0 ^ v1 ^ v2 ^ v3;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void sipRoundC(ref ulong v0, ref ulong v1, ref ulong v2, ref ulong v3)
        {
            sipRound(ref v0, ref v1, ref v2, ref v3);
            sipRound(ref v0, ref v1, ref v2, ref v3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void sipRoundD(ref ulong v0, ref ulong v1, ref ulong v2, ref ulong v3)
        {
            sipRound(ref v0, ref v1, ref v2, ref v3);
            sipRound(ref v0, ref v1, ref v2, ref v3);
            sipRound(ref v0, ref v1, ref v2, ref v3);
            sipRound(ref v0, ref v1, ref v2, ref v3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void sipRound(ref ulong v0, ref ulong v1, ref ulong v2, ref ulong v3)
        {
            v0 += v1;
            v1 = Bits.RotateLeft(v1, 13);
            v1 ^= v0;
            v0 = Bits.RotateLeft(v0, 32);

            v2 += v3;
            v3 = Bits.RotateLeft(v3, 16);
            v3 ^= v2;

            v2 += v1;
            v1 = Bits.RotateLeft(v1, 17);
            v1 ^= v2;
            v2 = Bits.RotateLeft(v2, 32);

            v0 += v3;
            v3 = Bits.RotateLeft(v3, 21);
            v3 ^= v0;
        }
    }
}