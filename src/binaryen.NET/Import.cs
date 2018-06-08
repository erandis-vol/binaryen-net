using System;
using System.Runtime.InteropServices;

namespace Binaryen
{
    /// <summary>
    /// Represents a module import.
    /// </summary>
    public class Import : AutomaticBaseObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Import"/> class for the specified handle.
        /// </summary>
        /// <param name="handle">The handle to be managed.</param>
        public Import(IntPtr handle)
            : base(handle)
        { }

        /// <summary>
        /// Gets the type (kind) of the import.
        /// </summary>
        public ExternalType Type => BinaryenImportGetKind(Handle);

        /// <summary>
        /// Gets the external module name of the import.
        /// </summary>
        public string Module => Marshal.PtrToStringAnsi(BinaryenImportGetModule(Handle));

        /// <summary>
        /// Gets the external base name of the import.
        /// </summary>
        public string Base => Marshal.PtrToStringAnsi(BinaryenImportGetBase(Handle));

        /// <summary>
        /// Gets the internal name of the import.
        /// </summary>
        public string Name => Marshal.PtrToStringAnsi(BinaryenImportGetName(Handle));

        /// <summary>
        /// Gets the global type of the import.
        /// </summary>
        public ValueType GlobalType => BinaryenImportGetGlobalType(Handle);

        /// <summary>
        /// Gets the name of the function type of the import.
        /// </summary>
        public string FunctionType => Marshal.PtrToStringAnsi(BinaryenImportGetFunctionType(Handle));

        #region Imports

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern ExternalType BinaryenImportGetKind(IntPtr import);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern /*const char**/ IntPtr BinaryenImportGetModule(IntPtr import);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern /*const char**/ IntPtr BinaryenImportGetBase(IntPtr import);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern /*const char**/ IntPtr BinaryenImportGetName(IntPtr import);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern ValueType BinaryenImportGetGlobalType(IntPtr import);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern /*const char**/ IntPtr BinaryenImportGetFunctionType(IntPtr import);

        #endregion
    }
}
