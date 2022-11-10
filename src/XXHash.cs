// <copyright file="XXHash.cs" company="Sedat Kapanoglu">
// Copyright (c) 2015-2022 Sedat Kapanoglu
// MIT License (see LICENSE file for details)
// </copyright>

using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace HashDepot;

/// <summary>
/// XXHash implementation.
/// </summary>
public static class XXHash
{
    private const ulong prime64v1 = 11400714785074694791ul;
    private const ulong prime64v2 = 14029467366897019727ul;
    private const ulong prime64v3 = 1609587929392839161ul;
    private const ulong prime64v4 = 9650029242287828579ul;
    private const ulong prime64v5 = 2870177450012600261ul;

    private const uint prime32v1 = 2654435761u;
    private const uint prime32v2 = 2246822519u;
    private const uint prime32v3 = 3266489917u;
    private const uint prime32v4 = 668265263u;
    private const uint prime32v5 = 374761393u;

    /// <summary>
    /// Generate a 32-bit xxHash value.
    /// </summary>
    /// <param name="buffer">Input buffer.</param>
    /// <param name="seed">Optional seed.</param>
    /// <returns>32-bit hash value.</returns>
    public static uint Hash32(ReadOnlySpan<byte> buffer, uint seed = 0)
    {
        const int stripeLength = 16;

        int len = buffer.Length;
        int evenLength = len - (len % stripeLength);
        int offset = 0;
        uint acc;

        if (len < stripeLength)
        {
            acc = seed + prime32v5;
            goto Exit;
        }

        var (acc1, acc2, acc3, acc4) = initAccumulators32(seed);
        do
        {
            int end = offset + stripeLength;
            acc = processStripe32(buffer[offset..end], ref acc1, ref acc2, ref acc3, ref acc4);
            offset = end;
        }
        while (offset < evenLength);

    Exit:
        acc += (uint)len;
        acc = processRemaining32(buffer[offset..], acc);

        return avalanche32(acc);
    }

    /// <summary>
    /// Generate a 32-bit xxHash value from a stream.
    /// </summary>
    /// <param name="stream">Input stream.</param>
    /// <param name="seed">Optional seed.</param>
    /// <returns>32-bit hash value.</returns>
    public static uint Hash32(Stream stream, uint seed = 0)
    {
        const int stripeLength = 16;
        const int readBufferSize = stripeLength * 1024; // 16kb read buffer - has to be stripe aligned

        var buffer = new byte[readBufferSize].AsSpan();
        uint acc;

        int readBytes = stream.Read(buffer);
        int len = readBytes;

        int offset = 0;
        if (readBytes < stripeLength)
        {
            acc = seed + prime32v5;
            goto Exit;
        }

        var (acc1, acc2, acc3, acc4) = initAccumulators32(seed);
        do
        {
            do
            {
                int end = offset + stripeLength;
                acc = processStripe32(
                    buffer[offset..end],
                    ref acc1,
                    ref acc2,
                    ref acc3,
                    ref acc4);
                offset = end;
                readBytes -= stripeLength;
            }
            while (readBytes >= stripeLength);

            // read more if the alignment is still intact
            if (readBytes == 0)
            {
                readBytes = stream.Read(buffer);
                offset = 0;
                len += readBytes;
            }
        }
        while (readBytes >= stripeLength);

    Exit:
        acc += (uint)len;
        acc = processRemaining32(buffer[offset..(offset + readBytes)], acc);

        return avalanche32(acc);
    }

    /// <summary>
    /// Generate a 64-bit xxHash value.
    /// </summary>
    /// <param name="buffer">Input buffer.</param>
    /// <param name="seed">Optional seed.</param>
    /// <returns>Computed 64-bit hash value.</returns>
    public static ulong Hash64(ReadOnlySpan<byte> buffer, ulong seed = 0)
    {
        const int stripeLength = 32;

        int evenLength = buffer.Length - (buffer.Length % stripeLength);
        ulong acc;

        int offset = 0;
        if (buffer.Length < stripeLength)
        {
            acc = seed + prime64v5;
            goto Exit;
        }

        var (acc1, acc2, acc3, acc4) = initAccumulators64(seed);
        do
        {
            int end = offset + stripeLength;
            acc = processStripe64(buffer[offset..end], ref acc1, ref acc2, ref acc3, ref acc4);
            offset = end;
        }
        while (offset < evenLength);

    Exit:
        acc += (ulong)buffer.Length;
        acc = processRemaining64(buffer[offset..], acc);
        return avalanche64(acc);
    }

