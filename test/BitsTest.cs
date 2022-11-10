using System;
using NUnit.Framework;

namespace HashDepot.Test;

[TestFixture]
public class BitsTest
{
    [Test]
    [TestCase(0xAABBCCDDEEFF1122u, 8, 0xBBCCDDEEFF1122AAu)]
    [TestCase(0x1122334455667788u, 16, 0x3344556677881122u)]
    [TestCase(0xAABBCCDDEEFF1122u, 64, 0xAABBCCDDEEFF1122u)]
    [TestCase(0xAABBCCDDEEFF1122u, 0, 0xAABBCCDDEEFF1122u)]
    public void RotateLeft_Ulong(ulong value, int bits, ulong expectedResult)
    {
        Assert.That(Bits.RotateLeft(value, bits), Is.EqualTo(expectedResult));
    }

    [Test]
    [TestCase(0x1122334455667788u, 8, 0x8811223344556677u)]
    [TestCase(0x1122334455667788u, 16, 0x7788112233445566u)]
    [TestCase(0x1122334455667788u, 64, 0x1122334455667788u)]
    [TestCase(0x1122334455667788u, 0, 0x1122334455667788u)]
    public void RotateRight_Ulong(ulong value, int bits, ulong expectedResult)
    {
        Assert.That(Bits.RotateRight(value, bits), Is.EqualTo(expectedResult));
    }

    [Test]
    [TestCase(0xAABBCCDDU, 8, 0xBBCCDDAAU)]
    [TestCase(0xAABBCCDDU, 32, 0xAABBCCDDU)]
    [TestCase(0xAABBCCDDU, 0, 0xAABBCCDDU)]
    public void RotateLeft_Uint(uint value, int bits, uint expectedResult)
    {
        Assert.That(Bits.RotateLeft(value, bits), Is.EqualTo(expectedResult));
    }

    [Test]
    [TestCase(0xAABBCCDDU, 8, 0xDDAABBCCU)]
    [TestCase(0xAABBCCDDU, 32, 0xAABBCCDDU)]
    [TestCase(0xAABBCCDDU, 0, 0xAABBCCDDU)]
    public void RotateRight_Uint(uint value, int bits, uint expectedResult)
    {
        Assert.That(Bits.RotateRight(value, bits), Is.EqualTo(expectedResult));
    }

    [Test]
    [TestCase(0xAABBCCDDEEFF11U, 7)]
    [TestCase(0xAABBCCDDEEFFU, 6)]
    [TestCase(0xAABBCCDDEEU, 5)]
    [TestCase(0xAABBCCDDU, 4)]
    [TestCase(0xAABBCCU, 3)]
    [TestCase(0xAABBU, 2)]
    [TestCase(0xAAU, 1)]
    public void PartialBytesToUInt64(ulong input, int len)
    {
        var buffer = BitConverter.GetBytes(input);
        Assert.That(Bits.PartialBytesToUInt64(buffer.AsSpan()[..len]), Is.EqualTo(input));
    }

    [Test]
    [TestCase(0xAABBCCDDU, 4)]
    [TestCase(0xAABBCCU, 3)]
    [TestCase(0xAABBU, 2)]
    [TestCase(0xAAU, 1)]
    public void PartialBytesToUInt32(uint input, int len)
    {
        var buffer = BitConverter.GetBytes(input).AsSpan();
        Assert.That(Bits.PartialBytesToUInt32(buffer[..len]), Is.EqualTo(input));
    }

    [Test]
    [TestCase(0xAABBCCDDU, 4)]
    [TestCase(0xAABBCCU, 3)]
    [TestCase(0xAABBU, 2)]
    [TestCase(0xAAU, 1)]
    public void PartialBytesToUInt32_Array(uint input, int len)
    {
        var buffer = BitConverter.GetBytes(input).AsSpan();
        Assert.That(Bits.PartialBytesToUInt32(buffer[..len]), Is.EqualTo(input));
    }

    [Test]
    [TestCase(0x11223344u, 0x44332211u)]
    [TestCase(0xAABBCCDDu, 0xDDCCBBAAu)]
    public void SwapBytes32(uint input, uint output)
    {
        uint result = Bits.SwapBytes32(input);
        Assert.That(result, Is.EqualTo(output), $"result ({result:x8}) != expected ({output:x8})");
    }

    [Test]
    [TestCase(0x1122334455667788u, 0x8877665544332211u)]
    [TestCase(0xAABBCCDDEEFF1122u, 0x2211FFEEDDCCBBAAu)]
    public void SwapBytes64(ulong input, ulong output)
    {
        ulong result = Bits.SwapBytes64(input);
        Assert.That(result, Is.EqualTo(output), $"result ({result:x16}) != expected ({output:x16})");
    }
}