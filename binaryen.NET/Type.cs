using System;
using System.Runtime.InteropServices;

namespace Binaryen
{
    public static class Type
    {
        /// <summary>No type.</summary>
        public static readonly uint None;
        /// <summary>32-bit integer.</summary>
        public static readonly uint Int32;
        /// <summary>64-bit integer.</summary>
        public static readonly uint Int64;
        /// <summary>32-bit floating point number.</summary>
        public static readonly uint Float32;
        /// <summary>64-bit floating point number.</summary>
        public static readonly uint Float64;
        /// <summary>Special type. Used to indicate unreachable code.</summary>
        public static readonly uint Unreachable;
        /// <summary>Special type. Used as the type of a block to allow the API determine it automatically.</summary>
        public static readonly uint Auto;

        static Type()
        {
            None = BinaryenTypeNone();
            Int32 = BinaryenTypeInt32();
            Int64 = BinaryenTypeInt64();
            Float32 = BinaryenTypeFloat32();
            Float64 = BinaryenTypeFloat64();
            Unreachable = BinaryenTypeUnreachable();
            Auto = BinaryenTypeAuto();
        }

        #region Imports

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint BinaryenTypeNone();

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint BinaryenTypeInt32();

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint BinaryenTypeInt64();

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint BinaryenTypeFloat32();

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint BinaryenTypeFloat64();

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint BinaryenTypeUnreachable();

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint BinaryenTypeAuto();

        #endregion
    }
}
