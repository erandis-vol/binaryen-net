using System;
using System.Runtime.InteropServices;

namespace Binaryen
{
    /// <summary>
    /// Represents a function.
    /// </summary>
    public class Function
    {
        private IntPtr handle;

        internal Function(IntPtr handle)
        {
            this.handle = handle;
        }

        /// <summary>
        /// Gets the name of the function.
        /// </summary>
        public string Name => Marshal.PtrToStringAnsi(BinaryenFunctionGetName(handle));

        /// <summary>
        /// Gets the handle of the function.
        /// </summary>
        internal IntPtr Handle => handle;

        #region Imports

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenFunctionGetName(IntPtr func);

        #endregion
    }
}
