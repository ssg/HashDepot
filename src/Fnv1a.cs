// Copyright (c) 2015, 2016 Sedat Kapanoglu
// MIT License - see LICENSE file for details

using System;

namespace HashDepot
{
    /// <summary>
    /// FNV-1a Hash functions
    /// </summary>
    public static class Fnv1a
    {
        /// <summary>
        /// Calculate 32-bit FNV-1a hash value
        /// </summary>
        public static uint Hash32(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            const uint offsetBasis32 = 2166136261;
            const uint prime32 = 16777619;

            uint result = offsetBasis32;
            unchecked
            {
                foreach (byte b in buffer)
                {
                    result ^= b;
                    result *= prime32;
                }
            }
            return result;
        }

        /// <summary>
        /// Calculate 64-bit FNV-1a hash value
        /// </summary>
        public static ulong Hash64(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            const ulong offsetBasis64 = 14695981039346656037;
            const ulong prime64 = 1099511628211;

            ulong result = offsetBasis64;
            unchecked
            {
                foreach (byte b in buffer)
                {
                    result ^= b;
                    result *= prime64;
                }
            }
            return result;
        }
    }
}