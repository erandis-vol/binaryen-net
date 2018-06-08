using System;
using System.Runtime.InteropServices;

namespace Binaryen
{
    /// <summary>
    /// Represents a function.
    /// </summary>
    public class Function : AutomaticBaseObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Function"/> class for the specified handle.
        /// </summary>
        /// <param name="handle">The handle to be managed.</param>
        public Function(IntPtr handle)
            : base(handle)
        { }

        /// <summary>
        /// Returns the parameter types of the function.
        /// </summary>
        public ValueType[] GetParameters()
        {
            var parameterCount = BinaryenFunctionGetNumParams(Handle);
            var parameters = new ValueType[parameterCount];

            for (uint i = 0; i < parameterCount; i++)
                parameters[i] = BinaryenFunctionGetParam(Handle, i);

            return parameters;
        }

        /// <summary>
        /// Returns the local variable types of the function.
        /// </summary>
        /// <returns></returns>
        public ValueType[] GetLocals()
        {
            var localCount = BinaryenFunctionGetNumVars(Handle);
            var locals = new ValueType[localCount];

            for (uint i = 0; i < localCount; i++)
                locals[i] = BinaryenFunctionGetVar(Handle, i);

            return locals;
        }

        /// <summary>
        /// Runs the standard optimization passes on the function, using the global optimization and shrink level.
        /// </summary>
        /// <param name="module">The parent module.</param>
        /// <exception cref="ArgumentNullException"><paramref name="module"/> is null.</exception>
        public void Optimize(Module module)
        {
            if (module == null)
                throw new ArgumentNullException(nameof(module));

            BinaryenFunctionOptimize(Handle, module.Handle);
        }

        /// <summary>
        /// Runs the specified passes on the function, using the global optimization and shrink level.
        /// </summary>
        /// <param name="module">The parent module.</param>
        /// <param name="passes">The passes to run.</param>
        /// <exception cref="ArgumentNullException"><paramref name="module"/> or <paramref name="passes"/> is null.</exception>
        public void RunPasses(Module module, string[] passes)
        {
            if (module == null || passes == null)
                throw new ArgumentNullException(module == null ? nameof(module) : nameof(passes));

            BinaryenFunctionRunPasses(Handle, module.Handle, passes, (uint)passes.Length);
        }

        /// <summary>
        /// Sets the debug information of the specified expression within this function.
        /// </summary>
        /// <param name="expression">The expresion.</param>
        /// <param name="fileIndex">The file index.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="columnNumber">The column number.</param>
        public void SetDebugLocation(Expression expression, uint fileIndex, uint lineNumber, uint columnNumber)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            BinaryenFunctionSetDebugLocation(Handle, expression.Handle, fileIndex, lineNumber, columnNumber);
        }

        /// <summary>
        /// Gets the name of the function.
        /// </summary>
        public string Name => Marshal.PtrToStringAnsi(BinaryenFunctionGetName(Handle));

        /// <summary>
        /// Gets the name of the function type. May be <c>null</c> if the type is implicit.
        /// </summary>
        public string Type => Marshal.PtrToStringAnsi(BinaryenFunctionGetType(Handle));

        /// <summary>
        /// Gets the result type of the function.
        /// </summary>
        public ValueType Result => BinaryenFunctionGetResult(Handle);

        /// <summary>
        /// Gets the body of the function.
        /// </summary>
        public Expression Body
        {
            get
            {
                var exprRef = BinaryenFunctionGetBody(Handle);
                return exprRef == IntPtr.Zero ? null : new Expression(exprRef);
            }
        }

        #region Imports

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenFunctionGetName(IntPtr func);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenFunctionGetType(IntPtr func);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint BinaryenFunctionGetNumParams(IntPtr func);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern ValueType BinaryenFunctionGetParam(IntPtr func, uint index);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern ValueType BinaryenFunctionGetResult(IntPtr func);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint BinaryenFunctionGetNumVars(IntPtr func);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern ValueType BinaryenFunctionGetVar(IntPtr func, uint index);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenFunctionGetBody(IntPtr func);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern void BinaryenFunctionOptimize(IntPtr func, IntPtr module);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void BinaryenFunctionRunPasses(IntPtr func, IntPtr module, string[] passes, uint numPasses);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern void BinaryenFunctionSetDebugLocation(IntPtr func, IntPtr expr, uint fileIndex, uint lineNumber, uint columnNumber);

        #endregion
    }
}
