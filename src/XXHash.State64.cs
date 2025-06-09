// <copyright file="XXHash.State64.cs" company="Sedat Kapanoglu">
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
    class State64
    {
        public const int StripeLength = 32;
        readonly ulong seed;
        ulong acc;
        ulong acc1;
        ulong acc2;
        ulong acc3;
        ulong acc4;
        int len;
        bool finalBlockProcessed;

        public State64(ulong seed)
        {
            this.seed = seed;
            (acc1, acc2, acc3, acc4) = initAccumulators64(seed);
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
                acc = seed + prime64v5;
                len += bufferSize;
                acc += (uint)len;
                acc = processRemaining64(buffer, acc);
                finalBlockProcessed = true;
                return;
            }

            int offset = 0;
            len += bufferSize;

            for (; bufferSize >= StripeLength; bufferSize -= StripeLength)
            {
                int end = offset + StripeLength;
                acc = processStripe64(
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
                acc = processRemaining64(buffer[offset..(offset + bufferSize)], acc);
                finalBlockProcessed = true;
            }
        }

        public ulong Result()
        {
            if (len == 0)
            {
                acc = seed + prime64v5;
            }

            if (!finalBlockProcessed)
            {
                acc += (uint)len;
                finalBlockProcessed = true;
            }

            return avalanche64(acc);
        }
    }
}
