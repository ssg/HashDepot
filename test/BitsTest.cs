// Copyright (c) 2015, 2016 Sedat Kapanoglu
// MIT License - see LICENSE file for details

using System;
using NUnit.Framework;

namespace HashDepot.Test;

[TestFixture]
public class BitsTest
{
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
}