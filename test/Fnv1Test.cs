// Copyright (c) 2015, 2016 Sedat Kapanoglu
// MIT License - see LICENSE file for details

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace HashDepot.Test
{
    [TestFixture]
    public class Fnv1Test
    {
        #pragma warning disable CS0618 // Fnv1 is now obsolete but we should still be testing it.

        public static IEnumerable<object[]> TestData = FnvVectors.GetFnv1TestVectors()
            .Select(v => new object[] { v })
            .ToArray();

        [Test]
        [TestCaseSource("TestData")]
        public void Hash32_Stream_ReturnsExpectedValues(FnvTestVector data)
        {
            using var stream = new MemoryStream(data.Buffer);
            uint result = Fnv1.Hash32(stream);
            Assert.AreEqual(data.ExpectedResult32, result);
        }

        [Test]
        [TestCaseSource("TestData")]
        public void Hash64_Stream_ReturnsExpectedValues(FnvTestVector data)
        {
            using var stream = new MemoryStream(data.Buffer);
            ulong result = Fnv1.Hash64(stream);
            Assert.AreEqual(data.ExpectedResult64, result);
        }

        [Test]
        [TestCaseSource("TestData")]
        public async Task Hash32_StreamAsync_ReturnsExpectedValuesAsync(FnvTestVector data)
        {
            using var stream = new MemoryStream(data.Buffer);
            uint result = await Fnv1.Hash32Async(stream);
            Assert.AreEqual(data.ExpectedResult32, result);
        }

        [Test]
        [TestCaseSource("TestData")]
        public async Task Hash64_StreamAsync_ReturnsExpectedValuesAsync(FnvTestVector data)
        {
            using var stream = new MemoryStream(data.Buffer);
            ulong result = await Fnv1.Hash64Async(stream);
            Assert.AreEqual(data.ExpectedResult64, result);
        }

        [Test]
        [TestCaseSource("TestData")]
        public void Hash32_ReturnsExpectedValues(FnvTestVector data)
        {
            uint result = Fnv1.Hash32(data.Buffer);
            Assert.AreEqual(data.ExpectedResult32, result);
        }

        [Test]
        [TestCaseSource("TestData")]
        public void Hash64_ReturnsExpectedValues(FnvTestVector data)
        {
            ulong result = Fnv1.Hash64(data.Buffer);
            Assert.AreEqual(data.ExpectedResult64, result);
        }
    }
}