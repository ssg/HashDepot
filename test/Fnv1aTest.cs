// Copyright (c) 2015, 2016 Sedat Kapanoglu
// MIT License - see LICENSE file for details

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace HashDepot.Test
{
    public class Fnv1aTest
    {
        [Fact]
        public void Hash32_NullBuffer_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Fnv1a.Hash32(null));
        }

        [Fact]
        public void Hash64_NullBuffer_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Fnv1a.Hash64(null));
        }

        public static IEnumerable<object[]> TestData = FnvVectors.GetFnv1aTestVectors()
            .Select(v => new object[] { v })
            .ToArray();

        [Theory]
        [MemberData("TestData")]
        public void Hash32_ReturnsExpectedValues(FnvTestVector data)
        {
            uint result = Fnv1a.Hash32(data.Buffer);
            Assert.Equal(data.ExpectedResult32, result);
        }

        [Theory]
        [MemberData("TestData")]
        public void Hash64_ReturnsExpectedValues(FnvTestVector data)
        {
            ulong result = Fnv1a.Hash64(data.Buffer);
            Assert.Equal(data.ExpectedResult64, result);
        }
    }
}