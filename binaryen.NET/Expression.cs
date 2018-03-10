using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Binaryen
{
    using ExpressionRef = IntPtr;

    /// <summary>
    /// Represents the ID (kind) of an expression.
    /// </summary>
    public enum ExpressionId
    {
        Invalid,
        Block,
        If,
        Loop,
        Break,
        Switch,
        Call,
        CallImport,
        CallIndirect,
        GetLocal,
        SetLocal,
        GetGlobal,
        SetGlobal,
        Load,
        Store,
        Const,
        Unary,
        Binary,
        Select,
        Drop,
        Return,
        Host,
        Nop,
        Unreachable,
        AtomicCmpxchg,
        AtomicRMW,
        AtomicWait,
        AtomicWake,
    }

    public class Expression
    {
        private ExpressionRef handle;

        /// <summary>
        /// Initializes a new instance of the <see cref="Expression"/> class with the specified handle.
        /// </summary>
        /// <param name="handle">The handle.</param>
        internal Expression(ExpressionRef handle)
        {
            this.handle = handle;
        }

        /// <summary>
        /// Prints the expression to STDOUT. Useful for debugging.
        /// </summary>
        public void Print()
        {
            BinaryenExpressionPrint(handle);
        }

        /// <summary>
        /// Gets the ID (kind) of the expression.
        /// </summary>
        public ExpressionId ID => BinaryenExpressionGetId(handle);

        /// <summary>
        /// Gets the type of the expression.
        /// </summary>
        public Type Type => BinaryenExpressionGetType(handle);

        /// <summary>
        /// Gets the name of the expression.
        /// </summary>
        public virtual string Name => string.Empty;

        /// <summary>
        /// Gets the handle of the expression.
        /// </summary>
        internal ExpressionRef Handle => handle;

        #region Imports

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern ExpressionId BinaryenExpressionGetId(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern Type BinaryenExpressionGetType(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern void BinaryenExpressionPrint(IntPtr expr);

        #endregion
    }
}
