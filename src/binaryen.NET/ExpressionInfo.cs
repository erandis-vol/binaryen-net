namespace Binaryen
{
    /// <summary>
    /// Represents information about an <see cref="Expression"/>.
    /// </summary>
    public abstract class ExpressionInfo
    {
        protected ExpressionId id;
        protected ValueType type;

        protected ExpressionInfo(ExpressionId id, ValueType type)
        {
            this.id = id;
            this.type = type;
        }

        /// <summary>
        /// Gets the ID (kind) of the expression.
        /// </summary>
        public ExpressionId ID => id;

        /// <summary>
        /// Gets the type of the expression.
        /// </summary>
        public ValueType Type => type;
    }

    /// <summary>
    /// Represenets information about a block <see cref="Expression"/>.
    /// </summary>
    public class BlockInfo : ExpressionInfo
    {
        private string name;
        private Expression[] children;

        public BlockInfo(string name, Expression[] children, ValueType type) : base(ExpressionId.Block, type)
        {
            this.name = name;
            this.children = children;
        }

        /// <summary>
        /// Gets the block name.
        /// </summary>
        public string Name => name;

        /// <summary>
        /// Gets the child expressions.
        /// </summary>
        public Expression[] Children => children;
    }

    /// <summary>
    /// Represents information about an if-else <see cref="Expression"/>.
    /// </summary>
    public class IfInfo : ExpressionInfo
    {
        private Expression condition, ifTrue, ifFalse;

        public IfInfo(Expression condition, Expression ifTrue, Expression ifFalse, ValueType type) : base(ExpressionId.If, type)
        {
            this.condition = condition;
            this.ifTrue = ifTrue;
            this.ifFalse = ifFalse;
        }

        /// <summary>
        /// Gets the nested condition expression.
        /// </summary>
        public Expression Condition => condition;

        /// <summary>
        /// Gets the nested if-true expression.
        /// </summary>
        public Expression IfTrue => ifTrue;

        /// <summary>
        /// Gets the nested if-false expression.
        /// </summary>
        public Expression IfFalse => ifFalse;
    }

    /// <summary>
    /// Represents information about a loop <see cref="Expression"/>.
    /// </summary>
    public class LoopInfo : ExpressionInfo
    {
        private string name;
        private Expression body;

        public LoopInfo(string name, Expression body, ValueType type) : base(ExpressionId.Loop, type)
        {
            this.name = name;
            this.body = body;
        }

        /// <summary>
        /// Gets the name of the loop.
        /// </summary>
        public string Name => name;

        /// <summary>
        /// Gets the body of the loop.
        /// </summary>
        public Expression Body => body;
    }

    /// <summary>
    /// Represents information about a break <see cref="Expression"/>.
    /// </summary>
    public class BreakInfo : ExpressionInfo
    {
        private string label;
        private Expression condition, value;

        public BreakInfo(string label, Expression condition, Expression value, ValueType type) : base(ExpressionId.Break, type)
        {
            this.label = label;
            this.condition = condition;
            this.value = value;
        }

        /// <summary>
        /// Gets the label.
        /// </summary>
        public string Label => label;

        /// <summary>
        /// Gets the condition expression.
        /// </summary>
        public Expression Condition => condition;

        /// <summary>
        /// Gets the value expression.
        /// </summary>
        public Expression Value => value;
    }

    /// <summary>
    /// Represents information about a switch <see cref="Expression"/>.
    /// </summary>
    public class SwitchInfo : ExpressionInfo
    {
        private string[] labels;
        private string defaultLabel;
        private Expression condition, value;

        public SwitchInfo(string[] labels, string defaultLabel, Expression condition, Expression value, ValueType type)
            : base(ExpressionId.Switch, type)
        {
            this.labels = labels;
            this.defaultLabel = defaultLabel;
            this.condition = condition;
            this.value = value;
        }

        /// <summary>
        /// Gets the labels for each case.
        /// </summary>
        public string[] Labels => labels;

        /// <summary>
        /// Gets the label for the default case.
        /// </summary>
        public string DefaultLabel => defaultLabel;

        /// <summary>
        /// Gets the condition expression.
        /// </summary>
        public Expression Condition => condition;

        /// <summary>
        /// Gets the value expression.
        /// </summary>
        public Expression Value => value;
    }

    /// <summary>
    /// Represents information about a call <see cref="Expression"/>.
    /// </summary>
    public class CallInfo : ExpressionInfo
    {
        private string target;
        private Expression[] operands;

        public CallInfo(string target, Expression[] operands, ValueType type) : base(ExpressionId.Call, type)
        {
            this.target = target;
            this.operands = operands;
        }

        /// <summary>
        /// Gets the call target.
        /// </summary>
        public string Target => target;

        /// <summary>
        /// Gets the call operands.
        /// </summary>
        public Expression[] Operands => operands;
    }

    /// <summary>
    /// Represents information about a call-import <see cref="Expression"/>.
    /// </summary>
    public class CallImportInfo : ExpressionInfo
    {
        private string target;
        private Expression[] operands;

        public CallImportInfo(string target, Expression[] operands, ValueType type) : base(ExpressionId.CallImport, type)
        {
            this.target = target;
            this.operands = operands;
        }

        /// <summary>
        /// Gets the call target.
        /// </summary>
        public string Target => target;

        /// <summary>
        /// Gets the call operands.
        /// </summary>
        public Expression[] Operands => operands;
    }

    /// <summary>
    /// Represents information about a call-indirect <see cref="Expression"/>.
    /// </summary>
    public class CallIndirectInfo : ExpressionInfo
    {
        private Expression target;
        private Expression[] operands;

        public CallIndirectInfo(Expression target, Expression[] operands, ValueType type) : base(ExpressionId.CallIndirect, type)
        {
            this.target = target;
            this.operands = operands;
        }

        /// <summary>
        /// Gets the call target.
        /// </summary>
        public Expression Target => target;

        /// <summary>
        /// Gets the call operands.
        /// </summary>
        public Expression[] Operands => operands;
    }

    /// <summary>
    /// Represents information about a get-local <see cref="Expression"/>.
    /// </summary>
    public class GetLocalInfo : ExpressionInfo
    {
        private uint index;

        public GetLocalInfo(uint index, ValueType type) : base(ExpressionId.GetLocal, type)
        {
            this.index = index;
        }

        /// <summary>
        /// Gets the local index.
        /// </summary>
        public uint Index => index;
    }

    /// <summary>
    /// Represents information about a set <see cref="Expression"/>.
    /// </summary>
    public class SetLocalInfo : ExpressionInfo
    {
        private uint index;
        private bool isTee;
        private Expression value;

        public SetLocalInfo(uint index, bool isTee, Expression value, ValueType type) : base(ExpressionId.SetLocal, type)
        {
            this.index = index;
            this.isTee = isTee;
            this.value = value;
        }

        /// <summary>
        /// Gets whether the set is a tee. A tee differs in that the value remains on the stack.
        /// </summary>
        public bool IsTee => isTee;

        /// <summary>
        /// Gets the local index.
        /// </summary>
        public uint Index => index;

        /// <summary>
        /// Gets the value expression.
        /// </summary>
        public Expression Value => value;
    }

    /// <summary>
    /// Represents information about a get-global <see cref="Expression"/>.
    /// </summary>
    public class GetGlobalInfo : ExpressionInfo
    {
        private string name;

        public GetGlobalInfo(string name, ValueType type) : base(ExpressionId.GetGlobal, type)
        {
            this.name = name;
        }

        /// <summary>
        /// Gets the global name.
        /// </summary>
        public string Name => name;
    }

    /// <summary>
    /// Represents information about a get-global <see cref="Expression"/>.
    /// </summary>
    public class SetGlobalInfo : ExpressionInfo
    {
        private string name;
        private Expression value;

        public SetGlobalInfo(string name, Expression value, ValueType type) : base(ExpressionId.SetGlobal, type)
        {
            this.name = name;
            this.value = value;
        }

        /// <summary>
        /// Gets the global name.
        /// </summary>
        public string Name => name;

        /// <summary>
        /// Gets the value expression.
        /// </summary>
        public Expression Value => value;
    }

    /// <summary>
    /// Represents information about a host <see cref="Expression"/>.
    /// </summary>
    public class HostInfo : ExpressionInfo
    {
        private string name;
        private Expression[] operands;

        public HostInfo(string name, Expression[] operands, ValueType type) : base(ExpressionId.Host, type)
        {
            this.name = name;
            this.operands = operands;
        }

        /// <summary>
        /// Gets the host name.
        /// </summary>
        public string Name => name;

        /// <summary>
        /// Gets the operand expressions.
        /// </summary>
        public Expression[] Operands => operands;
    }

    /// <summary>
    /// Represents information about a load <see cref="Expression"/>.
    /// </summary>
    public class LoadInfo : ExpressionInfo
    {
        private bool isAtomic, isSigned;
        private uint offset, bytes, align;
        private Expression ptr;

        public LoadInfo(bool isAtomic, bool isSigned, uint offset, uint bytes, uint align, Expression ptr, ValueType type)
            : base(ExpressionId.Load, type)
        {
            this.isAtomic = isAtomic;
            this.isSigned = isSigned;
            this.offset = offset;
            this.bytes = bytes;
            this.align = align;
            this.ptr = ptr;
        }

        /// <summary>
        /// Gets whether the load is atomic.
        /// </summary>
        public bool IsAtomic => isAtomic;

        /// <summary>
        /// Gets whether the load is signed.
        /// </summary>
        public bool IsSigned => isSigned;

        /// <summary>
        /// Gets the load offset.
        /// </summary>
        public uint Offset => offset;

        /// <summary>
        /// Gets the size of the load in bytes.
        /// </summary>
        public uint Bytes => bytes;

        /// <summary>
        /// Gets the load alignment.
        /// </summary>
        public uint Align => align;

        /// <summary>
        /// Gets the pointer expression.
        /// </summary>
        public Expression Ptr => ptr;
    }

    /// <summary>
    /// Represents information about a store <see cref="Expression"/>.
    /// </summary>
    public class StoreInfo : ExpressionInfo
    {
        private bool isAtomic;
        private uint offset, bytes, align;
        private Expression ptr, value;

        public StoreInfo(bool isAtomic, uint offset, uint bytes, uint align, Expression ptr, Expression value, ValueType type)
            : base(ExpressionId.Load, type)
        {
            this.isAtomic = isAtomic;
            this.offset = offset;
            this.bytes = bytes;
            this.align = align;
            this.ptr = ptr;
            this.value = value;
        }

        /// <summary>
        /// Gets whether the store is atomic.
        /// </summary>
        public bool IsAtomic => isAtomic;

        /// <summary>
        /// Gets the store offset.
        /// </summary>
        public uint Offset => offset;

        /// <summary>
        /// Gets the size of the store in bytes.
        /// </summary>
        public uint Bytes => bytes;

        /// <summary>
        /// Gets the store alignment.
        /// </summary>
        public uint Align => align;

        /// <summary>
        /// Gets the pointer expression.
        /// </summary>
        public Expression Ptr => ptr;

        /// <summary>
        /// Gets the value expression.
        /// </summary>
        public Expression Value => value;
    }

    /// <summary>
    /// Represents information about a constant (const) <see cref="Expression"/>.
    /// </summary>
    public class ConstInfo : ExpressionInfo
    {
        private Literal value;

        public ConstInfo(Literal value, ValueType type) : base(ExpressionId.Const, type)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public Literal Value => value;
    }

    /// <summary>
    /// Represents inforamtion about a unary <see cref="Expression"/>.
    /// </summary>
    public class UnaryInfo : ExpressionInfo
    {
        private UnaryOperator op;
        private Expression value;

        public UnaryInfo(UnaryOperator op, Expression value, ValueType type) : base(ExpressionId.Unary, type)
        {
            this.op = op;
            this.value = value;
        }

        /// <summary>
        /// Gets the operator.
        /// </summary>
        public UnaryOperator Operator => op;

        /// <summary>
        /// Gets the value expression.
        /// </summary>
        public Expression Value => value;
    }

    /// <summary>
    /// Represents information about a binary <see cref="Expression"/>.
    /// </summary>
    public class BinaryInfo : ExpressionInfo
    {
        private BinaryOperator op;
        private Expression left, right;

        public BinaryInfo(BinaryOperator op, Expression left, Expression right, ValueType type) : base(ExpressionId.Binary, type)
        {
            this.op = op;
            this.left = left;
            this.right = right;
        }

        /// <summary>
        /// Gets the operator.
        /// </summary>
        public BinaryOperator Operator => op;

        /// <summary>
        /// Gets the left value expression.
        /// </summary>
        public Expression Left => left;

        /// <summary>
        /// Gets the right value expression.
        /// </summary>
        public Expression Right => right;
    }

    /// <summary>
    /// Represents information about a select <see cref="Expression"/>.
    /// </summary>
    public class SelectInfo : ExpressionInfo
    {
        private Expression condition, ifTrue, ifFalse;

        public SelectInfo(Expression condition, Expression ifTrue, Expression ifFalse, ValueType type) : base(ExpressionId.Select, type)
        {
            this.condition = condition;
            this.ifTrue = ifTrue;
            this.ifFalse = ifFalse;
        }

        /// <summary>
        /// Gets the condition expression.
        /// </summary>
        public Expression Condition => condition;

        /// <summary>
        /// Gets the if-true expression.
        /// </summary>
        public Expression IfTrue => ifTrue;

        /// <summary>
        /// Gets the if-false expression.
        /// </summary>
        public Expression IfFalse => ifFalse;
    }

    /// <summary>
    /// Represents information about a drop <see cref="Expression"/>.
    /// </summary>
    public class DropInfo : ExpressionInfo
    {
        private Expression value;

        public DropInfo(Expression value, ValueType type) : base(ExpressionId.Drop, type)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the value expression.
        /// </summary>
        public Expression Value => value;
    }

    /// <summary>
    /// Represents information about a return <see cref="Expression"/>.
    /// </summary>
    public class ReturnInfo : ExpressionInfo
    {
        private Expression value;

        public ReturnInfo(Expression value, ValueType type) : base(ExpressionId.Return, type)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the value expression.
        /// </summary>
        public Expression Value => value;
    }

    /// <summary>
    /// Represents information about a no-operation (nop) <see cref="Expression"/>.
    /// </summary>
    public class NopInfo : ExpressionInfo
    {
        public NopInfo(ValueType type) : base(ExpressionId.Nop, type)
        { }
    }

    /// <summary>
    /// Represents information about an unreachable <see cref="Expression"/>.
    /// </summary>
    public class UnreachableInfo : ExpressionInfo
    {
        public UnreachableInfo(ValueType type) : base(ExpressionId.Unreachable, type)
        { }
    }

    /// <summary>
    /// Represents information about an atomic read-modify-write (RMW) <see cref="Expression"/>.
    /// </summary>
    public class AtomicReadModifyWriteInfo : ExpressionInfo
    {
        private AtomicOperator op;
        private uint bytes, offset;
        private Expression ptr, value;

        public AtomicReadModifyWriteInfo(AtomicOperator op, uint offset, uint bytes, Expression ptr, Expression value, ValueType type)
            : base(ExpressionId.AtomicRMW, type)
        {
            this.op = op;
            this.offset = offset;
            this.bytes = bytes;
            this.ptr = ptr;
            this.value = value;
        }

        /// <summary>
        /// Gets the operator.
        /// </summary>
        public AtomicOperator Operator => op;

        /// <summary>
        /// Gets the offset.
        /// </summary>
        public uint Offset => offset;

        /// <summary>
        /// Gets the size of the operation in bytes.
        /// </summary>
        public uint Bytes => bytes;

        /// <summary>
        /// Gets the pointer expression.
        /// </summary>
        public Expression Ptr => ptr;

        /// <summary>
        /// Gets the value expression.
        /// </summary>
        public Expression Value => value;
    }

    /// <summary>
    /// Represents information about a compare-exchange (Cmpxcgh) <see cref="Expression"/>.
    /// </summary>
    public class AtomicCompareExchangeInfo : ExpressionInfo
    {
        private uint offset, bytes;
        private Expression ptr, expected, replacement;

        public AtomicCompareExchangeInfo(uint offset, uint bytes, Expression ptr, Expression expected, Expression replacement, ValueType type)
            : base(ExpressionId.AtomicCmpxchg, type)
        {
            this.offset = offset;
            this.bytes = bytes;
            this.ptr = ptr;
            this.expected = expected;
            this.replacement = replacement;
        }

        /// <summary>
        /// Gets the start offset.
        /// </summary>
        public uint Offset => offset;

        /// <summary>
        /// Gets the size of the operation in bytes.
        /// </summary>
        public uint Bytes => bytes;

        /// <summary>
        /// Gets the pointer expression.
        /// </summary>
        public Expression Ptr => ptr;

        /// <summary>
        /// Gets the expected value expression.
        /// </summary>
        public Expression Expected => expected;

        /// <summary>
        /// Gets the replacement value expression.
        /// </summary>
        public Expression Replacement => replacement;
    }

    /// <summary>
    /// Represents information about an atomic wait <see cref="Expression"/>.
    /// </summary>
    public class AtomicWaitInfo : ExpressionInfo
    {
        private Expression ptr, expected, timeout;
        private ValueType expectedType;

        public AtomicWaitInfo(Expression ptr, Expression expected, Expression timeout, ValueType expectedType, ValueType type)
            : base(ExpressionId.AtomicWait, type)
        {
            this.ptr = ptr;
            this.expected = expected;
            this.timeout = timeout;
            this.expectedType = expectedType;
        }

        /// <summary>
        /// Gets the pointer expression.
        /// </summary>
        public Expression Ptr => ptr;

        /// <summary>
        /// Gets the expected value expression.
        /// </summary>
        public Expression Expected => expected;

        /// <summary>
        /// Gets the timeout value expression.
        /// </summary>
        public Expression Timeout => timeout;

        /// <summary>
        /// Gets the expected type.
        /// </summary>
        public ValueType ExpectedType => expectedType;
    }

    /// <summary>
    /// Represents information about an atomic wake <see cref="Expression"/>.
    /// </summary>
    public class AtomicWakeInfo : ExpressionInfo
    {
        private Expression ptr, wakeCount;

        public AtomicWakeInfo(Expression ptr, Expression wakeCount, ValueType type) : base(ExpressionId.AtomicWake, type)
        {
            this.ptr = ptr;
            this.wakeCount = wakeCount;
        }

        /// <summary>
        /// Gets the pointer expression.
        /// </summary>
        public Expression Ptr => ptr;

        /// <summary>
        /// Gets the wake count expression.
        /// </summary>
        public Expression WakeCount => wakeCount;
    }
}
