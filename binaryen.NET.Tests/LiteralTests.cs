using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Binaryen.Tests
{
    [TestClass]
    public class LiteralTests
    {
        const int Int32 = 0x1234_5678;
        const long Int64 = 0x1234_5678_9ABC_DEF0;
        const float Float32 = float.MaxValue;
        const double Float64 = float.MaxValue;

        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get => testContextInstance;
            set => testContextInstance = value;
        }

        [TestMethod]
        public void TestInt32()
        {
            var literal = Literal.Int32(Int32);

            Assert.AreEqual(Type.Int32, literal.Type);
            Assert.AreEqual(Int32, literal.I32);
        }

        [TestMethod]
        public void TestInt64()
        {
            var literal = Literal.Int64(Int64);

            Assert.AreEqual(Type.Int64, literal.Type);
            Assert.AreEqual(Int64, literal.I64);
        }

        [TestMethod]
        public void TestFloat32()
        {
            var literal = Literal.Float32(Float32);

            Assert.AreEqual(Type.Float32, literal.Type);
            Assert.AreEqual(Float32, literal.F32);
        }

        [TestMethod]
        public void TestFloat64()
        {
            var literal = Literal.Float64(Float64);

            Assert.AreEqual(Type.Float64, literal.Type);
            Assert.AreEqual(Float64, literal.F64);
        }
    }
}
