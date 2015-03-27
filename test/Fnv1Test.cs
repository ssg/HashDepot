using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Fnv.Test
{
    public class Fnv1Test
    {
        [Fact]
        public void Hash32_NullBuffer_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Fnv1.Hash32(null));
        }

        [Fact]
        public void Hash64_NullBuffer_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Fnv1.Hash64(null));
        }

        public static IEnumerable<object[]> TestData = Vectors.GetFnv1TestVectors()
            .Select(v => new object[] { v })
            .ToArray();

        [Theory]
        [MemberData("TestData")]
        public void Hash32_ReturnsExpectedValues(TestVector data)
        {
            uint result = Fnv1.Hash32(data.Buffer);
            Assert.Equal(data.ExpectedResult32, result);
        }

        [Theory]
        [MemberData("TestData")]
        public void Hash64_ReturnsExpectedValues(TestVector data)
        {
            ulong result = Fnv1.Hash64(data.Buffer);
            Assert.Equal(data.ExpectedResult64, result);
        }
    }
}