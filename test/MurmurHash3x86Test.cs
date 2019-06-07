using System.IO;
using System.Text;
using NUnit.Framework;

namespace HashDepot.Test
{
    // test vectors are courtesy of Ian Boyd -- https://stackoverflow.com/users/12597/ian-boyd
    [TestFixture]
    public class MurmurHash3x86Test
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
        [TestCaseSource(nameof(smHasherTestData))]
        public void Hash32_BinaryTests(MurmurTestVector vector)
        {
            uint result = MurmurHash3x86.Hash32(vector.Buffer, vector.Seed);
            Assert.AreEqual(vector.ExpectedResult, result);
        }

        [Test]
        [TestCaseSource(nameof(smHasherTestData))]
        public void Hash32_Stream_BinaryTests(MurmurTestVector vector)
        {
            using (var stream = new MemoryStream(vector.Buffer))
            {
                uint result = MurmurHash3x86.Hash32(stream, vector.Seed);
                Assert.AreEqual(vector.ExpectedResult, result);
            }
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
        [TestCase("My hovercraft is full of eels.", 25U, 2520298415U)] // source: https://github.com/pid/murmurHash3js
        public void Hash32_StringTests(string text, uint seed, uint expectedResult)
        {
            uint result = MurmurHash3x86.Hash32(Encoding.UTF8.GetBytes(text), seed);
            Assert.AreEqual(expectedResult, result);
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
        public void Hash32_Stream_StringTests(string text, uint seed, uint expectedResult)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            {
                uint result = MurmurHash3x86.Hash32(stream, seed);
                Assert.AreEqual(expectedResult, result);
            }
        }
    }
}