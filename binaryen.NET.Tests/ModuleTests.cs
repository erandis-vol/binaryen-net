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
            Assert.AreNotEqual(IntPtr.Zero, module.Handle);

            module.Dispose();
            Assert.AreEqual(IntPtr.Zero, module.Handle);
        }

        [TestMethod]
        public void TestAddFunctionType()
        {
            using (var module = new Module())
            {
                var signature = module.AddFunctionType("testMethod", Type.Int32, new[] { Type.Float32, Type.Int64 });

                Assert.AreEqual("testMethod", signature.Name);
                Assert.AreEqual(2u, signature.ParameterCount);
                Assert.AreEqual(Type.Float32, signature[0]);
                Assert.AreEqual(Type.Int64, signature[1]);
            }
        }
    }
}
