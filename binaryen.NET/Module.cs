using System;
using System.Runtime.InteropServices;

namespace Binaryen
{
    public class Module : IDisposable
    {
        private IntPtr handle;

        /// <summary>
        /// Initializes a new instance of the <see cref="Module"/> class.
        /// </summary>
        public Module()
        {
            handle = BinaryenModuleCreate();
            if (handle == IntPtr.Zero)
                throw new OutOfMemoryException();
        }

        ~Module()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases all resources used by this <see cref="Module"/>.
        /// </summary>
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

        public Signature AddFunctionType(string name, Type result, Type[] parameters)
        {
            var sig = BinaryenAddFunctionType(handle, name, result, parameters, (uint)parameters.Length);
            if (sig == IntPtr.Zero)
            {
                throw new OutOfMemoryException();
            }

            return new Signature(sig);
        }

        public Signature GetFunctionTypeBySignature(Type result, Type[] parameters)
        {
            var sig = BinaryenGetFunctionTypeBySignature(handle, result, parameters, (uint)parameters.Length);
            if (sig == IntPtr.Zero)
            {
                return null;
            }

            return new Signature(sig);
        }

        /// <summary>
        /// Gets the handle of the module.
        /// </summary>
#if DEBUG
        public
#else
            internal
#endif
            IntPtr Handle => handle;

        #region Imports

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenModuleCreate();

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern void BinaryenModuleDispose(IntPtr handle);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr BinaryenAddFunctionType(IntPtr module, string name, Type result, Type[] paramTypes, uint numParams);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenGetFunctionTypeBySignature(IntPtr module, Type result, Type[] paramTypes, uint numParams);

        #endregion
    }
}
