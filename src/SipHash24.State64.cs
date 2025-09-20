// <copyright file="SipHash24.State64.cs" company="Sedat Kapanoglu">
// Copyright (c) 2015-2025 Sedat Kapanoglu
// MIT License (see LICENSE file for details)
// </copyright>

using System;

namespace HashDepot;

/// <summary>
/// State machine for SipHash 2-4 algorithm's 64-bit variant.
/// </summary>
static partial class SipHash24
{
    /// <summary>
    /// State machine for streaming functions of 64-bit SipHash 2-4.
    /// </summary>
    internal class State64
    {
        readonly ulong k0;
        readonly ulong k1;

        ulong v0;
        ulong v1;
        ulong v2;
        ulong v3;

        ulong length;
        ulong lastWord;

        bool finalBlockProcessed;

        /// <summary>
        /// Initializes a new instance of the <see cref="State64"/> class.
        /// </summary>
        /// <param name="key">Key to use.</param>
        /// <exception cref="ArgumentException">In case key is not 16-bytes long.</exception>
        public State64(ReadOnlySpan<byte> key)
        {
            if (key.Length != keyLength)
            {
                throw new ArgumentException("Key must be 16-bytes long", nameof(key));
            }

            k0 = BitConverter.ToUInt64(key);
            k1 = BitConverter.ToUInt64(key[sizeof(ulong)..]);

            v0 = initv0 ^ k0;
            v1 = initv1 ^ k1;
            v2 = initv2 ^ k0;
            v3 = initv3 ^ k1;
        }

        /// <summary>
        /// Update the state machine with a given buffer. The buffer must be
        /// the multiple of 8 bytes (64 bits).
        /// </summary>
        /// <param name="buffer">Buffer to use.</param>
        public void Update(ReadOnlySpan<byte> buffer)
        {
            if (finalBlockProcessed)
            {
                throw new InvalidOperationException("Update() called after final block has been processed.");
            }

            int end = buffer.Length - (buffer.Length % sizeof(ulong));
            int n;
            for (n = 0; n < end; n += sizeof(ulong))
            {
                ulong m = BitConverter.ToUInt64(buffer[n..]);
                v3 ^= m;
                sipRoundC(ref v0, ref v1, ref v2, ref v3);
                v0 ^= m;
                length += sizeof(ulong);
            }

            var remainingBlock = buffer[n..];
            if (remainingBlock.Length > 0)
            {
                length += (ulong)remainingBlock.Length;
                lastWord = Bits.PartialBytesToUInt64(remainingBlock);
                finalBlockProcessed = true;
            }
        }

        public ulong Result()
        {
            lastWord |= length << 56;

            v3 ^= lastWord;
            sipRoundC(ref v0, ref v1, ref v2, ref v3);
            v0 ^= lastWord;

            v2 ^= finalVectorXor;
            sipRoundD(ref v0, ref v1, ref v2, ref v3);

            finalBlockProcessed = true;

            return v0 ^ v1 ^ v2 ^ v3;
        }
    }
}