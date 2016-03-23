// Copyright (c) 2015, 2016 Sedat Kapanoglu
// MIT License - see LICENSE file for details

using System;
namespace HashDepot.Test
{
    public class FnvTestVector
    {
        public byte[] Buffer { get; set; }
        public uint ExpectedResult32 { get; set; }
        public ulong ExpectedResult64 { get; set; }

        public override string ToString()
        {
            return BitConverter.ToString(Buffer) 
                + ExpectedResult32.ToString() 
                + ExpectedResult64.ToString();
        }
    }
}