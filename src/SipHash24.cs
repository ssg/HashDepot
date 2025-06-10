// <copyright file="SipHash24.cs" company="Sedat Kapanoglu">
// Copyright (c) 2015-2025 Sedat Kapanoglu
// MIT License (see LICENSE file for details)
// </copyright>

using System;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace HashDepot;

/// <summary>
/// SipHash 2-4 algorithm, the most common SipHash variant.
/// </summary>
public static partial class SipHash24
{
    const int keyLength = 16;

    const ulong initv0 = 0x736f6d6570736575U;
    const ulong initv1 = 0x646f72616e646f6dU;
    const ulong initv2 = 0x6c7967656e657261U;
    const ulong initv3 = 0x7465646279746573U;

    const ulong finalVectorXor = 0xFF;

    /// <summary>
    /// Calculate 64-bit SipHash-2-4 algorithm using the given key and the input.
    /// </summary>
    /// <param name="stream">Input stream.</param>
    /// <param name="key">16-byte key.</param>
    /// <returns>64-bit hash value.</returns>
    public static ulong Hash64(Stream stream, ReadOnlySpan<byte> key)
    {
        var state = new State64(key);
        const int blockSize = 256;
        Span<byte> buffer = stackalloc byte[blockSize * sizeof(ulong)];
        int bytesRead;
        while ((bytesRead = stream.Read(buffer)) > 0)
        {
            state.Update(buffer[..bytesRead]);
        }

        return state.Result();
    }

    /// <summary>
    /// Calculate 64-bit SipHash-2-4 algorithm using the given key and the input.
    /// </summary>
    /// <param name="stream">Input stream.</param>
    /// <param name="key">16-byte key.</param>
    /// <returns>A Task representing the 64-bit hash computation.</returns>
    public static async Task<ulong> Hash64Async(Stream stream, ReadOnlyMemory<byte> key)
    {
        var state = new State64(key.Span);
        const int blockSize = 256;
        byte[] buffer = new byte[blockSize * sizeof(ulong)]; // 4KB buffers
        int bytesRead;
        while ((bytesRead = await stream.ReadAsync(buffer).ConfigureAwait(false)) > 0)
        {
            state.Update(buffer.AsSpan(0, bytesRead));
        }

        return state.Result();
    }

    /// <summary>
    /// Calculate 64-bit SipHash-2-4 algorithm using the given key and the input.
    /// </summary>
    /// <param name="buffer">Input buffer.</param>
    /// <param name="key">16-byte key.</param>
    /// <returns>64-bit hash value.</returns>
    public static ulong Hash64(ReadOnlySpan<byte> buffer, ReadOnlySpan<byte> key)
    {
        if (key.Length != keyLength)
        {
            throw new ArgumentException("Key must be 16-bytes long", nameof(key));
        }

        ulong k0;
        ulong k1;

        k0 = BitConverter.ToUInt64(key[..8]);
        k1 = BitConverter.ToUInt64(key[8..16]);

        ulong v0 = initv0 ^ k0;
        ulong v1 = initv1 ^ k1;
        ulong v2 = initv2 ^ k0;
        ulong v3 = initv3 ^ k1;

        int length = buffer.Length;
        ulong lastWord = (ulong)length << 56;
        int leftBytes = length % sizeof(ulong);

        int offset = 0;
        int end = length - leftBytes;
        while (offset < end)
        {
            ulong m = BitConverter.ToUInt64(buffer[offset..(offset + sizeof(ulong))]);
            offset += sizeof(ulong);
            v3 ^= m;
            sipRoundC(ref v0, ref v1, ref v2, ref v3);
            v0 ^= m;
        }

        if (leftBytes > 0)
        {
            lastWord |= Bits.PartialBytesToUInt64(buffer[offset..]);
        }

        v3 ^= lastWord;
        sipRoundC(ref v0, ref v1, ref v2, ref v3);
        v0 ^= lastWord;

        v2 ^= finalVectorXor;
        sipRoundD(ref v0, ref v1, ref v2, ref v3);
        return v0 ^ v1 ^ v2 ^ v3;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void sipRoundC(ref ulong v0, ref ulong v1, ref ulong v2, ref ulong v3)
    {
        sipRound(ref v0, ref v1, ref v2, ref v3);
        sipRound(ref v0, ref v1, ref v2, ref v3);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void sipRoundD(ref ulong v0, ref ulong v1, ref ulong v2, ref ulong v3)
    {
        sipRound(ref v0, ref v1, ref v2, ref v3);
        sipRound(ref v0, ref v1, ref v2, ref v3);
        sipRound(ref v0, ref v1, ref v2, ref v3);
        sipRound(ref v0, ref v1, ref v2, ref v3);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void sipRound(ref ulong v0, ref ulong v1, ref ulong v2, ref ulong v3)
    {
        v0 += v1;
        v1 = BitOperations.RotateLeft(v1, 13);
        v1 ^= v0;
        v0 = BitOperations.RotateLeft(v0, 32);

        v2 += v3;
        v3 = BitOperations.RotateLeft(v3, 16);
        v3 ^= v2;

        v2 += v1;
        v1 = BitOperations.RotateLeft(v1, 17);
        v1 ^= v2;
        v2 = BitOperations.RotateLeft(v2, 32);

        v0 += v3;
        v3 = BitOperations.RotateLeft(v3, 21);
        v3 ^= v0;
    }
}