// Copyright (c) 2015, 2016 Sedat Kapanoglu
// MIT License - see LICENSE file for details

using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace HashDepot.Test
{
    [TestFixture]
    [Parallelizable(ParallelScope.Children)]
    public class Fnv1aTest
    {
        public static IEnumerable<object[]> TestData = FnvVectors.GetFnv1aTestVectors()
            .Select(v => new object[] { v })
            .ToArray();

        [Test]
        [TestCaseSource(nameof(TestData))]
        public void Hash32_Stream_ReturnsExpectedValues(FnvTestVector data)
        {
            using (var stream = new MemoryStream(data.Buffer))
            {
                uint result = Fnv1a.Hash32(stream);
                Assert.AreEqual(data.ExpectedResult32, result);
            }
        }

        [Test]
        [TestCaseSource(nameof(TestData))]
        public void Hash64_Stream_ReturnsExpectedValues(FnvTestVector data)
        {
            using (var stream = new MemoryStream(data.Buffer))
            {
                ulong result = Fnv1a.Hash64(stream);
                Assert.AreEqual(data.ExpectedResult64, result);
            }
        }

        [Test]
        [TestCaseSource(nameof(TestData))]
        public void Hash32_ReturnsExpectedValues(FnvTestVector data)
        {
            uint result = Fnv1a.Hash32(data.Buffer);
            Assert.AreEqual(data.ExpectedResult32, result);
        }

        [Test]
        [TestCaseSource(nameof(TestData))]
        public void Hash64_ReturnsExpectedValues(FnvTestVector data)
        {
            ulong result = Fnv1a.Hash64(data.Buffer);
            Assert.AreEqual(data.ExpectedResult64, result);
        }
    }
}