// Copyright (c) 2015, 2016 Sedat Kapanoglu
// MIT License - see LICENSE file for details

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace HashDepot.Test;

[TestFixture]
public class SipHash24Test
{
    // test vectors are from https://github.com/veorq/SipHash
    static readonly ulong[] vectors =
    [
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
    ];

    static readonly byte[] key = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15];

    static IEnumerable<object[]> testInput()
    {
        for (int i = 0; i < vectors.Length; ++i)
        {
            yield return new object[] { getIncrementalBuffer(i), vectors[i] };
        }
    }

    static byte[] getIncrementalBuffer(int i)
    {
        var buffer = new byte[i];
        for (int j = 0; j < i; j++)
        {
            buffer[j] = (byte)j;
        }

        return buffer;
    }

    [Test]
    [TestCaseSource(nameof(testInput))]
    public void Hash64_Binary_TestVectors(byte[] buffer, ulong expectedResult)
    {
        var result = SipHash24.Hash64(buffer, key);
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    [TestCaseSource(nameof(testInput))]
    public void Hash64_Stream_TestVectors(byte[] buffer, ulong expectedResult)
    {
        using var stream = new MemoryStream(buffer);
        ulong result = SipHash24.Hash64(stream, key);
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    [TestCaseSource(nameof(testInput))]
    public async Task Hash64Async_TestVectors(byte[] buffer, ulong expectedResult)
    {
        using var stream = new MemoryStream(buffer);
        ulong result = await SipHash24.Hash64Async(stream, key);
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    public void Hash64Async_InvalidKeyLength_Throws()
    {
        using var stream = new MemoryStream([]);
        _ = Assert.ThrowsAsync<ArgumentException>(async () => await SipHash24.Hash64Async(stream, new byte[15]));
    }

    [Test]
    public void Hash64_Stream_InvalidKeyLength_Throws()
    {
        using var stream = new MemoryStream([]);
        _ = Assert.Throws<ArgumentException>(() => SipHash24.Hash64(stream, new byte[15]));
    }

    [Test]
    [TestCase(15)]
    [TestCase(17)]
    public void Hash64_InvalidKeyLength_Throws(int keyLength)
    {
        var invalidKey = new byte[keyLength];
        var buffer = Array.Empty<byte>();
        _ = Assert.Throws<ArgumentException>(() => SipHash24.Hash64(buffer, invalidKey));
    }

    [Test]
    public void State64_UpdateAfterFinalBlock_ThrowsInvalidOperationException()
    {
        // Create a state with valid key
        var state = new SipHash24.State64(key);
        
        // Update with a buffer that has remaining bytes (not multiple of 8)
        // This will process the final block
        byte[] buffer = [1, 2, 3, 4, 5]; // 5 bytes - not multiple of 8
        state.Update(buffer);
        
        // Attempting to update again should throw
        byte[] secondBuffer = [6, 7, 8];
        var ex = Assert.Throws<InvalidOperationException>(() => state.Update(secondBuffer));
    }

    [Test]
    public void State64_UpdateAfterResult_ThrowsInvalidOperationException()
    {
        // Create a state with valid key
        var state = new SipHash24.State64(key);
        
        // Update with a buffer that is multiple of 8 bytes
        byte[] buffer = [1, 2, 3, 4, 5, 6, 7, 8]; // 8 bytes - multiple of 8
        state.Update(buffer);
        
        // Call Result() which processes the final block
        _ = state.Result();
        
        // Attempting to update after Result() should throw
        byte[] secondBuffer = [9, 10, 11];
        var ex = Assert.Throws<InvalidOperationException>(() => state.Update(secondBuffer));
    }

    [Test]
    public void State64_MultipleUpdatesWithCompleteBlocks_Works()
    {
        // Create a state with valid key
        var state = new SipHash24.State64(key);
        
        // Multiple updates with buffers that are multiples of 8 should work
        byte[] buffer1 = [1, 2, 3, 4, 5, 6, 7, 8];       // 8 bytes
        byte[] buffer2 = [9, 10, 11, 12, 13, 14, 15, 16]; // 8 bytes
        
        // These should not throw
        Assert.DoesNotThrow(() => state.Update(buffer1));
        Assert.DoesNotThrow(() => state.Update(buffer2));
        
        // Result should work normally
        ulong result = 0;
        Assert.DoesNotThrow(() => result = state.Result());
        Assert.That(result, Is.Not.Zero); // Just verify we get some result
    }

    [Test]
    public void State64_InvalidKeyLength_ThrowsArgumentException()
    {
        // Test with key that's too short
        byte[] shortKey = [1, 2, 3, 4, 5];
        var ex = Assert.Throws<ArgumentException>(() => new SipHash24.State64(shortKey));
        
        // Test with key that's too long
        byte[] longKey = new byte[20];
        ex = Assert.Throws<ArgumentException>(() => new SipHash24.State64(longKey));
    }
}