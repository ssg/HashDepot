namespace HashDepot
{
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
                for (byte* pInput = bufPtr, pEnd = bufPtr + buffer.Length; 
                    pInput != pEnd; pInput++)
                {
                    result += *pInput;
                }
            }
            return result;
        }
    }
}