using System;

namespace HashDepot;

/// <summary>
/// Dummy checksum implementations for benchmark baseline. Don't use in real life.
/// </summary>
public static class Checksum
{
    public static uint Hash32(ReadOnlySpan<byte> buffer)
    {
        uint result = 0;
        int i = 0;
        for (; i < buffer.Length; i += sizeof(uint))
        {
            result += BitConverter.ToUInt32(buffer[i..(i + 4)]);
        }

        int len = buffer.Length - i;
        if (len > 0)
        {
            result += Bits.PartialBytesToUInt32(buffer[i..(i + len)]);
        }
        return result;
    }
}