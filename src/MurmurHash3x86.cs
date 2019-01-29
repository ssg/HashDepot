// <copyright file="MurmurHash3x86.cs" company="Sedat Kapanoglu">
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
    /// x86 flavors of MurmurHash3 algorithms
    /// </summary>
    public static class MurmurHash3x86
    {
        /// <summary>
        /// Calculate 32-bit MurmurHash3 hash value
        /// </summary>
        /// <param name="buffer">Input buffer</param>
        /// <param name="seed">Seed value</param>
        /// <returns>Hash value</returns>
        public static uint Hash32(byte[] buffer, uint seed)
        {
            return Hash32(buffer.AsSpan(), seed);
        }

        /// <summary>
        /// Calculate 32-bit MurmurHash3 hash value using x86 version of the algorithm.
        /// </summary>
        /// <param name="stream">Input stream</param>
        /// <param name="seed">Seed value</param>
        /// <returns>Hash value</returns>
        public static unsafe uint Hash32(Stream stream, uint seed)
        {
            const int uintSize = sizeof(uint);
            const uint final1 = 0x85ebca6b;
            const uint final2 = 0xc2b2ae35;
            const uint n = 0xe6546b64;
            const uint m = 5;

            uint hash = seed;
            var buffer = new byte[uintSize];
            uint length = 0;
            int bytesRead;
            while ((bytesRead = stream.Read(buffer, 0, uintSize)) == uintSize)
            {
                uint k = BitConverter.ToUInt32(buffer, 0);
                round(ref k, ref hash);
                hash = Bits.RotateLeft(hash, 13);
                hash *= m;
                hash += n;
                length += (uint)bytesRead;
            }

            // process remaning bytes
            if (bytesRead > 0)
            {
                fixed (byte* bufPtr = buffer)
                {
                    uint remaining = Bits.PartialBytesToUInt32(bufPtr, bytesRead);
                    round(ref remaining, ref hash);
                }

                length += (uint)bytesRead;
            }

            hash ^= length;

            // finalization mix
            hash ^= hash >> 16;
            hash *= final1;
            hash ^= hash >> 13;
            hash *= final2;
            hash ^= hash >> 16;
            return hash;
        }

        /// <summary>
        /// Calculate 32-bit MurmurHash3 hash value using x86 version of the algorithm.
        /// </summary>
        /// <param name="stream">Input stream</param>
        /// <param name="seed">Seed value</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous hash operation.</returns>
        public static async Task<uint> Hash32Async(Stream stream, uint seed)
        {
            const int uintSize = sizeof(uint);
            const uint final1 = 0x85ebca6b;
            const uint final2 = 0xc2b2ae35;
            const uint n = 0xe6546b64;
            const uint m = 5;

            uint hash = seed;
            var buffer = new byte[uintSize];
            uint length = 0;
            int bytesRead;
            while ((bytesRead = await stream.ReadAsync(buffer, 0, uintSize).ConfigureAwait(false)) == uintSize)
            {
                uint k = BitConverter.ToUInt32(buffer, 0);
                round(ref k, ref hash);
                hash = Bits.RotateLeft(hash, 13);
                hash *= m;
                hash += n;
                length += (uint)bytesRead;
            }

            // process remaning bytes
            if (bytesRead > 0)
            {
                uint remaining = Bits.PartialBytesToUInt32(buffer, bytesRead);
                round(ref remaining, ref hash);
                length += (uint)bytesRead;
            }

            hash ^= length;

            // finalization mix
            hash ^= hash >> 16;
            hash *= final1;
            hash ^= hash >> 13;
            hash *= final2;
            hash ^= hash >> 16;
            return hash;
        }

        /// <summary>
        /// Calculate 32-bit MurmurHash3 hash value
        /// </summary>
        /// <param name="buffer">Input buffer</param>
        /// <param name="seed">Seed value</param>
        /// <returns>Hash value</returns>
        public static unsafe uint Hash32(ReadOnlySpan<byte> buffer, uint seed)
        {
            const int uintSize = sizeof(uint);
            const uint final1 = 0x85ebca6b;
            const uint final2 = 0xc2b2ae35;
            const uint n = 0xe6546b64;
            const uint m = 5;

            uint hash = seed;
            int length = buffer.Length;
            int numUInts = Math.DivRem(length, uintSize, out int leftBytes);
            fixed (byte* bufPtr = buffer)
            {
                uint* pInput = (uint*)bufPtr;
                for (uint* pEnd = pInput + numUInts; pInput != pEnd; pInput++)
                {
                    uint k = *pInput;
                    round(ref k, ref hash);
                    hash = Bits.RotateLeft(hash, 13);
                    hash *= m;
                    hash += n;
                }

                if (leftBytes > 0)
                {
                    uint remaining = Bits.PartialBytesToUInt32((byte*)pInput, leftBytes);
                    round(ref remaining, ref hash);
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
        private static void round(ref uint value, ref uint hash)
        {
            const uint c1 = 0xcc9e2d51;
            const uint c2 = 0x1b873593;

            value *= c1;
            value = Bits.RotateLeft(value, 15);
            value *= c2;
            hash ^= value;
        }
    }
}