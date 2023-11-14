using NUnit.Framework;
using System.IO;
using System.Text;

namespace HashDepot.Test;

[TestFixture]
public class XXHashTest
{
    // test values are from https://asecuritysite.com/encryption/xxHash
    private static readonly object[][] testVectors =
    [
        ["",                                              0U,     0x02cc5d05U, 0xef46db3751d8e999UL],
        ["a",                                             0U,     0x550d7456U, 0xd24ec4f1a98c6e5bUL],
        ["123",                                           0U,     0xb6855437U, 0x3c697d223fa7e885UL],
        ["1234",                                          0U,     0x01543429U, 0xd8316e61d84f6ba4UL],
        ["123456789012345",                               0U,     0xda7b17e8U, 0xc377d78ade001a3cUL],
        ["1234567890123456123456789012345",               0U,     0xf3556ecfU, 0x8947ecb58263b70fUL],
        ["Nobody inspects the spammish repetition",       0u,     0xe2293b2fU, 0xfbcea83c8a378bf1UL],
        ["Nobody inspects the spammish repetition",       123u,   0xfa7f6052U, 0xa8ba45551f24b7aeUL],
        ["The quick brown fox jumps over the lazy dog",   0u,     0xe85ea4deU, 0x0b242d361fda71bcUL],
    ];

    [Test]
    [TestCaseSource(nameof(testVectors))]
    public void Hash32_BinaryTests(string text, uint seed, uint hash32, ulong _)
    {
        var buffer = Encoding.UTF8.GetBytes(text);
        var result = XXHash.Hash32(buffer, seed);
        Assert.That(result, Is.EqualTo(hash32));
    }

    [Test]
    [TestCaseSource(nameof(testVectors))]
    public void Hash32_StreamTests(string text, uint seed, uint hash32, ulong _)
    {
        var buffer = Encoding.UTF8.GetBytes(text);
        using var stream = new MemoryStream(buffer);
        var result = XXHash.Hash32(stream, seed);
        Assert.That(result, Is.EqualTo(hash32));
    }

    [Test]
    [TestCaseSource(nameof(testVectors))]
    public void Hash64_BinaryTests(string text, uint seed, uint _, ulong hash64)
    {
        var buffer = Encoding.UTF8.GetBytes(text);
        var result = XXHash.Hash64(buffer, seed);
        Assert.That(result, Is.EqualTo(hash64));
    }

    [Test]
    [TestCaseSource(nameof(testVectors))]
    public void Hash64_StreamTests(string text, uint seed, uint _, ulong hash64)
    {
        var buffer = Encoding.UTF8.GetBytes(text);
        using var stream = new MemoryStream(buffer);
        var result = XXHash.Hash64(stream, seed);
        Assert.That(result, Is.EqualTo(hash64));
    }

    [Test]
    public void Hash32_LongStream_ReturnsExpected()
    {
        var buffer = getLargeBuffer();
        using var stream = new MemoryStream(buffer);
        Assert.That(XXHash.Hash32(stream), Is.EqualTo(3662909991));
    }

    [Test]
    public void Hash64_LongStream_ReturnsExpected()
    {
        var buffer = getLargeBuffer();
        using var stream = new MemoryStream(buffer);
        Assert.That(XXHash.Hash64(stream), Is.EqualTo(17345881079506341799));
    }

    private static byte[] getLargeBuffer()
    {
        return new byte[64 * 1024 * 1024];
    }
}
