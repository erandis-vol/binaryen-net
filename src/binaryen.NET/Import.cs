using System;
using System.Runtime.InteropServices;

namespace Binaryen
{
    /// <summary>
    /// Represents a module import.
    /// </summary>
    public class Import
    {
        private IntPtr handle;

        public Import(IntPtr handle)
        {
            this.handle = handle;
        }

        /// <summary>
        /// Gets the type (kind) of the import.
        /// </summary>
        public ExternalType Type => BinaryenImportGetKind(handle);

        /// <summary>
        /// Gets the external module name of the import.
        /// </summary>
        public string Module => Marshal.PtrToStringAnsi(BinaryenImportGetModule(handle));

        /// <summary>
        /// Gets the external base name of the import.
        /// </summary>
        public string Base => Marshal.PtrToStringAnsi(BinaryenImportGetBase(handle));

        /// <summary>
        /// Gets the internal name of the import.
        /// </summary>
        public string Name => Marshal.PtrToStringAnsi(BinaryenImportGetName(handle));

        /// <summary>
        /// Gets the global type of the import.
        /// </summary>
        public ValueType GlobalType => BinaryenImportGetGlobalType(handle);

        /// <summary>
        /// Gets the name of the function type of the import.
        /// </summary>
        public string FunctionType => Marshal.PtrToStringAnsi(BinaryenImportGetFunctionType(handle));

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
