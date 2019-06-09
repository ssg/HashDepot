using System;
using NUnit.Framework;

namespace HashDepot.Test
{
    [TestFixture]
    public class RequireTest
    {
        [Test]
        public void NotNull_NullArg_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Require.NotNull((object)null, "test"));
        }

        [Test]
        public void NotNull_NotNullArg_DoesNothing()
        {
            Assert.DoesNotThrow(() => Require.NotNull("test", "mest"));
        }

        [Test]
        public void NotNull_NullName_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Require.NotNull("test", null));
        }
    }
}
