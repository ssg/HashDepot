// Copyright (c) 2015, 2016 Sedat Kapanoglu
// MIT License - see LICENSE file for details

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace HashDepot.Test;

[TestFixture]
public class Fnv1aTest
{
    internal static IEnumerable<object[]> TestData = FnvVectors.GetFnv1aTestVectors()
        .Select(v => new object[] { v })
        .ToArray();

    [Test]
    [TestCaseSource(nameof(TestData))]
    public void Hash32_Stream_ReturnsExpectedValues(FnvTestVector data)
    {
        using var stream = new MemoryStream(data.Buffer);
        uint result = Fnv1a.Hash32(stream);
        Assert.That(result, Is.EqualTo(data.ExpectedResult32));
    }

    [Test]
    [TestCaseSource(nameof(TestData))]
    public void Hash64_Stream_ReturnsExpectedValues(FnvTestVector data)
    {
        using var stream = new MemoryStream(data.Buffer);
        ulong result = Fnv1a.Hash64(stream);
        Assert.That(result, Is.EqualTo(data.ExpectedResult64));
    }

    [Test]
    [TestCaseSource(nameof(TestData))]
    public async Task Hash32_StreamAsync_ReturnsExpectedValuesAsync(FnvTestVector data)
    {
        using var stream = new MemoryStream(data.Buffer);
        uint result = await Fnv1a.Hash32Async(stream);
        Assert.That(result, Is.EqualTo(data.ExpectedResult32));
    }

    [Test]
    [TestCaseSource(nameof(TestData))]
    public async Task Hash64_StreamAsync_ReturnsExpectedValuesAsync(FnvTestVector data)
    {
        using var stream = new MemoryStream(data.Buffer);
        ulong result = await Fnv1a.Hash64Async(stream);
        Assert.That(result, Is.EqualTo(data.ExpectedResult64));
    }

    [Test]
    [TestCaseSource(nameof(TestData))]
    public void Hash32_ReturnsExpectedValues(FnvTestVector data)
    {
        uint result = Fnv1a.Hash32(data.Buffer);
        Assert.That(result, Is.EqualTo(data.ExpectedResult32));
    }

    [Test]
    [TestCaseSource(nameof(TestData))]
    public void Hash64_ReturnsExpectedValues(FnvTestVector data)
    {
        ulong result = Fnv1a.Hash64(data.Buffer);
        Assert.That(result, Is.EqualTo(data.ExpectedResult64));
    }
}