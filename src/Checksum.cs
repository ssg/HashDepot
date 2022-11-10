// <copyright file="Checksum.cs" company="Sedat Kapanoglu">
// Copyright (c) 2015-2022 Sedat Kapanoglu
// MIT License (see LICENSE file for details)
// </copyright>

using System;

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
        int unevenLength = buffer.Length % sizeof(uint);
        for (; offset < buffer.Length - unevenLength; offset += sizeof(uint))
        {
            result += BitConverter.ToUInt32(buffer[offset..(offset + sizeof(uint))]);
        }

        if (unevenLength > 0)
        {
            result += Bits.PartialBytesToUInt32(buffer[offset..(offset + unevenLength)]);
        }

        return result;
    }
}