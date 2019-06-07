// <copyright file="XXHash.cs" company="Sedat Kapanoglu">
// Copyright (c) 2015-2019 Sedat Kapanoglu
// MIT License (see LICENSE file for details)
// </copyright>

namespace HashDepot
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// XXHash implementation.
    /// </summary>
    public static class XXHash
    {
        private const ulong prime641 = 11400714785074694791ul;
        private const ulong prime642 = 14029467366897019727ul;
        private const ulong prime643 = 1609587929392839161ul;
        private const ulong prime644 = 9650029242287828579ul;
        private const ulong prime645 = 2870177450012600261ul;

        /// <summary>
        /// Generate a 32-bit xxHash value.
        /// </summary>
        /// <param name="buffer">Input buffer.</param>
        /// <param name="seed">Optional seed.</param>
        /// <returns>32-bit hash value.</returns>
        public static unsafe uint Hash32(ReadOnlySpan<byte> buffer, uint seed = 0)
        {
            const uint prime1 = 2654435761u;
            const uint prime2 = 2246822519u;
            const uint prime3 = 3266489917u;
            const uint prime4 = 668265263u;
            const uint prime5 = 374761393u;

            const int stripeLength = 16;
            const int laneLength = 4;

            int len = buffer.Length;
            int remainingLen = len;
            uint acc;

            fixed (byte* inputPtr = buffer)
            {
                byte* pInput = inputPtr;

                if (len < stripeLength)
                {
                    acc = seed + prime5;
                    goto Skip;
                }

                uint acc1 = seed + prime1 + prime2;
                uint acc2 = seed + prime2;
                uint acc3 = seed;
                uint acc4 = seed - prime1;

                do
                {
                    void processLane(ref uint accn)
                    {
                        uint lane = *(uint*)pInput;
                        accn += lane * prime2;
                        accn = Bits.RotateLeft(accn, 13);
                        accn *= prime1;
                        pInput += laneLength;
                    }

                    processLane(ref acc1);
                    processLane(ref acc2);
                    processLane(ref acc3);
                    processLane(ref acc4);

                    acc = Bits.RotateLeft(acc1, 1)
                        + Bits.RotateLeft(acc2, 7)
                        + Bits.RotateLeft(acc3, 12)
                        + Bits.RotateLeft(acc4, 18);
                    remainingLen -= stripeLength;
                }
                while (remainingLen >= stripeLength);

            Skip:
                acc += (uint)len;

                for (uint lane; remainingLen >= laneLength; remainingLen -= laneLength, pInput += laneLength)
                {
                    lane = *(uint*)pInput;
                    acc += lane * prime3;
                    acc = Bits.RotateLeft(acc, 11) * prime4;
                }

                for (byte lane; remainingLen >= 1; remainingLen--, pInput++)
                {
                    lane = *pInput;
                    acc += lane * prime5;
                    acc = Bits.RotateLeft(acc, 11) * prime1;
                }

                acc ^= acc >> 15;
                acc *= prime2;
                acc ^= acc >> 13;
                acc *= prime3;
                acc ^= acc >> 16;

                return acc;
            }
        }

        /// <summary>
        /// Generate a 64-bit xxHash value.
        /// </summary>
        /// <param name="buffer">Input buffer.</param>
        /// <param name="seed">Optional seed.</param>
        /// <returns>Computed 64-bit hash value.</returns>
        public static unsafe ulong Hash64(ReadOnlySpan<byte> buffer, ulong seed = 0)
        {
            const int stripeLength = 32;
            const int laneLength = 8;

            int len = buffer.Length;
            int remainingLen = len;
            ulong acc;

            fixed (byte* inputPtr = buffer)
            {
                byte* pInput = inputPtr;

                if (len < stripeLength)
                {
                    acc = seed + prime645;
                    goto Skip;
                }

                ulong acc1 = seed + prime641 + prime642;
                ulong acc2 = seed + prime642;
                ulong acc3 = seed;
                ulong acc4 = seed - prime641;

                do
                {
                    void processLane(ref ulong accn)
                    {
                        ulong lane = *(ulong*)pInput;
                        accn = round64(accn, lane);
                        pInput += laneLength;
                    }

                    processLane(ref acc1);
                    processLane(ref acc2);
                    processLane(ref acc3);
                    processLane(ref acc4);

                    acc = Bits.RotateLeft(acc1, 1)
                        + Bits.RotateLeft(acc2, 7)
                        + Bits.RotateLeft(acc3, 12)
                        + Bits.RotateLeft(acc4, 18);

                    mergeAccumulator64(ref acc, acc1);
                    mergeAccumulator64(ref acc, acc2);
                    mergeAccumulator64(ref acc, acc3);
                    mergeAccumulator64(ref acc, acc4);

                    remainingLen -= stripeLength;
                }
                while (remainingLen >= stripeLength);

            Skip:
                acc += (ulong)len;

                for (ulong lane; remainingLen >= laneLength; remainingLen -= laneLength, pInput += laneLength)
                {
                    lane = *(ulong*)pInput;
                    acc ^= round64(0, lane);
                    acc = Bits.RotateLeft(acc, 27) * prime641;
                    acc += prime644;
                }

                for (uint lane; remainingLen >= 4; remainingLen -= 4, pInput += 4)
                {
                    lane = *(uint*)pInput;
                    acc ^= lane * prime641;
                    acc = Bits.RotateLeft(acc, 23) * prime642;
                    acc += prime643;
                }

                for (byte lane; remainingLen >= 1; remainingLen--, pInput++)
                {
                    lane = *pInput;
                    acc ^= lane * prime645;
                    acc = Bits.RotateLeft(acc, 11) * prime641;
                }

                acc ^= acc >> 33;
                acc *= prime642;
                acc ^= acc >> 29;
                acc *= prime643;
                acc ^= acc >> 32;

                return acc;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong round64(ulong accn, ulong lane)
        {
            accn += lane * prime642;
            return Bits.RotateLeft(accn, 31);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void mergeAccumulator64(ref ulong acc, ulong accn)
        {
            acc ^= round64(0, accn);
            acc *= prime641;
            acc += prime644;
        }
    }
}
