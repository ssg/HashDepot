using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace HashDepot.Test;

[TestFixture]
public class XXHashTest
{
    // test values are from https://asecuritysite.com/encryption/xxHash
    static readonly object[][] testVectors =
    [
        ["",                                              0U,     0x02cc5d05U, 0xef46db3751d8e999UL],
        ["",                                              123U,   0x3930C86EU, 0xE0DB84DE91F3E198UL],
        ["a",                                             0U,     0x550d7456U, 0xd24ec4f1a98c6e5bUL],
        ["a",                                             123U,   0xA2BCDA53U, 0x5E820EB0DDEB5AEBUL],
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
    public async Task Hash32Async_StreamTests(string text, uint seed, uint hash32, ulong _)
    {
        var buffer = Encoding.UTF8.GetBytes(text);
        using var stream = new MemoryStream(buffer);
        var result = await XXHash.Hash32Async(stream, seed);
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
    [TestCaseSource(nameof(testVectors))]
    public async Task Hash64Async_StreamTests(string text, uint seed, uint _, ulong hash64)
    {
        var buffer = Encoding.UTF8.GetBytes(text);
        using var stream = new MemoryStream(buffer);
        var result = await XXHash.Hash64Async(stream, seed);
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

    // Tests for State32 streaming API
    [Test]
    public void State32_UpdateAfterFinalBlock_ThrowsInvalidOperationException()
    {
        // Create a state with seed
        var state = new XXHash.State32(0);
        
        // Update with a buffer that has remaining bytes (not multiple of stripe length)
        // This will process the final block since buffer size < StripeLength (16)
        byte[] buffer = [1, 2, 3, 4, 5]; // 5 bytes - less than stripe length
        state.Update(buffer);
        
        // Attempting to update again should throw
        byte[] secondBuffer = [6, 7, 8];
        var ex = Assert.Throws<InvalidOperationException>(() => state.Update(secondBuffer));
    }

    [Test]
    public void State32_UpdateAfterResult_ThrowsInvalidOperationException()
    {
        // Create a state with seed
        var state = new XXHash.State32(0);
        
        // Update with a buffer that is multiple of stripe length (16 bytes)
        byte[] buffer = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16]; // 16 bytes
        state.Update(buffer);
        
        // Call Result() which processes the final block
        _ = state.Result();
        
        // Attempting to update after Result() should throw
        byte[] secondBuffer = [17, 18, 19];
        var ex = Assert.Throws<InvalidOperationException>(() => state.Update(secondBuffer));
    }

    [Test]
    public void State32_UpdateAfterRemainingBytes_ThrowsInvalidOperationException()
    {
        // Create a state with seed
        var state = new XXHash.State32(0);
        
        // Update with a buffer that is multiple of stripe length
        byte[] buffer1 = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16]; // 16 bytes
        state.Update(buffer1);
        
        // Update with a buffer that has remaining bytes (not multiple of stripe length)
        // This will process the final block
        byte[] buffer2 = [17, 18, 19, 20, 21]; // 5 bytes - remaining bytes
        state.Update(buffer2);
        
        // Attempting to update again should throw
        byte[] buffer3 = [22, 23, 24];
        var ex = Assert.Throws<InvalidOperationException>(() => state.Update(buffer3));
    }

    [Test]
    public void State32_MultipleUpdatesWithCompleteStripes_Works()
    {
        // Create a state with seed
        var state = new XXHash.State32(0);
        
        // Multiple updates with buffers that are multiples of stripe length should work
        byte[] buffer1 = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16];       // 16 bytes
        byte[] buffer2 = [17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32]; // 16 bytes
        
        // These should not throw
        Assert.DoesNotThrow(() => state.Update(buffer1));
        Assert.DoesNotThrow(() => state.Update(buffer2));
        
        // Result should work normally
        uint result = 0;
        Assert.DoesNotThrow(() => result = state.Result());
        Assert.That(result, Is.Not.Zero); // Just verify we get some result
    }

    [Test]
    public void State32_EmptyFirstUpdate_ProcessesFinalBlock()
    {
        // Create a state with seed
        var state = new XXHash.State32(0);
        
        // Update with empty buffer (length 0, which is < StripeLength)
        // This will process the final block immediately
        byte[] emptyBuffer = [];
        state.Update(emptyBuffer);
        
        // Attempting to update again should throw
        byte[] secondBuffer = [1, 2, 3];
        var ex = Assert.Throws<InvalidOperationException>(() => state.Update(secondBuffer));
    }

