using System;
using System.Runtime.InteropServices;

namespace Binaryen
{
    /// <summary>
    /// Represents a function type.
    /// </summary>
    public class Signature
    {
        private IntPtr handle;

        /// <summary>
        /// Initializes a new instance of the <see cref="Signature"/> class
        /// </summary>
        /// <param name="handle"></param>
        internal Signature(IntPtr handle)
        {
            this.handle = handle;
        }

        /// <summary>
        /// Gets the type of the specified parameter.
        /// </summary>
        /// <param name="index">The paramter index.</param>
        /// <returns>A <see cref="ValueType"/>.</returns>
        public ValueType this[uint index]
        {
            get => BinaryenFunctionTypeGetParam(handle, index);
        }

        /// <summary>
        /// Gets the name of the signature.
        /// </summary>
        public string Name
        {
            get
            {
                var ptr = BinaryenFunctionTypeGetName(handle);
                var str = Marshal.PtrToStringAnsi(ptr);
                return str;
            }
        }

        /// <summary>
        /// Gets the number of parameters of the signature.
        /// </summary>
        public uint ParameterCount => BinaryenFunctionTypeGetNumParams(handle);

        /// <summary>
        /// Gets the result type of the signature.
        /// </summary>
        public ValueType Result => BinaryenFunctionTypeGetResult(handle);

        /// <summary>
        /// Gets the handle of the signature.
        /// </summary>
        internal IntPtr Handle => handle;

        #region Imports

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenFunctionTypeGetName(IntPtr ftype); // NOTE: marshal value to string.

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint BinaryenFunctionTypeGetNumParams(IntPtr ftype);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern ValueType BinaryenFunctionTypeGetParam(IntPtr ftype, uint index);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern ValueType BinaryenFunctionTypeGetResult(IntPtr ftype);

        #endregion
    }
}
