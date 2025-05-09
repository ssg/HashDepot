using System;
using NUnit.Framework;

namespace HashDepot.Test;

[TestFixture]
public class ChecksumTest
{
    static readonly byte[] array = new byte[1001 * 1003];

    [Test]
    public void Test()
    {
        Assert.DoesNotThrow(() => Checksum.Hash32(array.AsSpan()));
    }
}
