using System.Text;
using NUnit.Framework;
using SimpleBase;

namespace HashDepot.Test
{
    [TestFixture]
    class MurmurHash3x64Test
    {
        [Test]
        [TestCase("Hello World", "1a6326abc1a0c2db83e61fcf9fc0b427")]
        [TestCase("I will not buy this tobacconist's, it is scratched.", "d30654abbd8227e367d73523f0079673")] // source: https://github.com/pid/murmurHash3js
        [TestCase("I will not buy this tobacconist's, it is scratched.", "9b5b7ba2ef3f7866889adeaf00f3f98e")] // this is the x86 version but just in case our implementation fucks up
        public void Hash128_Preliminary(string input, string expectedOutput)
        {
            var expectedBuffer = Base16.Decode(expectedOutput).ToArray();
            var buffer = Encoding.UTF8.GetBytes(input);
            uint seed = 0;

            var result = MurmurHash3x64.Hash128(buffer, seed);
            CollectionAssert.AreEquivalent(expectedBuffer, buffer);
        }
    }
}