    /// <summary>
    /// Generate a 64-bit xxHash value from a stream.
    /// </summary>
    /// <param name="stream">Input stream.</param>
    /// <param name="seed">Optional seed.</param>
    /// <returns>Computed 64-bit hash value.</returns>
    public static ulong Hash64(Stream stream, ulong seed = 0)
    {
        const int stripeLength = 32;
        const int readBufferSize = stripeLength * 1024; // 32kb buffer length

        ulong acc;

        var buffer = new byte[readBufferSize].AsSpan();
        int readBytes = stream.Read(buffer);
        ulong len = (ulong)readBytes;

        int offset = 0;
        if (readBytes < stripeLength)
        {
            acc = seed + prime64v5;
            goto Exit;
        }

        var (acc1, acc2, acc3, acc4) = initAccumulators64(seed);
        do
        {
            do
            {
                int end = offset + stripeLength;
                acc = processStripe64(
                    buffer[offset..end],
                    ref acc1,
                    ref acc2,
                    ref acc3,
                    ref acc4);
                offset = end;
                readBytes -= stripeLength;
            }
            while (readBytes >= stripeLength);

            // read more if the alignment is intact
            if (readBytes == 0)
            {
                readBytes = stream.Read(buffer);
                offset = 0;
                len += (ulong)readBytes;
            }
        }
        while (readBytes >= stripeLength);

    Exit:
        acc += len;
        acc = processRemaining64(buffer[offset..(offset + readBytes)], acc);

        return avalanche64(acc);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (ulong, ulong, ulong, ulong) initAccumulators64(ulong seed)
    {
        return (seed + prime64v1 + prime64v2, seed + prime64v2, seed, seed - prime64v1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong processStripe64(
        ReadOnlySpan<byte> buf,
        ref ulong acc1,
        ref ulong acc2,
        ref ulong acc3,
        ref ulong acc4)
    {
        processLane64(ref acc1, buf[0..8]);
        processLane64(ref acc2, buf[8..16]);
        processLane64(ref acc3, buf[16..24]);
        processLane64(ref acc4, buf[24..32]);

        ulong acc = Bits.RotateLeft(acc1, 1)
                  + Bits.RotateLeft(acc2, 7)
                  + Bits.RotateLeft(acc3, 12)
                  + Bits.RotateLeft(acc4, 18);

        mergeAccumulator64(ref acc, acc1);
        mergeAccumulator64(ref acc, acc2);
        mergeAccumulator64(ref acc, acc3);
        mergeAccumulator64(ref acc, acc4);
        return acc;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void processLane64(ref ulong accn, ReadOnlySpan<byte> buf)
    {
        ulong lane = Bits.ToUInt64(buf);
        accn = round64(accn, lane);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong processRemaining64(
        ReadOnlySpan<byte> remaining,
        ulong acc)
    {
        int remainingLen = remaining.Length;
        int i = 0;
        for (ulong lane; remainingLen >= 8; remainingLen -= 8, i += 8)
        {
            lane = Bits.ToUInt64(remaining[i..(i + 8)]);
            acc ^= round64(0, lane);
            acc = Bits.RotateLeft(acc, 27) * prime64v1;
            acc += prime64v4;
        }

        for (uint lane32; remainingLen >= 4; remainingLen -= 4, i += 4)
        {
            lane32 = Bits.ToUInt32(remaining[i..(i + 4)]);
            acc ^= lane32 * prime64v1;
            acc = Bits.RotateLeft(acc, 23) * prime64v2;
            acc += prime64v3;
        }

        for (byte lane8; remainingLen >= 1; remainingLen--, i++)
        {
            lane8 = remaining[i];
            acc ^= lane8 * prime64v5;
            acc = Bits.RotateLeft(acc, 11) * prime64v1;
        }

        return acc;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong avalanche64(ulong acc)
    {
        acc ^= acc >> 33;
        acc *= prime64v2;
        acc ^= acc >> 29;
        acc *= prime64v3;
        acc ^= acc >> 32;
        return acc;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong round64(ulong accn, ulong lane)
    {
        accn += lane * prime64v2;
        return Bits.RotateLeft(accn, 31) * prime64v1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void mergeAccumulator64(ref ulong acc, ulong accn)
    {
        acc ^= round64(0, accn);
        acc *= prime64v1;
        acc += prime64v4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (uint, uint, uint, uint) initAccumulators32(
        uint seed)
    {
        return (seed + prime32v1 + prime32v2, seed + prime32v2, seed, seed - prime32v1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint processStripe32(
        ReadOnlySpan<byte> buf,
        ref uint acc1,
        ref uint acc2,
        ref uint acc3,
        ref uint acc4)
    {
        processLane32(buf[0..4], ref acc1);
        processLane32(buf[4..8], ref acc2);
        processLane32(buf[8..12], ref acc3);
        processLane32(buf[12..16], ref acc4);

        return Bits.RotateLeft(acc1, 1)
             + Bits.RotateLeft(acc2, 7)
             + Bits.RotateLeft(acc3, 12)
             + Bits.RotateLeft(acc4, 18);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void processLane32(ReadOnlySpan<byte> buf, ref uint accn)
    {
        uint lane = Bits.ToUInt32(buf);
        accn = round32(accn, lane);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint processRemaining32(
        ReadOnlySpan<byte> remaining,
        uint acc)
    {
        int i = 0;
        int remainingLen = remaining.Length;
        for (uint lane; remainingLen >= 4; remainingLen -= 4, i += 4)
        {
            lane = Bits.ToUInt32(remaining[i..]);
            acc += lane * prime32v3;
            acc = Bits.RotateLeft(acc, 17) * prime32v4;
        }

        for (byte lane; remainingLen >= 1; remainingLen--, i++)
        {
            lane = remaining[i];
            acc += lane * prime32v5;
            acc = Bits.RotateLeft(acc, 11) * prime32v1;
        }

        return acc;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint round32(uint accn, uint lane)
    {
        accn += lane * prime32v2;
        accn = Bits.RotateLeft(accn, 13);
        accn *= prime32v1;
        return accn;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint avalanche32(uint acc)
    {
        acc ^= acc >> 15;
        acc *= prime32v2;
        acc ^= acc >> 13;
        acc *= prime32v3;
        acc ^= acc >> 16;
        return acc;
    }
}
