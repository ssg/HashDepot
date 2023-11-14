// Copyright (c) 2015, 2016 Sedat Kapanoglu
// MIT License - see LICENSE file for details

using System;
namespace HashDepot.Test;

public class FnvTestVector(byte[] buffer, uint expectedResult32, ulong expectedResult64)
{
    public byte[] Buffer { get; set; } = buffer;
    public uint ExpectedResult32 { get; set; } = expectedResult32;
    public ulong ExpectedResult64 { get; set; } = expectedResult64;

    public override string ToString()
    {
        return BitConverter.ToString(Buffer) 
            + "_"
            + ExpectedResult32.ToString() 
            + "_"
            + ExpectedResult64.ToString();
    }
}