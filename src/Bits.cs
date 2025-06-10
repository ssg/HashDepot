// <copyright file="Bits.cs" company="Sedat Kapanoglu">
// Copyright (c) 2015-2025 Sedat Kapanoglu
// MIT License (see LICENSE file for details)
// </copyright>

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace HashDepot;

/// <summary>
/// Bit operations.
/// </summary>
static class Bits
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ulong PartialBytesToUInt64(ReadOnlySpan<byte> remainingBytes)
    {
        Span<byte> buffer = stackalloc byte[sizeof(ulong)];
        remainingBytes.CopyTo(buffer);
        return BitConverter.ToUInt64(buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint PartialBytesToUInt32(ReadOnlySpan<byte> remainingBytes)
    {
        Span<byte> buffer = stackalloc byte[sizeof(uint)];
        remainingBytes.CopyTo(buffer);
        return BitConverter.ToUInt32(buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ulong XxhXorShift64(ulong v64, int shift)
    {
        Debug.Assert(shift is >= 0 and < 64, "Shift must be between 0 and 63 inclusive.");
        return v64 ^ (v64 >> shift);
    }
}