using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Binaryen.Tests
{
    [TestClass]
    public class ModuleTests
    {
        [TestMethod]
        public void TestCreateAndDestroy()
        {
            var module = new Module();
            module.Dispose();
        }
    }
}