    // Tests for State64 streaming API
    [Test]
    public void State64_UpdateAfterFinalBlock_ThrowsInvalidOperationException()
    {
        // Create a state with seed
        var state = new XXHash.State64(0);
        
        // Update with a buffer that has remaining bytes (not multiple of stripe length)
        // This will process the final block since buffer size < StripeLength (32)
        byte[] buffer = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]; // 10 bytes - less than stripe length
        state.Update(buffer);
        
        // Attempting to update again should throw
        byte[] secondBuffer = [11, 12, 13];
        var ex = Assert.Throws<InvalidOperationException>(() => state.Update(secondBuffer));
    }

    [Test]
    public void State64_UpdateAfterResult_ThrowsInvalidOperationException()
    {
        // Create a state with seed
        var state = new XXHash.State64(0);
        
        // Update with a buffer that is multiple of stripe length (32 bytes)
        byte[] buffer = new byte[32]; // 32 bytes
        for (int i = 0; i < buffer.Length; i++) buffer[i] = (byte)(i + 1);
        state.Update(buffer);
        
        // Call Result() which processes the final block
        _ = state.Result();
        
        // Attempting to update after Result() should throw
        byte[] secondBuffer = [33, 34, 35];
        var ex = Assert.Throws<InvalidOperationException>(() => state.Update(secondBuffer));
    }

    [Test]
    public void State64_UpdateAfterRemainingBytes_ThrowsInvalidOperationException()
    {
        // Create a state with seed
        var state = new XXHash.State64(0);
        
        // Update with a buffer that is multiple of stripe length
        byte[] buffer1 = new byte[32]; // 32 bytes
        for (int i = 0; i < buffer1.Length; i++) buffer1[i] = (byte)(i + 1);
        state.Update(buffer1);
        
        // Update with a buffer that has remaining bytes (not multiple of stripe length)
        // This will process the final block
        byte[] buffer2 = [33, 34, 35, 36, 37, 38, 39, 40, 41, 42]; // 10 bytes - remaining bytes
        state.Update(buffer2);
        
        // Attempting to update again should throw
        byte[] buffer3 = [43, 44, 45];
        var ex = Assert.Throws<InvalidOperationException>(() => state.Update(buffer3));
    }

    [Test]
    public void State64_MultipleUpdatesWithCompleteStripes_Works()
    {
        // Create a state with seed
        var state = new XXHash.State64(0);
        
        // Multiple updates with buffers that are multiples of stripe length should work
        byte[] buffer1 = new byte[32]; // 32 bytes
        byte[] buffer2 = new byte[32]; // 32 bytes
        for (int i = 0; i < buffer1.Length; i++) 
        {
            buffer1[i] = (byte)(i + 1);
            buffer2[i] = (byte)(i + 33);
        }
        
        // These should not throw
        Assert.DoesNotThrow(() => state.Update(buffer1));
        Assert.DoesNotThrow(() => state.Update(buffer2));
        
        // Result should work normally
        ulong result = 0;
        Assert.DoesNotThrow(() => result = state.Result());
        Assert.That(result, Is.Not.Zero); // Just verify we get some result
    }

    [Test]
    public void State64_EmptyFirstUpdate_ProcessesFinalBlock()
    {
        // Create a state with seed
        var state = new XXHash.State64(0);
        
        // Update with empty buffer (length 0, which is < StripeLength)
        // This will process the final block immediately
        byte[] emptyBuffer = [];
        state.Update(emptyBuffer);
        
        // Attempting to update again should throw
        byte[] secondBuffer = [1, 2, 3];
        var ex = Assert.Throws<InvalidOperationException>(() => state.Update(secondBuffer));
    }

    [Test]
    public void State64_UpdateAfterMultipleResults_ThrowsInvalidOperationException()
    {
        // Create a state with seed
        var state = new XXHash.State64(0);
        
        // Update with a buffer that is multiple of stripe length (32 bytes)
        byte[] buffer = new byte[64]; // 64 bytes (2 complete stripes)
        for (int i = 0; i < buffer.Length; i++) buffer[i] = (byte)(i + 1);
        state.Update(buffer);
        
        // Call Result() multiple times (should be allowed)
        var result1 = state.Result();
        var result2 = state.Result();
        Assert.That(result1, Is.EqualTo(result2)); // Results should be the same
        
        // Attempting to update after multiple Result() calls should still throw
        byte[] secondBuffer = [65, 66, 67];
        var ex = Assert.Throws<InvalidOperationException>(() => state.Update(secondBuffer));
    }

    static byte[] getLargeBuffer()
    {
        return new byte[64 * 1024 * 1024];
    }
}
