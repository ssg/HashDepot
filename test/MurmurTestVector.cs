using System;

namespace HashDepot.Test;

public class MurmurTestVector(byte[] buffer, uint seed, uint expectedResult)
{
    public byte[] Buffer = buffer;
    public uint Seed = seed;
    public uint ExpectedResult = expectedResult;

    public override string ToString()
    {
        return BitConverter.ToString(Buffer)
            + "_"
            + Seed.ToString("x")
            + "_"
            + ExpectedResult.ToString("x");
    }
}