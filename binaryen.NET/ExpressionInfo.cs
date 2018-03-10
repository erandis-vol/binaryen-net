using System;

namespace Binaryen
{
    public abstract class ExpressionInfo
    {
        protected ExpressionId id;
        protected Type type;

        protected ExpressionInfo(ExpressionId id, Type type)
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
        public Type Type => type;
    }

    public class BlockInfo : ExpressionInfo
    {
        private string name;
        private Expression[] children;

        public BlockInfo(string name, Expression[] children, Type type) : base(ExpressionId.Block, type)
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

    public class IfInfo : ExpressionInfo
    {
        private Expression condition, ifTrue, ifFalse;

        public IfInfo(Expression condition, Expression ifTrue, Expression ifFalse, Type type) : base(ExpressionId.If, type)
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

    public class NopInfo : ExpressionInfo
    {
        public NopInfo(Type type) : base(ExpressionId.Nop, type)
        { }
    }

    public class UnreachableInfo : ExpressionInfo
    {
        public UnreachableInfo(Type type) : base(ExpressionId.Unreachable, type)
        { }
    }
}
