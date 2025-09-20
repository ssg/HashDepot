// <copyright file="XXHash.State32.cs" company="Sedat Kapanoglu">
// Copyright (c) 2015-2025 Sedat Kapanoglu
// MIT License (see LICENSE file for details)
// </copyright>

using System;

namespace HashDepot;

/// <summary>
/// State machine for XXHash 32-bit algorithm's streaming functions.
/// </summary>
public static partial class XXHash
{
    internal class State32
    {
        public const int StripeLength = 16;
        readonly uint seed;
        uint acc;
        uint acc1;
        uint acc2;
        uint acc3;
        uint acc4;
        int len;
        bool finalBlockProcessed;

        public State32(uint seed)
        {
            this.seed = seed;
            (acc1, acc2, acc3, acc4) = initAccumulators32(seed);
        }

        public void Update(ReadOnlySpan<byte> buffer)
        {
            if (finalBlockProcessed)
            {
                throw new InvalidOperationException("Update() called after final stripe has been processed.");
            }

            int bufferSize = buffer.Length;

            // first call?
            if (len == 0 && bufferSize < StripeLength)
            {
                acc = seed + prime32v5;
                len += bufferSize;
                acc += (uint)len;
                acc = processRemaining32(buffer, acc);
                finalBlockProcessed = true;
                return;
            }

            int offset = 0;
            len += bufferSize;

            for (; bufferSize >= StripeLength; bufferSize -= StripeLength)
            {
                int end = offset + StripeLength;
                acc = processStripe32(
                    buffer[offset..end],
                    ref acc1,
                    ref acc2,
                    ref acc3,
                    ref acc4);
                offset = end;
            }

            // process the final incomplete stripe
            if (bufferSize > 0)
            {
                acc += (uint)len;
                acc = processRemaining32(buffer[offset..(offset + bufferSize)], acc);
                finalBlockProcessed = true;
            }
        }

        public uint Result()
        {
            if (len == 0)
            {
                acc = seed + prime32v5;
            }

            if (!finalBlockProcessed)
            {
                acc += (uint)len;
                finalBlockProcessed = true;
            }

            return avalanche32(acc);
        }
    }
}
