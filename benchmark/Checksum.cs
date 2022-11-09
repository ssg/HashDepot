namespace HashDepot;

/// <summary>
/// Dummy checksum implementations for benchmark baseline. Don't use in real life.
/// </summary>
public static class Checksum
{
    public static unsafe uint Hash32(byte[] buffer)
    {
        uint result = 0;
        fixed (byte* bufPtr = buffer)
        {
            int len = buffer.Length;
            uint* pInput = (uint*)bufPtr;
            for (; len >= sizeof(uint); pInput++, len -= sizeof(uint))
            {
                result += *pInput;
            }

            if (len > 0)
            {
                result += Bits.PartialBytesToUInt32((byte*)pInput, len);
            }
        }
        return result;
    }
}