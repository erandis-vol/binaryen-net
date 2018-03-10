using System;
using System.Runtime.InteropServices;

namespace Binaryen
{
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
        private IntPtr handle;

        /// <summary>
        /// Initializes a new instance of the <see cref="Expression"/> class with the specified handle.
        /// </summary>
        /// <param name="handle">The handle.</param>
        internal Expression(IntPtr handle)
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
        /// Gets information about the expression.
        /// </summary>
        public ExpressionInfo Info
        {
            get
            {
                var id = BinaryenExpressionGetId(handle);
                var type = BinaryenExpressionGetType(handle);
                
                switch (id)
                {
                    case ExpressionId.Block:
                        {
                            var count = BinaryenBlockGetNumChildren(handle);
                            var children = new Expression[count];

                            for (uint i = 0; i < count; i++)
                                children[i] = new Expression(BinaryenBlockGetChild(handle, i));

                            return new BlockInfo(BinaryenBlockGetName(handle), children, type);
                        }

                    case ExpressionId.If:
                        return new IfInfo(
                            new Expression(BinaryenIfGetCondition(handle)),
                            new Expression(BinaryenIfGetIfTrue(handle)),
                            new Expression(BinaryenIfGetIfFalse(handle)),
                            type
                        );

                    // TODO

                    case ExpressionId.Nop:
                        return new NopInfo(type);

                    case ExpressionId.Unary:
                        return new UnreachableInfo(type);

                    // TODO

                    default:
                        throw new NotSupportedException($"Unexpected function ID: {id}");
                }
            }
        }

        /// <summary>
        /// Gets the handle of the expression.
        /// </summary>
        internal IntPtr Handle => handle;

        #region Imports

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern ExpressionId BinaryenExpressionGetId(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern Type BinaryenExpressionGetType(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern void BinaryenExpressionPrint(IntPtr expr);

        // Block

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern string BinaryenBlockGetName(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint BinaryenBlockGetNumChildren(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenBlockGetChild(IntPtr expr, uint index);

        // If

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenIfGetCondition(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenIfGetIfTrue(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenIfGetIfFalse(IntPtr expr);

        #endregion
    }
}
