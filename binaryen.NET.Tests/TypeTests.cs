using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Binaryen.Tests
{
    [TestClass]
    public class TypeTests
    {
        const uint None = 0;
        const uint Int32 = 1;
        const uint Int64 = 2;
        const uint Float32 = 3;
        const uint Float64 = 4;
        const uint Unreachable = 5;
        const uint Auto = 0xFFFFFFFF; // (uint32_t)(-1)

        [TestMethod]
        public void TestNone()
        {
            Assert.AreEqual(Type.None, None);
        }

        [TestMethod]
        public void TestInt32()
        {
            Assert.AreEqual(Type.Int32, Int32);
        }

        [TestMethod]
        public void TestInt64()
        {
            Assert.AreEqual(Type.Int64, Int64);
        }

        [TestMethod]
        public void TestFloat32()
        {
            Assert.AreEqual(Type.Float32, Float32);
        }

        [TestMethod]
        public void TestFloat64()
        {
            Assert.AreEqual(Type.Float64, Float64);
        }

        [TestMethod]
        public void TestUnreachable()
        {
            Assert.AreEqual(Type.Unreachable, Unreachable);
        }

        [TestMethod]
        public void TestAuto()
        {
            Assert.AreEqual(Type.Auto, Auto);
        }
    }
}
