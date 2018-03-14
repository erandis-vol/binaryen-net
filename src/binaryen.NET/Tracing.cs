using System.Runtime.InteropServices;

namespace Binaryen
{
    /// <summary>
    /// Provides methods for controlling API tracing.
    /// </summary>
    public static class Tracing
    {
        /// <summary>
        /// Enables API tracing. When enabled, each call to an API method will print out C code
        /// equivalent to it, which is useful for autogenerating testcases from projects using the API.
        /// </summary>
        public static void Enable() => BinaryenSetAPITracing(1);

        /// <summary>
        /// Disables API tracing.
        /// </summary>
        public static void Disable() => BinaryenSetAPITracing(0);

        #region Imports

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern void BinaryenSetAPITracing(int on);

        #endregion
    }
}
