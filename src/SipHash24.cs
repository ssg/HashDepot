// <copyright file="SipHash24.cs" company="Sedat Kapanoglu">
// Copyright (c) 2015-2019 Sedat Kapanoglu
// MIT License (see LICENSE file for details)
// </copyright>

namespace HashDepot
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    /// <summary>
    /// SipHash 2-4 algorithm. This is the most common implementation of SipHash.
    /// </summary>
    public static class SipHash24
    {
        private const int keyLength = 16;

        private const ulong initv0 = 0x736f6d6570736575U;
        private const ulong initv1 = 0x646f72616e646f6dU;
        private const ulong initv2 = 0x6c7967656e657261U;
        private const ulong initv3 = 0x7465646279746573U;

        /// <summary>
        /// Calculate 64-bit SipHash-2-4 algorithm using the given key and the input.
        /// </summary>
        /// <param name="buffer">Input buffer.</param>
        /// <param name="key">16-byte key.</param>
        /// <returns>64-bit hash value.</returns>
        public static ulong Hash64(byte[] buffer, byte[] key)
        {
            return Hash64(buffer.AsSpan(), key.AsSpan());
        }

        /// <summary>
        /// Calculate 64-bit SipHash-2-4 algorithm using the given key and the input.
        /// </summary>
        /// <param name="stream">Input stream.</param>
        /// <param name="key">16-byte key.</param>
        /// <returns>64-bit hash value.</returns>
        public static unsafe ulong Hash64(Stream stream, ReadOnlySpan<byte> key)
        {
            Require.NotNull(stream, nameof(stream));
            const ulong finalVectorXor = 0xFF;
            const int ulongSize = sizeof(ulong);

            if (key.Length != keyLength)
            {
                throw new ArgumentException("Key must be 16-bytes long", nameof(key));
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

            ulong length = 0;

            int bytesRead;
            var buffer = new byte[ulongSize];
            while ((bytesRead = stream.Read(buffer, 0, ulongSize)) == ulongSize)
            {
                ulong m = BitConverter.ToUInt64(buffer, 0);
                v3 ^= m;
                sipRoundC(ref v0, ref v1, ref v2, ref v3);
                v0 ^= m;
                length += (ulong)bytesRead;
            }

            length += (ulong)bytesRead;
            ulong lastWord = length << 56;

            if (bytesRead > 0)
            {
                fixed (byte* bufPtr = buffer)
                {
                    lastWord |= Bits.PartialBytesToUInt64(bufPtr, bytesRead);
                }
            }

            v3 ^= lastWord;
            sipRoundC(ref v0, ref v1, ref v2, ref v3);
            v0 ^= lastWord;

            v2 ^= finalVectorXor;
            sipRoundD(ref v0, ref v1, ref v2, ref v3);
            return v0 ^ v1 ^ v2 ^ v3;
        }

        /// <summary>
        /// Calculate 64-bit SipHash-2-4 algorithm using the given key and the input.
        /// </summary>
        /// <param name="stream">Input stream.</param>
        /// <param name="key">16-byte key.</param>
        /// <returns>A Task representing the 64-bit hash computation.</returns>
        public static async Task<ulong> Hash64Async(Stream stream, byte[] key)
        {
            Require.NotNull(stream, nameof(stream));
            Require.NotNull(key, nameof(key));

            const ulong finalVectorXor = 0xFF;
            const int ulongSize = sizeof(ulong);

            if (key.Length != keyLength)
            {
                throw new ArgumentException("Key must be 16-bytes long", nameof(key));
            }

            ulong k0 = BitConverter.ToUInt64(key, sizeof(ulong));
            ulong k1 = BitConverter.ToUInt64(key, 0);

            ulong v0 = initv0 ^ k0;
            ulong v1 = initv1 ^ k1;
            ulong v2 = initv2 ^ k0;
            ulong v3 = initv3 ^ k1;

            ulong length = 0;

            int bytesRead;
            var buffer = new byte[ulongSize];
            while ((bytesRead = await stream.ReadAsync(buffer, 0, ulongSize).ConfigureAwait(false)) == ulongSize)
            {
                ulong m = BitConverter.ToUInt64(buffer, 0);
                v3 ^= m;
                sipRoundC(ref v0, ref v1, ref v2, ref v3);
                v0 ^= m;
                length += (ulong)bytesRead;
            }

            length += (ulong)bytesRead;
            ulong lastWord = length << 56;

            if (bytesRead > 0)
            {
                lastWord |= Bits.PartialBytesToUInt64(buffer, bytesRead);
            }

            v3 ^= lastWord;
            sipRoundC(ref v0, ref v1, ref v2, ref v3);
            v0 ^= lastWord;

            v2 ^= finalVectorXor;
            sipRoundD(ref v0, ref v1, ref v2, ref v3);
            return v0 ^ v1 ^ v2 ^ v3;
        }

        /// <summary>
        /// Calculate 64-bit SipHash-2-4 algorithm using the given key and the input.
        /// </summary>
        /// <param name="buffer">Input buffer.</param>
        /// <param name="key">16-byte key.</param>
        /// <returns>64-bit hash value.</returns>
        public static unsafe ulong Hash64(ReadOnlySpan<byte> buffer, ReadOnlySpan<byte> key)
        {
            const ulong finalVectorXor = 0xFF;

            if (key.Length != keyLength)
            {
                throw new ArgumentException("Key must be 16-bytes long", nameof(key));
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
            int numUlongs = length / sizeof(ulong);
            int left = length % sizeof(ulong);

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