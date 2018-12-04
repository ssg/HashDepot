using System;
using NUnit.Framework;

namespace HashDepot.Test
{
    [TestFixture]
    class RequireTest
    {
        [Test]
        public void NotNull_NullValues_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => Require.NotNull((string)null, "test"));
        }

        [Test]
        public void NotNull_NonNullValues_Ignored()
        {
            Assert.DoesNotThrow(() => Require.NotNull("allah", "test"));
        }

        [Test]
        public void NotNull_NullValues_ShowParameterName()
        {
            try
            {
                Require.NotNull((string)null, "test");
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual("test", e.ParamName);
            }
        }
    }
}