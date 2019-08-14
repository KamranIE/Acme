using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Acme.UI.Helper.Extensions;

namespace Acme.Test.HelpersTests.ExtensionsTest
{
    /// <summary>
    /// Summary description for StringExtensionTest
    /// </summary>
    [TestClass]
    public class StringExtensionTest
    {
        [TestMethod]
        public void GivenNullStringTestIsNullExpectTrue()
        {
            // prespare data
            string str = null;

            // prepare target
            // invoke
            var result = str.IsNull();

            // validate
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GivenNotNullStringTestIsNullExpectFalse()
        {
            // prespare data
            string str = "SomeValue";

            // prepare target
            // invoke
            var result = str.IsNull();

            // validate
            Assert.IsFalse(result);
        }
    }
}
