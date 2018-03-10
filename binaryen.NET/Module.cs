using System;
using System.Runtime.InteropServices;

namespace Binaryen
{
    public class Module : IDisposable
    {
        private IntPtr handle;

        public Module()
        {
            handle = BinaryenModuleCreate();
        }

        ~Module()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (handle != IntPtr.Zero)
            {
                BinaryenModuleDispose(handle);
                handle = IntPtr.Zero;
            }
        }

        public IntPtr Handle => handle;

        #region Imports

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenModuleCreate();

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern void BinaryenModuleDispose(IntPtr handle);

        #endregion
    }
}
