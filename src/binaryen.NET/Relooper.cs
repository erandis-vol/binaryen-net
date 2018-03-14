using System;
using System.Runtime.InteropServices;

namespace Binaryen
{
    /// <summary>
    /// Represents an optimized implementation of the Relooper algorithm.
    /// </summary>
    /// <remarks>
    /// <para>General usage is (1) create a relooper, (2) create blocks, (3) add branches between them, (4) render the output.</para>
    /// <para>See Relooper.h for the original implementation and further details.</para>
    /// </remarks>
    public class Relooper
    {
        private IntPtr handle;

        /// <summary>
        /// Initializes a new instance of the <see cref="Relooper"/> class.
        /// </summary>
        public Relooper()
        {
            handle = RelooperCreate();
            if (handle == IntPtr.Zero)
                throw new OutOfMemoryException();
        }

        public RelooperBlock AddBlock(Expression code)
        {
            var blockRef = RelooperAddBlock(handle, code.Handle);
            if (blockRef == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new RelooperBlock(blockRef);
        }

        public void AddBranch(RelooperBlock from, RelooperBlock to, Expression condition, Expression code)
        {
            RelooperAddBranch(from.Handle, to.Handle, condition.Handle, code.Handle);
        }

        public RelooperBlock AddBlockWithSwitch(Expression code, Expression condition)
        {
            var blockRef = RelooperAddBlockWithSwitch(handle, code.Handle, condition.Handle);
            if (blockRef == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new RelooperBlock(blockRef);
        }

        public void AddBranchForSwitch(RelooperBlock from, RelooperBlock to, uint[] indexes, Expression code)
        {
            RelooperAddBranchForSwitch(from.Handle, to.Handle, indexes, (uint)indexes.Length, code.Handle);
        }

        /// <summary>
        /// Renders and cleans up the <see cref="Relooper"/> instance.
        /// </summary>
        /// <param name="entry">The entry block.</param>
        /// <param name="labelHelper">The label helper variable.</param>
        /// <param name="module">The parent module.</param>
        /// <returns>An <see cref="Expression"/> instance.</returns>
        /// <exception cref="OutOfMemoryException">the expression could not be created.</exception>
        public Expression RenderAndDispose(RelooperBlock entry, uint labelHelper, Module module)
        {
            var exprPtr = RelooperRenderAndDispose(handle, entry.Handle, labelHelper, module.Handle);
            if (exprPtr == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(exprPtr);
        }

        #region Imports

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr RelooperCreate();

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr RelooperAddBlock(IntPtr relooper, IntPtr code);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern void RelooperAddBranch(IntPtr from, IntPtr to, IntPtr condition, IntPtr code);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr RelooperAddBlockWithSwitch(IntPtr relooper, IntPtr code, IntPtr condition);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern void RelooperAddBranchForSwitch(IntPtr from, IntPtr to, uint[] indexes, uint numIndexes, IntPtr code);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr RelooperRenderAndDispose(IntPtr relooper, IntPtr entry, uint labelHelper, IntPtr module);

        #endregion
    }

    /// <summary>
    /// Represents a relooper block.
    /// </summary>
    public class RelooperBlock
    {
        private IntPtr handle;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelooperBlock"/> class.
        /// </summary>
        /// <param name="handle"></param>
        public RelooperBlock(IntPtr handle)
        {
            this.handle = handle;
        }

        /// <summary>
        /// Gets the block handle.
        /// </summary>
        public IntPtr Handle => handle;
    }
}
