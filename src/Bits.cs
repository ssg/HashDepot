// <copyright file="Bits.cs" company="Sedat Kapanoglu">
// Copyright (c) 2015-2025 Sedat Kapanoglu
// MIT License (see LICENSE file for details)
// </copyright>

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HashDepot;

/// <summary>
/// Bit operations.
/// </summary>
static class Bits
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ulong RotateLeft(ulong value, int bits)
    {
        return (value << bits) | (value >> (64 - bits));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint RotateLeft(uint value, int bits)
    {
        return (value << bits) | (value >> (32 - bits));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint RotateRight(uint value, int bits)
    {
        return (value >> bits) | (value << (32 - bits));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ulong RotateRight(ulong value, int bits)
    {
        return (value >> bits) | (value << (64 - bits));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ulong ToUInt64(ReadOnlySpan<byte> bytes)
    {
        return Unsafe.ReadUnaligned<ulong>(ref MemoryMarshal.GetReference(bytes));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint ToUInt32(ReadOnlySpan<byte> bytes)
    {
        return Unsafe.ReadUnaligned<uint>(ref MemoryMarshal.GetReference(bytes));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ulong PartialBytesToUInt64(ReadOnlySpan<byte> remainingBytes)
    {
        // a switch/case approach is slightly faster than the loop but .net
        // refuses to inline it due to larger code size.
        ulong result = 0;

        // trying to modify leftBytes would invalidate inlining
        // need to use local variable instead
        for (int i = 0; i < remainingBytes.Length; i++)
        {
            result |= ((ulong)remainingBytes[i]) << (i << 3);
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint PartialBytesToUInt32(ReadOnlySpan<byte> remainingBytes)
    {
        int len = remainingBytes.Length;
        if (len > 3)
        {
            return Bits.ToUInt32(remainingBytes);
        }

        // a switch/case approach is slightly faster than the loop but .net
        // refuses to inline it due to larger code size.
        uint result = remainingBytes[0];
        if (len > 1)
        {
            result |= (uint)(remainingBytes[1] << 8);
        }

        if (len > 2)
        {
            result |= (uint)(remainingBytes[2] << 16);
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint SwapBytes32(uint num)
    {
        return (Bits.RotateLeft(num, 8) & 0x00FF00FFu)
             | (Bits.RotateRight(num, 8) & 0xFF00FF00u);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ulong SwapBytes64(ulong num)
    {
        num = (Bits.RotateLeft(num, 48) & 0xFFFF0000FFFF0000ul)
            | (Bits.RotateLeft(num, 16) & 0x0000FFFF0000FFFFul);
        return (Bits.RotateLeft(num, 8) & 0xFF00FF00FF00FF00ul)
             | (Bits.RotateRight(num, 8) & 0x00FF00FF00FF00FFul);
    }
}