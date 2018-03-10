using System;
using System.Runtime.InteropServices;

namespace Binaryen
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Literal
    {
        [FieldOffset(0)]
        public Type Type;

        [FieldOffset(8)]
        public int I32;
        
        [FieldOffset(8)]
        public long I64;
        
        [FieldOffset(8)]
        public float F32;

        [FieldOffset(8)]
        public double F64;

        public static Literal Int32(int x)
        {
            return BinaryenLiteralInt32(x);
        }

        public static Literal Int64(long x)
        {
            return BinaryenLiteralInt64(x);
        }

        public static Literal Float32(float x)
        {
            return BinaryenLiteralFloat32(x);
        }

        public static Literal Float64(double x)
        {
            return BinaryenLiteralFloat64(x);
        }

        public static Literal Float32Bits(int x)
        {
            return BinaryenLiteralFloat32Bits(x);
        }

        public static Literal Float64Bits(long x)
        {
            return BinaryenLiteralFloat64Bits(x);
        }

        #region Imports

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern Literal BinaryenLiteralInt32(int x);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern Literal BinaryenLiteralInt64(long x);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern Literal BinaryenLiteralFloat32(float x);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern Literal BinaryenLiteralFloat64(double x);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern Literal BinaryenLiteralFloat32Bits(int x);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern Literal BinaryenLiteralFloat64Bits(long x);

        #endregion
    }
}
