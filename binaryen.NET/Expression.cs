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
        public ValueType Type => BinaryenExpressionGetType(handle);

        /// <summary>
        /// Gets information about the expression.
        /// </summary>
        public ExpressionInfo GetInfo()
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

                        return new BlockInfo(Marshal.PtrToStringAnsi(BinaryenBlockGetName(handle)), children, type);
                    }

                case ExpressionId.If:
                    return new IfInfo(
                        new Expression(BinaryenIfGetCondition(handle)),
                        new Expression(BinaryenIfGetIfTrue(handle)),
                        new Expression(BinaryenIfGetIfFalse(handle)),
                        type
                    );

                case ExpressionId.Loop:
                    {
                        var name = Marshal.PtrToStringAnsi(BinaryenLoopGetName(handle));
                        var body = new Expression(BinaryenLoopGetBody(handle));

                        return new LoopInfo(name, body, type);
                    }

                case ExpressionId.Break:
                    {
                        var label = Marshal.PtrToStringAnsi(BinaryenBreakGetName(handle));
                        var condition = new Expression(BinaryenBreakGetCondition(handle));
                        var value = new Expression(BinaryenBreakGetValue(handle));

                        return new BreakInfo(label, condition, value, type);
                    }

                case ExpressionId.Switch:
                    {
                        var labelCount = BinaryenSwitchGetNumNames(handle);
                        var labels = new string[labelCount];

                        for (uint i = 0; i < labelCount; i++)
                            labels[i] = Marshal.PtrToStringAnsi(BinaryenSwitchGetName(handle, i));

                        var defaultLabel = Marshal.PtrToStringAnsi(BinaryenSwitchGetDefaultName(handle));

                        var condition = new Expression(BinaryenSwitchGetCondition(handle));
                        var value = new Expression(BinaryenSwitchGetValue(handle));

                        return new SwitchInfo(labels, defaultLabel, condition, value, type);
                    }

                case ExpressionId.Call:
                    {
                        var target = Marshal.PtrToStringAnsi(BinaryenCallGetTarget(handle));

                        var operandCount = BinaryenCallGetNumOperands(handle);
                        var operands = new Expression[operandCount];

                        for (uint i = 0; i < operandCount; i++)
                            operands[i] = new Expression(BinaryenCallGetOperand(handle, i));

                        return new CallInfo(target, operands, type);
                    }

                case ExpressionId.CallImport:
                    {
                        var target = Marshal.PtrToStringAnsi(BinaryenCallImportGetTarget(handle));

                        var operandCount = BinaryenCallImportGetNumOperands(handle);
                        var operands = new Expression[operandCount];

                        for (uint i = 0; i < operandCount; i++)
                            operands[i] = new Expression(BinaryenCallImportGetOperand(handle, i));

                        return new CallImportInfo(target, operands, type);
                    }

                case ExpressionId.CallIndirect:
                    {
                        var target = new Expression(BinaryenCallIndirectGetTarget(handle));

                        var operandCount = BinaryenCallIndirectGetNumOperands(handle);
                        var operands = new Expression[operandCount];

                        for (uint i = 0; i < operandCount; i++)
                            operands[i] = new Expression(BinaryenCallIndirectGetOperand(handle, i));

                        return new CallIndirectInfo(target, operands, type);
                    }

                case ExpressionId.GetLocal:
                    return new GetLocalInfo(BinaryenGetLocalGetIndex(handle), type);

                case ExpressionId.SetLocal:
                    {
                        var index = BinaryenSetLocalGetIndex(handle);
                        var isTee = BinaryenSetLocalIsTee(handle);
                        var value = new Expression(BinaryenSetLocalGetValue(handle));

                        return new SetLocalInfo(index, isTee > 0, value, type);
                    }

                case ExpressionId.GetGlobal:
                    return new GetGlobalInfo(Marshal.PtrToStringAnsi(BinaryenGetGlobalGetName(handle)), type);

                case ExpressionId.SetGlobal:
                    {
                        var name = Marshal.PtrToStringAnsi(BinaryenSetGlobalGetName(handle));
                        var value = new Expression(BinaryenSetGlobalGetValue(handle));

                        return new SetGlobalInfo(name, value, type);
                    }

                case ExpressionId.Load:
                    {
                        var isAtomic = BinaryenLoadIsAtomic(handle);
                        var isSigned = BinaryenLoadIsSigned(handle);
                        var offset = BinaryenLoadGetOffset(handle);
                        var bytes = BinaryenLoadGetBytes(handle);
                        var align = BinaryenLoadGetAlign(handle);
                        var ptr = new Expression(BinaryenLoadGetPtr(handle));

                        return new LoadInfo(isAtomic > 0, isSigned > 0, offset, bytes, align, ptr, type);
                    }

                case ExpressionId.Store:
                    {
                        var isAtomic = BinaryenStoreIsAtomic(handle);
                        var offset = BinaryenStoreGetOffset(handle);
                        var bytes = BinaryenStoreGetBytes(handle);
                        var align = BinaryenStoreGetAlign(handle);
                        var ptr = new Expression(BinaryenStoreGetPtr(handle));
                        var value = new Expression(BinaryenStoreGetValue(handle));

                        return new StoreInfo(isAtomic > 0, offset, bytes, align, ptr, value, type);
                    }

                case ExpressionId.Const:
                    {
                        Literal value;

                        switch (type)
                        {
                            case ValueType.Int32:
                                value = new Literal { I32 = BinaryenConstGetValueI32(handle), Type = ValueType.Int32 };
                                break;

                            case ValueType.Int64:
                                value = new Literal { I64 = BinaryenConstGetValueI64(handle), Type = ValueType.Int64 };
                                break;

                            case ValueType.Float32:
                                value = new Literal { F32 = BinaryenConstGetValueF32(handle), Type = ValueType.Float32 };
                                break;

                            case ValueType.Float64:
                                value = new Literal { F64 = BinaryenConstGetValueF64(handle), Type = ValueType.Float64 };
                                break;

                            default:
                                throw new Exception($"Unexpected Const type: {type}");
                        }

                        return new ConstInfo(value, type);
                    }

                case ExpressionId.Unary:
                    return new UnaryInfo(
                        BinaryenUnaryGetOp(handle),
                        new Expression(BinaryenUnaryGetValue(handle)),
                        type
                    );

                case ExpressionId.Binary:
                    return new BinaryInfo(
                        BinaryenBinaryGetOp(handle),
                        new Expression(BinaryenBinaryGetLeft(handle)),
                        new Expression(BinaryenBinaryGetRight(handle)),
                        type
                    );

                case ExpressionId.Select:
                    return new SelectInfo(
                        new Expression(BinaryenSelectGetCondition(handle)),
                        new Expression(BinaryenSelectGetIfTrue(handle)),
                        new Expression(BinaryenSelectGetIfFalse(handle)),
                        type
                    );

                case ExpressionId.Drop:
                    return new DropInfo(new Expression(BinaryenDropGetValue(handle)), type);

                case ExpressionId.Return:
                    return new ReturnInfo(new Expression(BinaryenReturnGetValue(handle)), type);

                case ExpressionId.Host:
                    {
                        var name = Marshal.PtrToStringAnsi(BinaryenHostGetNameOperand(handle));

                        var operandCount = BinaryenHostGetNumOperands(handle);
                        var operands = new Expression[operandCount];

                        for (uint i = 0; i < operandCount; i++)
                            operands[i] = new Expression(BinaryenHostGetOperand(handle, i));

                        return new HostInfo(name, operands, type);
                    }

                case ExpressionId.Nop:
                    return new NopInfo(type);

                case ExpressionId.Unreachable:
                    return new UnreachableInfo(type);

                case ExpressionId.AtomicRMW:
                    {
                        var op = BinaryenAtomicRMWGetOp(handle);
                        var offset = BinaryenAtomicRMWGetOffset(handle);
                        var bytes = BinaryenAtomicRMWGetBytes(handle);
                        var ptr = new Expression(BinaryenAtomicRMWGetPtr(handle));
                        var value = new Expression(BinaryenAtomicRMWGetValue(handle));

                        return new AtomicReadModifyWriteInfo(op, offset, bytes, ptr, value, type);
                    }

                case ExpressionId.AtomicCmpxchg:
                    {
                        var offset = BinaryenAtomicCmpxchgGetOffset(handle);
                        var bytes = BinaryenAtomicCmpxchgGetBytes(handle);
                        var ptr = new Expression(BinaryenAtomicCmpxchgGetPtr(handle));
                        var expected = new Expression(BinaryenAtomicCmpxchgGetExpected(handle));
                        var replacement = new Expression(BinaryenAtomicCmpxchgGetReplacement(handle));

                        return new AtomicCompareExchangeInfo(offset, bytes, ptr, expected, replacement, type);
                    }

                case ExpressionId.AtomicWait:
                    return new AtomicWaitInfo(
                        new Expression(BinaryenAtomicWaitGetPtr(handle)),
                        new Expression(BinaryenAtomicWaitGetExpected(handle)),
                        new Expression(BinaryenAtomicWaitGetTimeout(handle)),
                        BinaryenAtomicWaitGetExpectedType(handle),
                        type
                    );

                case ExpressionId.AtomicWake:
                    return new AtomicWakeInfo(
                        new Expression(BinaryenAtomicWakeGetPtr(handle)),
                        new Expression(BinaryenAtomicWakeGetWakeCount(handle)),
                        type
                    );

                default:
                    throw new NotSupportedException($"Unexpected expression ID: {id}");
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
        private static extern ValueType BinaryenExpressionGetType(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern void BinaryenExpressionPrint(IntPtr expr);

        // Block

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern /*const char**/IntPtr BinaryenBlockGetName(IntPtr expr);

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

        // Loop

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenLoopGetName(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenLoopGetBody(IntPtr expr);

        // Break

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenBreakGetName(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenBreakGetCondition(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenBreakGetValue(IntPtr expr);

        // Switch

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint BinaryenSwitchGetNumNames(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenSwitchGetName(IntPtr expr, uint index);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenSwitchGetDefaultName(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenSwitchGetCondition(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenSwitchGetValue(IntPtr expr);

        // Call

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern /*const char**/ IntPtr BinaryenCallGetTarget(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint BinaryenCallGetNumOperands(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenCallGetOperand(IntPtr expr, uint index);

        // CallImport

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenCallImportGetTarget(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint BinaryenCallImportGetNumOperands(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenCallImportGetOperand(IntPtr expr, uint index);

        // CallIndirect

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenCallIndirectGetTarget(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint BinaryenCallIndirectGetNumOperands(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenCallIndirectGetOperand(IntPtr expr, uint index);

        // GetLocal

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint BinaryenGetLocalGetIndex(IntPtr expr);

        // SetLocal

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern int BinaryenSetLocalIsTee(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint BinaryenSetLocalGetIndex(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenSetLocalGetValue(IntPtr expr);

        // GetGlobal

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern /*const char**/ IntPtr BinaryenGetGlobalGetName(IntPtr expr);

        // SetGlobal

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern /*const char**/ IntPtr BinaryenSetGlobalGetName(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenSetGlobalGetValue(IntPtr expr);

        // Host

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern HostOperator BinaryenHostGetOp(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern /*const char**/ IntPtr BinaryenHostGetNameOperand(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint BinaryenHostGetNumOperands(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenHostGetOperand(IntPtr expr, uint index);

        // Load

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern int BinaryenLoadIsAtomic(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern int BinaryenLoadIsSigned(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint BinaryenLoadGetOffset(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint BinaryenLoadGetBytes(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint BinaryenLoadGetAlign(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenLoadGetPtr(IntPtr expr);

        // Store

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern int BinaryenStoreIsAtomic(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint BinaryenStoreGetBytes(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint BinaryenStoreGetOffset(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint BinaryenStoreGetAlign(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenStoreGetPtr(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenStoreGetValue(IntPtr expr);

        // Const

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern int BinaryenConstGetValueI32(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern long BinaryenConstGetValueI64(IntPtr expr);

        //[DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        //private static extern int BinaryenConstGetValueI64Low(IntPtr expr);

        //[DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        //private static extern int BinaryenConstGetValueI64High(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern float BinaryenConstGetValueF32(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern double BinaryenConstGetValueF64(IntPtr expr);

        // Unary

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern UnaryOperator BinaryenUnaryGetOp(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenUnaryGetValue(IntPtr expr);

        // Binary

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern BinaryOperator BinaryenBinaryGetOp(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenBinaryGetLeft(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenBinaryGetRight(IntPtr expr);

        // Select

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenSelectGetIfTrue(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenSelectGetIfFalse(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenSelectGetCondition(IntPtr expr);

        // Drop

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenDropGetValue(IntPtr expr);

        // Return

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenReturnGetValue(IntPtr expr);

        // AtommicRMW

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern AtomicOperator BinaryenAtomicRMWGetOp(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint BinaryenAtomicRMWGetBytes(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint BinaryenAtomicRMWGetOffset(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenAtomicRMWGetPtr(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenAtomicRMWGetValue(IntPtr expr);

        // AtomicCmpxchg

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint BinaryenAtomicCmpxchgGetBytes(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint BinaryenAtomicCmpxchgGetOffset(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenAtomicCmpxchgGetPtr(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenAtomicCmpxchgGetExpected(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenAtomicCmpxchgGetReplacement(IntPtr expr);

        // AtomicWait

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenAtomicWaitGetPtr(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenAtomicWaitGetExpected(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenAtomicWaitGetTimeout(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern ValueType BinaryenAtomicWaitGetExpectedType(IntPtr expr);

        // AtomicWake

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenAtomicWakeGetPtr(IntPtr expr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenAtomicWakeGetWakeCount(IntPtr expr);

        #endregion
    }
}
