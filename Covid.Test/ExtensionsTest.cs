using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Covid.Lib;

namespace Covid.Test
{
    [TestFixture]
    public class ExtensionsTest
    {
        [Test]
        public void TryToLongKMB_ShouldWork()
        {
            var ValidLong = "1000";
            Assert.AreEqual(ValidLong.TryToLongKMB(), "1K");
        }

    }
}
