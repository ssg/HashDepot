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
        Debug.Assert(remainingBytes.Length <= sizeof(ulong), "remainingBytes length must be less than or equal to 8");

        ulong value = 0;
        for (int i = 0; i < remainingBytes.Length; i++)
        {
            value |= (ulong)remainingBytes[i] << (i * 8);
        }

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint PartialBytesToUInt32(ReadOnlySpan<byte> remainingBytes)
    {
#pragma warning disable IDE0302 // Simplify collection initialization - we want to keep stackalloc for pre-.NET10
        Span<byte> buffer = stackalloc byte[sizeof(uint)] { 0, 0, 0, 0 };
#pragma warning restore IDE0302 // Simplify collection initialization
        remainingBytes.CopyTo(buffer);
        return BitConverter.ToUInt32(buffer);
    }
}