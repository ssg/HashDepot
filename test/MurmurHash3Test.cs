using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SimpleBase;

namespace HashDepot.Test;

// test vectors are courtesy of Ian Boyd -- https://stackoverflow.com/users/12597/ian-boyd
[TestFixture]
public class MurmurHash3Test
{
    private static readonly MurmurTestVector[] smHasherTestData = new MurmurTestVector[]
    {
        new MurmurTestVector(Array.Empty<byte>(), 0U,                                   0),
        new MurmurTestVector(Array.Empty<byte>(), 1U,                                   0x514E28B7),
        new MurmurTestVector(Array.Empty<byte>(), 0xFFFFFFFF,                           0x81F16F39),
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
        uint result = MurmurHash3.Hash32(vector.Buffer, vector.Seed);
        Assert.That(result, Is.EqualTo(vector.ExpectedResult));
    }

    [Test]
    [TestCaseSource(nameof(smHasherTestData))]
    public void Hash32_Stream_BinaryTests(MurmurTestVector vector)
    {
        using var stream = new MemoryStream(vector.Buffer);
        uint result = MurmurHash3.Hash32(stream, vector.Seed);
        Assert.That(result, Is.EqualTo(vector.ExpectedResult));
    }

    [Test]
    [TestCaseSource(nameof(smHasherTestData))]
    public async Task Hash32_StreamAsync_BinaryTestsAsync(MurmurTestVector vector)
    {
        using var stream = new MemoryStream(vector.Buffer);
        uint result = await MurmurHash3.Hash32Async(stream, vector.Seed);
        Assert.That(result, Is.EqualTo(vector.ExpectedResult));
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
        uint result = MurmurHash3.Hash32(Encoding.UTF8.GetBytes(text), seed);
        Assert.That(result, Is.EqualTo(expectedResult));
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
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
        uint result = MurmurHash3.Hash32(stream, seed);
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    [TestCase("Hello World", "1a6326abc1a0c2db83e61fcf9fc0b427")]
    [TestCase("I will not buy this tobacconist's, it is scratched.", "d30654abbd8227e367d73523f0079673")] // source: https://github.com/pid/murmurHash3js
    public void Hash128_Preliminary(string input, string expectedOutput)
    {
        var expectedBuffer = Base16.Decode(expectedOutput).ToArray();
        var buffer = Encoding.UTF8.GetBytes(input);
        uint seed = 0;

        var result = MurmurHash3.Hash128(buffer, seed);
        CollectionAssert.AreEquivalent(expectedBuffer, result);
    }

    [Test]
    [TestCase("Hello World", "1a6326abc1a0c2db83e61fcf9fc0b427")]
    [TestCase("I will not buy this tobacconist's, it is scratched.", "d30654abbd8227e367d73523f0079673")] // source: https://github.com/pid/murmurHash3js
    public void Hash128_Stream_Preliminary(string input, string expectedOutput)
    {
        var expectedBuffer = Base16.Decode(expectedOutput).ToArray();
        var buffer = Encoding.UTF8.GetBytes(input);
        uint seed = 0;

        using var stream = new MemoryStream(buffer);
        var result = MurmurHash3.Hash128(stream, seed);
        CollectionAssert.AreEquivalent(expectedBuffer, result);
    }
}