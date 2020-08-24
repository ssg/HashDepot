// Copyright (c) 2015, 2016 Sedat Kapanoglu
// MIT License - see LICENSE file for details

using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace HashDepot.Test
{
    [TestFixture]
    public class SipHash24Test
    {
        // test vectors are from https://github.com/veorq/SipHash
        private static readonly ulong[] vectors = new ulong[]
        {
            0x726fdb47dd0e0e31UL,
            0x74f839c593dc67fdUL,
            0x0d6c8009d9a94f5aUL,
            0x85676696d7fb7e2dUL,
            0xcf2794e0277187b7UL,
            0x18765564cd99a68dUL,
            0xcbc9466e58fee3ceUL,
            0xab0200f58b01d137UL,
            0x93f5f5799a932462UL,
            0x9e0082df0ba9e4b0UL,
            0x7a5dbbc594ddb9f3UL,
            0xf4b32f46226bada7UL,
            0x751e8fbc860ee5fbUL,
            0x14ea5627c0843d90UL,
            0xf723ca908e7af2eeUL,
            0xa129ca6149be45e5UL,
            0x3f2acc7f57c29bdbUL,
            0x699ae9f52cbe4794UL,
            0x4bc1b3f0968dd39cUL,
            0xbb6dc91da77961bdUL,
            0xbed65cf21aa2ee98UL,
            0xd0f2cbb02e3b67c7UL,
            0x93536795e3a33e88UL,
            0xa80c038ccd5ccec8UL,
            0xb8ad50c6f649af94UL,
            0xbce192de8a85b8eaUL,
            0x17d835b85bbb15f3UL,
            0x2f2e6163076bcfadUL,
            0xde4daaaca71dc9a5UL,
            0xa6a2506687956571UL,
            0xad87a3535c49ef28UL,
            0x32d892fad841c342UL,
            0x7127512f72f27cceUL,
            0xa7f32346f95978e3UL,
            0x12e0b01abb051238UL,
            0x15e034d40fa197aeUL,
            0x314dffbe0815a3b4UL,
            0x027990f029623981UL,
            0xcadcd4e59ef40c4dUL,
            0x9abfd8766a33735cUL,
            0x0e3ea96b5304a7d0UL,
            0xad0c42d6fc585992UL,
            0x187306c89bc215a9UL,
            0xd4a60abcf3792b95UL,
            0xf935451de4f21df2UL,
            0xa9538f0419755787UL,
            0xdb9acddff56ca510UL,
            0xd06c98cd5c0975ebUL,
            0xe612a3cb9ecba951UL,
            0xc766e62cfcadaf96UL,
            0xee64435a9752fe72UL,
            0xa192d576b245165aUL,
            0x0a8787bf8ecb74b2UL,
            0x81b3e73d20b49b6fUL,
            0x7fa8220ba3b2eceaUL,
            0x245731c13ca42499UL,
            0xb78dbfaf3a8d83bdUL,
            0xea1ad565322a1a0bUL,
            0x60e61c23a3795013UL,
            0x6606d7e446282b93UL,
            0x6ca4ecb15c5f91e1UL,
            0x9f626da15c9625f3UL,
            0xe51b38608ef25f57UL,
            0x958a324ceb064572UL,
        };

        private static readonly byte[] key = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

        [Test]
        public void Hash64_Binary_TestVectors()
        {
            for (int i = 0; i < vectors.Length; ++i)
            {
                var buffer = getBuffer(i);
                var result = SipHash24.Hash64(buffer, key);
                var expectedResult = vectors[i];
                Debug.WriteLine("testing iteration #" + i);
                Assert.That(result, Is.EqualTo(expectedResult));
            }
        }

        [Test]
        public void Hash64_Stream_TestVectors()
        {
            for (int i = 0; i < vectors.Length; ++i)
            {
                var buffer = getBuffer(i);
                using var stream = new MemoryStream(buffer);
                var result = SipHash24.Hash64(stream, key);
                var expectedResult = vectors[i];
                Debug.WriteLine("testing iteration #" + i);
                Assert.That(result, Is.EqualTo(expectedResult));
            }
        }

        [Test]
        public async Task Hash64Async_TestVectors()
        {
            for (int i = 0; i < vectors.Length; ++i)
            {
                var buffer = getBuffer(i);
                using var stream = new MemoryStream(buffer);
                ulong result = await SipHash24.Hash64Async(stream, key);
                ulong expectedResult = vectors[i];
                Assert.That(result, Is.EqualTo(expectedResult));
            }
        }

        [Test]
        public void Hash64Async_InvalidKeyLength_Throws()
        {
            using var stream = new MemoryStream(new byte[0]);
            Assert.ThrowsAsync<ArgumentException>(async () => await SipHash24.Hash64Async(stream, new byte[15]));
        }

        [Test]
        public void Hash64_Stream_InvalidKeyLength_Throws()
        {
            using var stream = new MemoryStream(new byte[0]);
            Assert.Throws<ArgumentException>(() => SipHash24.Hash64(stream, new byte[15]));
        }

        private static byte[] getBuffer(int i)
        {
            var buffer = new byte[i];
            for (int j = 0; j < i; j++)
            {
                buffer[j] = (byte)j;
            }

            return buffer;
        }

        [Test]
        [TestCase(15)]
        [TestCase(17)]
        public void Hash64_InvalidKeyLength_Throws(int keyLength)
        {
            var invalidKey = new byte[keyLength];
            var buffer = new byte[0];
            Assert.Throws<ArgumentException>(() => SipHash24.Hash64(buffer, invalidKey));
        }

        [TestFixture]
        public class BigEndian
        {
            [SetUp]
            public void Setup()
            {
                Bits.IsBigEndian = true;
            }

            [TearDown]
            public void Teardown()
            {
                Bits.IsBigEndian = false;
            }

            [Test]
            public void Hash64_BinaryBigEndian_Throws()
            {
                Assert.Throws<NotSupportedException>(() => SipHash24.Hash64(new byte[0], new byte[16]));
            }

            [Test]
            public void Hash64_StreamBigEndian_Throws()
            {
                Assert.Throws<NotSupportedException>(() => SipHash24.Hash64(new MemoryStream(new byte[0]), new byte[16]));
            }

            [Test]
            public void Hash64Async_BigEndian_Throws()
            {
                Assert.ThrowsAsync<NotSupportedException>(() =>
                    SipHash24.Hash64Async(new MemoryStream(new byte[0]), new byte[16]));
            }
        }
    }
}