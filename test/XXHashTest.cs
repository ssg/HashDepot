using System.IO;
using System.Text;
using NUnit.Framework;

namespace HashDepot.Test
{
    [TestFixture]
    public class XXHashTest
    {
        // test values are from https://asecuritysite.com/encryption/xxHash
        private static readonly object[] testVectors = new[]
        {
            new object[] { "",                                              0U,     0x02cc5d05U, 0xef46db3751d8e999UL },
            new object[] { "a",                                             0U,     0x550d7456U, 0xd24ec4f1a98c6e5bUL },
            new object[] { "123",                                           0U,     0xb6855437U, 0x3c697d223fa7e885UL },
            new object[] { "1234",                                          0U,     0x01543429U, 0xd8316e61d84f6ba4UL },
            new object[] { "123456789012345",                               0U,     0xda7b17e8U, 0xc377d78ade001a3cUL },
            new object[] { "Nobody inspects the spammish repetition",       0u,     0xe2293b2fU, 0xfbcea83c8a378bf1UL },
            new object[] { "Nobody inspects the spammish repetition",       123u,   0xfa7f6052U, 0xa8ba45551f24b7aeUL },
            new object[] { "The quick brown fox jumps over the lazy dog",   0u,     0xe85ea4deU, 0x0b242d361fda71bcUL },
        };

        [Test]
        [TestCaseSource(nameof(testVectors))]
        public void Hash32_BinaryTests(string text, uint seed, uint hash32, ulong _)
        {
            var buffer = Encoding.UTF8.GetBytes(text);
            var result = XXHash.Hash32(buffer, seed);
            Assert.AreEqual(hash32, result);
        }

        [Test]
        [TestCaseSource(nameof(testVectors))]
        public void Hash32_StreamTests(string text, uint seed, uint hash32, ulong _)
        {
            var buffer = Encoding.UTF8.GetBytes(text);
            using (var stream = new MemoryStream(buffer))
            {
                var result = XXHash.Hash32(stream, seed);
                Assert.AreEqual(hash32, result);
            }
        }

        [Test]
        [TestCaseSource(nameof(testVectors))]
        public void Hash64_BinaryTests(string text, uint seed, uint _, ulong hash64)
        {
            var buffer = Encoding.UTF8.GetBytes(text);
            var result = XXHash.Hash64(buffer, seed);
            Assert.AreEqual(hash64, result);
        }

        [Test]
        [TestCaseSource(nameof(testVectors))]
        public void Hash64_StreamTests(string text, uint seed, uint _, ulong hash64)
        {
            var buffer = Encoding.UTF8.GetBytes(text);
            using (var stream = new MemoryStream(buffer))
            {
                var result = XXHash.Hash64(stream, seed);
                Assert.AreEqual(hash64, result);
            }
        }

    }
}
