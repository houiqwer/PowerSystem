using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerSystemLibrary.Util;
using System;

namespace PowerSystem.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string password = new BaseUtil().BuildPassword("电工1", "888888");
        }
    }
}
