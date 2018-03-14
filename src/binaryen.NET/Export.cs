using System;
using System.Runtime.InteropServices;

namespace Binaryen
{
    /// <summary>
    /// Represents a module export.
    /// </summary>
    public class Export
    {
        private IntPtr handle;

        public Export(IntPtr handle)
        {
            this.handle = handle;
        }

        /// <summary>
        /// Gets the type (kind) of the export.
        /// </summary>
        public ExternalType Type => BinaryenExportGetKind(handle);

        /// <summary>
        /// Gets the external name of the export.
        /// </summary>
        public string Name => Marshal.PtrToStringAnsi(BinaryenExportGetName(handle));

        /// <summary>
        /// Gets the internal name of the export.
        /// </summary>
        public string Value => Marshal.PtrToStringAnsi(BinaryenExportGetValue(handle));

        #region Imports

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern ExternalType BinaryenExportGetKind(IntPtr export_);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern /*const char**/ IntPtr BinaryenExportGetName(IntPtr export_);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern /*const char**/ IntPtr BinaryenExportGetValue(IntPtr export_);

        #endregion
    }
}
