using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashDepot.Test
{
    public class MurmurTestVector
    {
        public byte[] Buffer;
        public uint Seed;
        public uint ExpectedResult;

        public MurmurTestVector(byte[] buffer, uint seed, uint expectedResult)
        {
            this.Buffer = buffer;
            this.Seed = seed;
            this.ExpectedResult = expectedResult;
        }

        public override string ToString()
        {
            return BitConverter.ToString(Buffer)
                + "_"
                + Seed.ToString("x")
                + "_"
                + ExpectedResult.ToString("x");
        }
    }
}
