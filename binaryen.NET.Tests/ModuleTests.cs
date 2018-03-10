using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Binaryen.Tests
{
    [TestClass]
    public class ModuleTests
    {
        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get => testContextInstance;
            set => testContextInstance = value;
        }

        [TestMethod]
        public void TestCreateAndDispose()
        {
            var module = new Module();
            Assert.AreNotEqual(module.Handle, IntPtr.Zero);
            TestContext.WriteLine("Module.Handle = ${0:X}", module.Handle);

            module.Dispose();
            Assert.AreEqual(module.Handle, IntPtr.Zero);
            TestContext.WriteLine("Module.Handle = ${0:X}", module.Handle);
        }
    }
}
