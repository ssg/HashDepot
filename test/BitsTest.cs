using System;
using NUnit.Framework;

namespace HashDepot.Test
{
    [TestFixture]
    public class BitsTest
    {
        [Test]
        [TestCase(0xAABBCCDDEEFF1122U, 8, 0xBBCCDDEEFF1122AAU)]
        [TestCase(0xAABBCCDDEEFF1122U, 64, 0xAABBCCDDEEFF1122U)]
        [TestCase(0xAABBCCDDEEFF1122U, 0, 0xAABBCCDDEEFF1122U)]
        public void RotateLeft_Ulong(ulong value, int bits, ulong expectedResult)
        {
            Assert.AreEqual(expectedResult, Bits.RotateLeft(value, bits));
        }

        [Test]
        [TestCase(0xAABBCCDDU, 8, 0xBBCCDDAAU)]
        [TestCase(0xAABBCCDDU, 32, 0xAABBCCDDU)]
        [TestCase(0xAABBCCDDU, 0, 0xAABBCCDDU)]
        public void RotateLeft_Uint(uint value, int bits, uint expectedResult)
        {
            Assert.AreEqual(expectedResult, Bits.RotateLeft(value, bits));
        }

        [Test]
        [TestCase(0xAABBCCDDU, 8, 0xDDAABBCCU)]
        [TestCase(0xAABBCCDDU, 32, 0xAABBCCDDU)]
        [TestCase(0xAABBCCDDU, 0, 0xAABBCCDDU)]
        public void RotateRight_Uint(uint value, int bits, uint expectedResult)
        {
            Assert.AreEqual(expectedResult, Bits.RotateRight(value, bits));
        }

        [Test]
        [TestCase(0xAABBCCDDEEFF11U, 7)]
        [TestCase(0xAABBCCDDEEFFU, 6)]
        [TestCase(0xAABBCCDDEEU, 5)]
        [TestCase(0xAABBCCDDU, 4)]
        [TestCase(0xAABBCCU, 3)]
        [TestCase(0xAABBU, 2)]
        [TestCase(0xAAU, 1)]
        public unsafe void PartialBytesToUInt64(ulong input, int len)
        {
            var buffer = BitConverter.GetBytes(input);
            var testBuffer = new byte[len];
            Array.Copy(buffer, testBuffer, len);
            fixed (byte* bufPtr = testBuffer)
            {
                Assert.AreEqual(input, Bits.PartialBytesToUInt64(bufPtr, len));
            }
        }

        [Test]
        [TestCase(0xAABBCCDDU, 4)]
        [TestCase(0xAABBCCU, 3)]
        [TestCase(0xAABBU, 2)]
        [TestCase(0xAAU, 1)]
        public unsafe void PartialBytesToUInt32(uint input, int len)
        {
            var buffer = BitConverter.GetBytes(input);
            var testBuffer = new byte[len];
            Array.Copy(buffer, testBuffer, len);
            fixed (byte* bufPtr = buffer)
            {
                Assert.AreEqual(input, Bits.PartialBytesToUInt32(bufPtr, len));
            }
        }
    }
}