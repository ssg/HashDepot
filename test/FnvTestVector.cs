// Copyright (c) 2015, 2016 Sedat Kapanoglu
// MIT License - see LICENSE file for details

using System;
namespace HashDepot.Test;

public class FnvTestVector
{
    public byte[] Buffer { get; set; }
    public uint ExpectedResult32 { get; set; }
    public ulong ExpectedResult64 { get; set; }

    public FnvTestVector(byte[] buffer, uint expectedResult32, ulong expectedResult64)
    {
        Buffer = buffer;
        ExpectedResult32 = expectedResult32;
        ExpectedResult64 = expectedResult64;
    }

    public override string ToString()
    {
        return BitConverter.ToString(Buffer) 
            + "_"
            + ExpectedResult32.ToString() 
            + "_"
            + ExpectedResult64.ToString();
    }
}