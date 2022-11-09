// <copyright file="Bits.cs" company="Sedat Kapanoglu">
// Copyright (c) 2015-2021 Sedat Kapanoglu
// MIT License (see LICENSE file for details)
// </copyright>

using System;
using System.Runtime.CompilerServices;

namespace HashDepot;

/// <summary>
/// Bit operations.
/// </summary>
internal static class Bits
{
#pragma warning disable SA1401 // Fields should be private - this isn't publicly exposed
    internal static bool IsBigEndian = !BitConverter.IsLittleEndian;
#pragma warning restore SA1401 // Fields should be private

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
    internal static unsafe ulong PartialBytesToUInt64(byte* ptr, int leftBytes)
    {
        // a switch/case approach is slightly faster than the loop but .net
        // refuses to inline it due to larger code size.
        ulong result = 0;

        // trying to modify leftBytes would invalidate inlining
        // need to use local variable instead
        for (int i = 0; i < leftBytes; i++)
        {
            result |= ((ulong)ptr[i]) << (i << 3);
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static unsafe ulong PartialBytesToUInt64(byte[] buffer, int leftBytes)
    {
        // a switch/case approach is slightly faster than the loop but .net
        // refuses to inline it due to larger code size.
        ulong result = 0;

        // trying to modify leftBytes would invalidate inlining
        // need to use local variable instead
        for (int i = 0; i < leftBytes; i++)
        {
            result |= ((ulong)buffer[i]) << (i << 3);
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static unsafe uint PartialBytesToUInt32(byte* ptr, int leftBytes)
    {
        if (leftBytes > 3)
        {
            return *(uint*)ptr;
        }

        // a switch/case approach is slightly faster than the loop but .net
        // refuses to inline it due to larger code size.
        uint result = *ptr;
        if (leftBytes > 1)
        {
            result |= (uint)(ptr[1] << 8);
        }

        if (leftBytes > 2)
        {
            result |= (uint)(ptr[2] << 16);
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static unsafe uint PartialBytesToUInt32(byte[] buffer, int leftBytes)
    {
        if (leftBytes > 3)
        {
            return BitConverter.ToUInt32(buffer, 0);
        }

        // a switch/case approach is slightly faster than the loop but .net
        // refuses to inline it due to larger code size.
        uint result = buffer[0];
        if (leftBytes > 1)
        {
            result |= (uint)(buffer[1] << 8);
        }

        if (leftBytes > 2)
        {
            result |= (uint)(buffer[2] << 16);
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