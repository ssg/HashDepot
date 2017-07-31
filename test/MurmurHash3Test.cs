using System.Text;
using NUnit.Framework;
using SimpleBase;
using HashDepot;

namespace HashDepot.Test
{
    // test vectors are courtesy of Ian Boyd -- https://stackoverflow.com/users/12597/ian-boyd
    [TestFixture]
    public class MurmurHash3Test
    {
        private static readonly MurmurTestVector[] smHasherTestData = new MurmurTestVector[]
        {
            new MurmurTestVector(new byte[0], 0U,                                   0),
            new MurmurTestVector(new byte[0], 1U,                                   0x514E28B7),
            new MurmurTestVector(new byte[0], 0xFFFFFFFF,                           0x81F16F39),
            new MurmurTestVector(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }, 0,          0x76293B50),
            new MurmurTestVector(new byte[] { 0x21, 0x43, 0x65, 0x87 }, 0,          0xF55B516B),
            new MurmurTestVector(new byte[] { 0x21, 0x43, 0x65, 0x87 }, 0x5082EDEE, 0x2362F9DE),
            new MurmurTestVector(new byte[] { 0x21, 0x43, 0x65 }, 0,                0x7E4A8634),
            new MurmurTestVector(new byte[] { 0x21, 0x43 }, 0,                      0xA0F7B07A),
            new MurmurTestVector(new byte[] { 0x21 }, 0,                            0x72661CF4),
            new MurmurTestVector(new byte[] { 0, 0, 0, 0 }, 0,                      0x2362F9DE),
            new MurmurTestVector(new byte[] { 0, 0, 0 }, 0,                         0x85F0B427),
            new MurmurTestVector(new byte[] { 0, 0 }, 0,                            0x30F4C306),
            new MurmurTestVector(new byte[] { 0 }, 0,                               0x514E28B7),
        };

        [Test]
        [TestCaseSource("smHasherTestData")]
        public void Hash32_BinaryTests(MurmurTestVector vector)
        {
            uint result = MurmurHash3.Hash32(vector.Buffer, vector.Seed);
            Assert.AreEqual(vector.ExpectedResult, result);
        }

        [Test]
        [TestCase("", 0U, 0U)]
        [TestCase("", 1U, 0x514E28B7U)]
        [TestCase("", 0xffffffffU, 0x81F16F39U)]
        [TestCase("\0\0\0\0", 0U, 0x2362F9DEU)]
        [TestCase("aaaa", 0x9747b28cU, 0x5A97808AU)]
        [TestCase("aaa", 0x9747b28cU, 0x283E0130U)]
        [TestCase("aa", 0x9747b28cU, 0x5D211726U)]
        [TestCase("a", 0x9747b28cU, 0x7FA09EA6U)]
        [TestCase("abcd", 0x9747b28cU, 0xF0478627U)]
        [TestCase("abc", 0x9747b28cU, 0xC84A62DDU)]
        [TestCase("ab", 0x9747b28cU, 0x74875592U)]
        public void Hash32_StringTests(string text, uint seed, uint expectedResult)
        {
            uint result = MurmurHash3.Hash32(Encoding.UTF8.GetBytes(text), seed);
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        [Ignore("not finished the implementation yet")]
        [TestCase("The quick brown fox jumps over the lazy dog", "213163D23B7F8A73E516C07E727345F9", 0x9747b28cU)]
        [TestCase("The quick brown fox jumps over the lazy cog", "94618270B057CDB83CF873585B456F55", 0x9747b28cU)]
        [TestCase("THE QUICK BROWN FOX JUMPS OVER THE LAZY COG", "DD904D9A52D2FB5E71EAD0546624BEA0", 0x9747b28cU)]
        [TestCase("the quick brown fox jumps over the lazy dog", "A8FA6851BD2C21CDF33E80C8968B74D0", 0x9747b28cU)]
        [TestCase("THE QUICK BROWN FOX JUMPS OVER THE LAZY DOG", "6C8AD027E39089788AD2CB67789CA4CF", 0x9747b28cU)]
        [TestCase("the quick brown fox jumps over the lazy cog", "714C9A5ADD16AA271F90A72183FD2BE0", 0x9747b28cU)]
        [TestCase("The quick brown fox jumps over the lazy dog", "6C1B07BC7BBC4BE347939AC4A93C437A", 0U)]
        [TestCase("The quick brown fox jumps over the lazy cog", "9A2685FF70A98C653E5C8EA6EAE3FE43", 0U)]
        [TestCase("THE QUICK BROWN FOX JUMPS OVER THE LAZY DOG", "C9FB0A32011820A64B3C7A60B06C3982", 0U)]
        [TestCase("THE QUICK BROWN FOX JUMPS OVER THE LAZY COG", "24B0E694C86C766A6C8FD44492BB010B", 0U)]
        [TestCase("the quick brown fox jumps over the lazy dog", "B386ADE2FEE9E4BC7F4B6E4074E3E20A", 0U)]
        [TestCase("the quick brown fox jumps over the lazy cog", "3222507256FE092F24D124BB1E8D7586", 0U)]         
        public void Hash128_ReturnsExpectedVersions(string text, string expectedHash, uint seed)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            byte[] expectedResult = Base16.Decode(expectedHash);
            byte[] result = MurmurHash3.Hash128(buffer, seed);
            CollectionAssert.AreEqual(expectedResult, result);
        }

    }
}