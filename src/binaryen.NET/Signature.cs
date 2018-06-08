using System;
using System.Runtime.InteropServices;

namespace Binaryen
{
    /// <summary>
    /// Represents a function type.
    /// </summary>
    public class Signature : AutomaticBaseObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Signature"/> class for the specified handle.
        /// </summary>
        /// <param name="handle">The handle to be managed.</param>
        public Signature(IntPtr handle)
            : base(handle)
        { }

        /// <summary>
        /// Gets the type of the specified parameter.
        /// </summary>
        /// <param name="index">The paramter index.</param>
        /// <returns>A <see cref="ValueType"/>.</returns>
        public ValueType this[uint index]
        {
            get => BinaryenFunctionTypeGetParam(Handle, index);
        }

        /// <summary>
        /// Gets the name of the signature.
        /// </summary>
        public string Name
        {
            get
            {
                var ptr = BinaryenFunctionTypeGetName(Handle);
                var str = Marshal.PtrToStringAnsi(ptr);
                return str;
            }
        }

        /// <summary>
        /// Gets the number of parameters of the signature.
        /// </summary>
        public uint ParameterCount => BinaryenFunctionTypeGetNumParams(Handle);

        /// <summary>
        /// Gets the result type of the signature.
        /// </summary>
        public ValueType Result => BinaryenFunctionTypeGetResult(Handle);

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
