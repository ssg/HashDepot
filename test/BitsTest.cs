// Copyright (c) 2015, 2016 Sedat Kapanoglu
// MIT License - see LICENSE file for details

using System;
using System.Buffers.Binary;
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
    public void PartialBytesToUInt64LE(ulong input, int len)
    {
        var buffer = new byte[8];
        BinaryPrimitives.WriteUInt64LittleEndian(buffer, input);
        Assert.That(Bits.PartialBytesToUInt64LE(buffer.AsSpan()[..len]), Is.EqualTo(input));
    }

    [Test]
    [TestCase(0xAABBCCDDU, 4)]
    [TestCase(0xAABBCCU, 3)]
    [TestCase(0xAABBU, 2)]
    [TestCase(0xAAU, 1)]
    public void PartialBytesToUInt32LE(uint input, int len)
    {
        var buffer = new byte[4];
        BinaryPrimitives.WriteUInt32LittleEndian(buffer, input);
        Assert.That(Bits.PartialBytesToUInt32LE(buffer.AsSpan()[..len]), Is.EqualTo(input));
    }

    [Test]
    [TestCase(0xAABBCCDDU, 4)]
    [TestCase(0xAABBCCU, 3)]
    [TestCase(0xAABBU, 2)]
    [TestCase(0xAAU, 1)]
    public void PartialBytesToUInt32LE_Array(uint input, int len)
    {
        var buffer = new byte[4];
        BinaryPrimitives.WriteUInt32LittleEndian(buffer, input);
        Assert.That(Bits.PartialBytesToUInt32LE(buffer.AsSpan()[..len]), Is.EqualTo(input));
    }
}