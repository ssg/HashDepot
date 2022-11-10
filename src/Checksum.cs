// <copyright file="Checksum.cs" company="Sedat Kapanoglu">
// Copyright (c) 2015-2022 Sedat Kapanoglu
// MIT License (see LICENSE file for details)
// </copyright>

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HashDepot;

/// <summary>
/// Dummy checksum implementation for benchmark baseline. Don't use in real life.
/// </summary>
internal static class Checksum
{
    public static uint Hash32(ReadOnlySpan<byte> buffer)
    {
        uint result = 0;
        int offset = 0;
        int len = buffer.Length;
        int unevenLength = len % sizeof(uint);
        int evenLength = len - unevenLength;
        while (offset < evenLength)
        {
            int end = offset + sizeof(uint);
            result += Unsafe.ReadUnaligned<uint>(ref MemoryMarshal.GetReference(buffer[offset..end]));
            offset = end;
        }

        if (unevenLength > 0)
        {
            result += Bits.PartialBytesToUInt32(buffer[offset..(offset + unevenLength)]);
        }

        return result;
    }
}