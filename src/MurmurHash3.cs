// <copyright file="MurmurHash3.cs" company="Sedat Kapanoglu">
// Copyright (c) 2015-2025 Sedat Kapanoglu
// MIT License (see LICENSE file for details)
// </copyright>

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace HashDepot;

/// <summary>
/// x86 flavors of MurmurHash3 algorithms.
/// </summary>
public static class MurmurHash3
{
    /// <summary>
    /// Calculate 32-bit MurmurHash3 hash value using x86 version of the algorithm.
    /// </summary>
    /// <param name="stream">Input stream.</param>
    /// <param name="seed">Seed value.</param>
    /// <returns>Hash value.</returns>
    public static uint Hash32(Stream stream, uint seed)
    {
        const int uintSize = sizeof(uint);
        const uint final1 = 0x85ebca6b;
        const uint final2 = 0xc2b2ae35;
        const uint n = 0xe6546b64;
        const uint m = 5;

        uint hash = seed;
        var buffer = new byte[uintSize].AsSpan();
        uint length = 0;
        int bytesRead;
        while ((bytesRead = stream.Read(buffer)) == uintSize)
        {
            uint k = Bits.ToUInt32(buffer);
            round32(ref k, ref hash);
            hash = Bits.RotateLeft(hash, 13);
            hash *= m;
            hash += n;
            length += (uint)bytesRead;
        }

        // process remaning bytes
        if (bytesRead > 0)
        {
            uint remaining = Bits.PartialBytesToUInt32(buffer[..bytesRead]);
            round32(ref remaining, ref hash);
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
    /// <param name="stream">Input stream.</param>
    /// <param name="seed">Seed value.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous hash operation.</returns>
    public static async Task<uint> Hash32Async(Stream stream, uint seed)
    {
        const int uintSize = sizeof(uint);
        const uint final1 = 0x85ebca6b;
        const uint final2 = 0xc2b2ae35;
        const uint n = 0xe6546b64;
        const uint m = 5;

        uint hash = seed;
        var buffer = new byte[uintSize].AsMemory();
        uint length = 0;
        int bytesRead;
        while ((bytesRead = await stream.ReadAsync(buffer).ConfigureAwait(false)) == uintSize)
        {
            uint k = Bits.ToUInt32(buffer.Span);
            round32(ref k, ref hash);
            hash = Bits.RotateLeft(hash, 13);
            hash *= m;
            hash += n;
            length += (uint)bytesRead;
        }

        // process remaning bytes
        if (bytesRead > 0)
        {
            uint remaining = Bits.PartialBytesToUInt32(buffer[..bytesRead].Span);
            round32(ref remaining, ref hash);
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
    /// Calculate 32-bit MurmurHash3 hash value.
    /// </summary>
    /// <param name="buffer">Input buffer.</param>
    /// <param name="seed">Seed value.</param>
    /// <returns>Hash value.</returns>
    public static uint Hash32(ReadOnlySpan<byte> buffer, uint seed)
    {
        const int uintSize = sizeof(uint);
        const uint final1 = 0x85ebca6b;
        const uint final2 = 0xc2b2ae35;
        const uint n = 0xe6546b64;
        const uint m = 5;

        uint hash = seed;
        int length = buffer.Length;
        var (numUInts, leftBytes) = Math.DivRem(length, uintSize);
        int i = 0;
        for (; i < numUInts * sizeof(uint); i += sizeof(uint))
        {
            uint k = Bits.ToUInt32(buffer[i..(i + sizeof(uint))]);
            round32(ref k, ref hash);
            hash = Bits.RotateLeft(hash, 13);
            hash *= m;
            hash += n;
        }

        if (leftBytes > 0)
        {
            uint remaining = Bits.PartialBytesToUInt32(buffer[i..(i + leftBytes)]);
            round32(ref remaining, ref hash);
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

    /// <summary>
    /// Calculate 128-bit MurmurHash3 hash value using 64-bit version of the algorithm.
    /// </summary>
    /// <param name="stream">Input stream.</param>
    /// <param name="seed">Seed value.</param>
    /// <returns>128-bit hash value in a Span.</returns>
    public static byte[] Hash128(Stream stream, uint seed)
    {
        const int ulongSize = sizeof(ulong);
        const int blockSize = ulongSize * 2;

        const ulong c1 = 0x87c37b91114253d5UL;
        const ulong c2 = 0x4cf5ad432745937fUL;

        ulong h1 = seed;
        ulong h2 = seed;
        var buffer = new byte[blockSize].AsSpan();
        int outputLength = 0;
        int readBytes;
        while ((readBytes = stream.Read(buffer)) == blockSize)
        {
            ulong ik1 = Bits.ToUInt64(buffer);
            ulong ik2 = Bits.ToUInt64(buffer[ulongSize..]);

            round128(ref ik1, ref h1, c1, c2, h2, 31, 27, 0x52dce729U);
            round128(ref ik2, ref h2, c2, c1, h1, 33, 31, 0x38495ab5U);

            outputLength += blockSize;
        }

        ulong k1;
        ulong k2;

        if (readBytes > ulongSize)
        {
            k2 = Bits.PartialBytesToUInt64(buffer[ulongSize..]);
            tailRound128(ref k2, ref h2, c2, c1, 33);
            outputLength += readBytes - ulongSize;
            readBytes = ulongSize;
        }

        if (readBytes > 0)
        {
            k1 = Bits.PartialBytesToUInt64(buffer[..readBytes]);
            tailRound128(ref k1, ref h1, c1, c2, 31);
            outputLength += readBytes;
        }

        h1 ^= (ulong)outputLength;
        h2 ^= (ulong)outputLength;

        h1 += h2;
        h2 += h1;

        fmix64(ref h1);
        fmix64(ref h2);

        h1 += h2;
        h2 += h1;

        return makeBytes(h1, h2);
    }

    /// <summary>
    /// Calculate 128-bit MurmurHash3 hash value using x64 version of the algorithm.
    /// </summary>
    /// <param name="buffer">Input buffer.</param>
    /// <param name="seed">Seed value.</param>
    /// <returns>128-bit hash value as a Span of bytes.</returns>
    public static byte[] Hash128(ReadOnlySpan<byte> buffer, uint seed)
    {
        const int blockSize = 16;
        const ulong c1 = 0x87c37b91114253d5UL;
        const ulong c2 = 0x4cf5ad432745937fUL;

        ulong h1 = seed;
        ulong h2 = seed;

        int length = buffer.Length;
        var (numBlocks, leftBytes) = Math.DivRem(length, blockSize);

        int offset = 0;
        int end = numBlocks * sizeof(ulong) * 2;
        while (offset != end)
        {
            ulong ik1 = Bits.ToUInt64(buffer[offset..(offset + sizeof(ulong))]);
            offset += sizeof(ulong);
            ulong ik2 = Bits.ToUInt64(buffer[offset..(offset + sizeof(ulong))]);
            offset += sizeof(ulong);

            round128(ref ik1, ref h1, c1, c2, h2, 31, 27, 0x52dce729U);
            round128(ref ik2, ref h2, c2, c1, h1, 33, 31, 0x38495ab5U);
        }

        int tail = offset;
        ulong k1;
        ulong k2;

        if (leftBytes > sizeof(ulong))
        {
            offset += sizeof(ulong);
            k2 = Bits.PartialBytesToUInt64(buffer[offset..]);
            tailRound128(ref k2, ref h2, c2, c1, 33);
            leftBytes = sizeof(ulong);
        }

        if (leftBytes > 0)
        {
            k1 = Bits.PartialBytesToUInt64(buffer[tail..(tail + leftBytes)]);
            tailRound128(ref k1, ref h1, c1, c2, 31);
        }

        h1 ^= (ulong)length;
        h2 ^= (ulong)length;

        h1 += h2;
        h2 += h1;

        fmix64(ref h1);
        fmix64(ref h2);

        h1 += h2;
        h2 += h1;

        return makeBytes(h1, h2);
    }

    static byte[] makeBytes(ulong h1, ulong h2)
    {
        var result = new byte[16];
        _ = BitConverter.TryWriteBytes(result, h1);
        _ = BitConverter.TryWriteBytes(result.AsSpan()[8..], h2);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void round32(ref uint value, ref uint hash)
    {
        const uint c1 = 0xcc9e2d51;
        const uint c2 = 0x1b873593;

        value *= c1;
        value = Bits.RotateLeft(value, 15);
        value *= c2;
        hash ^= value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void round128(
        ref ulong k,
        ref ulong h,
        ulong c1,
        ulong c2,
        ulong hn,
        int krot,
        int hrot,
        uint x)
    {
        k *= c1;
        k = Bits.RotateLeft(k, krot);
        k *= c2;
        h ^= k;
        h = Bits.RotateLeft(h, hrot);
        h += hn;
        h = (h * 5) + x;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void fmix64(ref ulong h)
    {
        h ^= h >> 33;
        h *= 0xff51afd7ed558ccdUL;
        h ^= h >> 33;
        h *= 0xc4ceb9fe1a85ec53UL;
        h ^= h >> 33;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void tailRound128(ref ulong k, ref ulong h, ulong c1, ulong c2, int rot)
    {
        k *= c1;
        k = Bits.RotateLeft(k, rot);
        k *= c2;
        h ^= k;
    }
}