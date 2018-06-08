using System;
using System.Runtime.InteropServices;

namespace Binaryen
{
    /// <summary>
    /// Represents a module export.
    /// </summary>
    public class Export : AutomaticBaseObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Export"/> class for the specified handle.
        /// </summary>
        /// <param name="handle">The handle to be managed.</param>
        public Export(IntPtr handle)
            : base(handle)
        { }

        /// <summary>
        /// Gets the type (kind) of the export.
        /// </summary>
        public ExternalType Type => BinaryenExportGetKind(Handle);

        /// <summary>
        /// Gets the external name of the export.
        /// </summary>
        public string Name => Marshal.PtrToStringAnsi(BinaryenExportGetName(Handle));

        /// <summary>
        /// Gets the internal name of the export.
        /// </summary>
        public string Value => Marshal.PtrToStringAnsi(BinaryenExportGetValue(Handle));

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
