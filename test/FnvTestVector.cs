// Copyright (c) 2015, 2016 Sedat Kapanoglu
// MIT License - see LICENSE file for details

using System;
namespace HashDepot.Test;

public record FnvTestVector(byte[] Buffer, uint ExpectedResult32, ulong ExpectedResult64)
{
    public override string ToString()
    {
        return BitConverter.ToString(Buffer) 
            + "_"
            + ExpectedResult32.ToString() 
            + "_"
            + ExpectedResult64.ToString();
    }
}