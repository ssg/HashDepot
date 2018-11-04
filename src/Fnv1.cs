// Copyright (c) 2015, 2016 Sedat Kapanoglu
// MIT License - see LICENSE file for details

using System;

namespace HashDepot
{
    /// <summary>
    /// FNV-1 Hash functions.
    /// </summary>
    public static class Fnv1
    {
        /// <summary>
        /// Calculate 32-bit FNV-1 hash value
        /// </summary>
        public static uint Hash32(byte[] buffer)
        {
            Require.NotNull(buffer, nameof(buffer));

            const uint offsetBasis = 2166136261;
            const uint prime = 16777619;

            uint result = offsetBasis;
            foreach (byte b in buffer)
            {
                result *= prime;
                result ^= b;
            }
            return result;
        }

        /// <summary>
        /// Calculate 64-bit FNV-1 hash value
        /// </summary>
        public static ulong Hash64(byte[] buffer)
        {
            Require.NotNull(buffer, nameof(buffer));

            const ulong offsetBasis = 14695981039346656037;
            const ulong prime = 1099511628211;

            ulong result = offsetBasis;
            foreach (byte b in buffer)
            {
                result *= prime;
                result ^= b;
            }
            return result;
        }
    }
}