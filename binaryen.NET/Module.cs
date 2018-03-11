using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Binaryen
{
    /// <summary>Represents a module.</summary>
    /// <remarks>
    /// Modules contain lists of functions, imports, exports and types. The Add* methods create them on the module.
    /// The module owns them and will free their memory when disposed of.
    /// 
    /// Expressions are also allocated within and freed by modules. They are not created by Add* methods,
    /// since they are not added directly to the module, instead, they are arguments to other expressions
    /// (and then they are the children of that AST node), or to a function
    /// (and then they are the body of that function).
    /// 
    /// A module can also contain a function table for indirect calls, a memory, and a start method.
    /// </remarks>
    public class Module : IDisposable
    {
        private IntPtr handle;

        /// <summary>
        /// Initializes a new instance of the <see cref="Module"/> class.
        /// </summary>
        /// <exception cref="OutOfMemoryException">the module could not be created.</exception>
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

        /// <summary>
        /// Prints the module to STDOUT. Useful for debugging.
        /// </summary>
        public void Print()
        {
            if (handle != IntPtr.Zero)
            {
                BinaryenModulePrint(handle);
            }
        }

        /// <summary>
        /// Adds a new function type.
        /// </summary>
        /// <param name="name">The name of the type.</param>
        /// <param name="result">The return type.</param>
        /// <returns>A <see cref="Signature"/> representing the function type.</returns>
        /// <exception cref="OutOfMemoryException">the type could not be created.</exception>
        public Signature AddFunctionType(string name, Type result)
        {
            return AddFunctionType(name, result, null);
        }

        /// <summary>
        /// Adds a new function type.
        /// </summary>
        /// <param name="name">The name of the type.</param>
        /// <param name="result">The return type.</param>
        /// <param name="parameters">The parameter types.</param>
        /// <returns>A <see cref="Signature"/> representing the function type.</returns>
        /// <exception cref="OutOfMemoryException">the type could not be created.</exception>
        public Signature AddFunctionType(string name, Type result, IEnumerable<Type> parameters)
        {
            if (parameters.Any())
            {
                return AddFunctionType(name, result, parameters.ToArray());
            }
            else
            {
                return AddFunctionType(name, result, null);
            }
        }

        /// <summary>
        /// Adds a new function type.
        /// </summary>
        /// <param name="name">The name of the type.</param>
        /// <param name="result">The return type.</param>
        /// <param name="parameters">The parameter types.</param>
        /// <returns>A <see cref="Signature"/> representing the function type.</returns>
        /// <exception cref="OutOfMemoryException">the type could not be created.</exception>
        public Signature AddFunctionType(string name, Type result, Type[] parameters)
        {
            IntPtr signatureRef;

            if (parameters == null)
            {
                signatureRef = BinaryenAddFunctionType(handle, name, result, null, 0u);
            }
            else
            {
                signatureRef = BinaryenAddFunctionType(handle, name, result, parameters, (uint)parameters.Length);
            }

            if (signatureRef == IntPtr.Zero)
            {
                throw new OutOfMemoryException();
            }

            return new Signature(signatureRef);
        }

        /// <summary>
        /// Returns an existing function type by its parametric signature.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parameters"></param>
        /// <returns>A <see cref="Signature"/> representing the function type.
        /// If there is no such type, returns <c>null</c>.</returns>
        public Signature GetFunctionTypeBySignature(Type result, Type[] parameters)
        {
            var sig = BinaryenGetFunctionTypeBySignature(handle, result, parameters, (uint)parameters.Length);
            return sig == IntPtr.Zero ? null : new Signature(sig);
        }

        public Function AddFunction(string name, Signature signature, Expression body)
        {
            var func = BinaryenAddFunction(handle, name, signature.Handle, null, 0u, body.Handle);
            if (func == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Function(func);
        }

        public void AddFunction(string name, Signature signature, IEnumerable<Type> varTypes, Expression body)
        {
            AddFunction(name, signature, varTypes.ToArray(), body);
        }

        public Function AddFunction(string name, Signature signature, Type[] varTypes, Expression body)
        {
            if (varTypes == null)
                throw new ArgumentNullException(nameof(varTypes));

            var func = BinaryenAddFunction(handle, name, signature.Handle, varTypes, (uint)varTypes.Length, body.Handle);
            if (func == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Function(func);
        }

        public Function GetFunction(string name)
        {
            var ptr = BinaryenGetFunction(handle, name);
            if (ptr == IntPtr.Zero)
                return null;
            else
                return new Function(ptr);
        }

        public void RemoveFunction(string name)
        {
            BinaryenRemoveFunction(handle, name);
        }

        /// <summary>
        /// Sets the start function for the module. There can only be one.
        /// </summary>
        /// <param name="start">The start function.</param>
        /// <exception cref="ArgumentNullException"><paramref name="start"/> is null.</exception>
        public void SetStart(Function start)
        {
            if (start == null)
                throw new ArgumentNullException(nameof(start));

            BinaryenSetStart(handle, start.Handle);
        }

        public Expression Block(string name, IEnumerable<Expression> children, Type type = Type.None)
        {
            var childrenHandles = children.Select(x => x.Handle).ToArray();

            var expr = BinaryenBlock(handle, name, childrenHandles, (uint)childrenHandles.Length, type);
            if (expr == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(expr);
        }

        public Expression If(Expression condition, Expression ifTrue, Expression ifFalse = null)
        {
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));

            if (ifTrue == null)
                throw new ArgumentNullException(nameof(ifTrue));

            var conditionHandle = condition.Handle;
            var ifTrueHandle = ifTrue.Handle;
            var ifFalseHandle = ifFalse == null ? IntPtr.Zero : ifFalse.Handle;

            var @if = BinaryenIf(handle, conditionHandle, ifTrueHandle, ifFalseHandle);
            if (@if == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(@if);
        }

        public Expression Loop(string label, Expression body)
        {
            if (body == null)
                throw new ArgumentNullException(nameof(body));

            var loop = BinaryenLoop(handle, label, body.Handle);
            if (loop == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(loop);
        }

        public Expression Break(string label, Expression condition = null, Expression value = null)
        {
            // value AND condition can be NULL
            if (label == null)
                throw new ArgumentNullException(nameof(label));

            var @break = BinaryenBreak(handle, label, condition == null ? IntPtr.Zero : condition.Handle,
                value == null ? IntPtr.Zero : value.Handle);
            if (@break == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(@break);
        }

        public Expression Switch(string[] labels, string defaultLabel, Expression condition, Expression value = null)
        {
            // value can be NULL
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));

            var @switch = BinaryenSwitch(handle, labels, (uint)labels.Length, defaultLabel, condition.Handle,
                value == null ? IntPtr.Zero : value.Handle);
            if (@switch == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(@switch);
        }

        public Expression Call(string target, IEnumerable<Expression> operands, Type returnType)
        {
            var operandHandles = operands.Select(x => x.Handle).ToArray();

            var call = BinaryenCall(handle, target, operandHandles, (uint)operandHandles.Length, returnType);
            if (call == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(call);
        }

        public Expression CallImport(string target, IEnumerable<Expression> operands, Type returnType)
        {
            var operandHandles = operands.Select(x => x.Handle).ToArray();

            var call = BinaryenCallImport(handle, target, operandHandles, (uint)operandHandles.Length, returnType);
            if (call == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(call);
        }

        public Expression CallIndirect(Expression target, IEnumerable<Expression> operands, string type)
        {
            var operandHandles = operands.Select(x => x.Handle).ToArray();

            var call = BinaryenCallIndirect(handle, target.Handle, operandHandles, (uint)operandHandles.Length, type);
            if (call == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(call);
        }

        public Expression GetLocal(uint index, Type type)
        {
            var expr = BinaryenGetLocal(handle, index, type);
            if (expr == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(expr);
        }

        public Expression SetLocal(uint index, Expression value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var expr = BinaryenSetLocal(handle, index, value.Handle);
            if (expr == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(expr);
        }

        public Expression TeeLocal(uint index, Expression value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var expr = BinaryenTeeLocal(handle, index, value.Handle);
            if (expr == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(expr);
        }

        public Expression GetGlobal(string name, Type type)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var expr = BinaryenGetGlobal(handle, name, type);
            if (expr == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(expr);
        }

        public Expression SetGlobal(string name, Expression value)
        {
            if (value == null || name == null)
                throw new ArgumentNullException(value == null ? nameof(value) : nameof(name));

            var expr = BinaryenSetGlobal(handle, name, value.Handle);
            if (expr == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(expr);
        }

        public Expression Load(uint bytes, bool signed, uint offset, Type type, Expression ptr)
        {
            return Load(bytes, signed, offset, 0, type, ptr);
        }

        public Expression Load(uint bytes, bool signed, uint offset, uint align, Type type, Expression ptr)
        {
            if (ptr == null)
                throw new ArgumentNullException(nameof(ptr));

            var load = BinaryenLoad(handle, bytes, (sbyte)(signed ? 1 : 0), offset, align, type, ptr.Handle);
            if (load == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(load);
        }

        public Expression Store(uint bytes, uint offset, Expression ptr, Expression value, Type type)
        {
            return Store(bytes, offset, 0, ptr, value, type);
        }

        public Expression Store(uint bytes, uint offset, uint align, Expression ptr, Expression value, Type type)
        {
            if (ptr == null || value == null)
                throw new ArgumentNullException(ptr == null ? nameof(ptr) : nameof(value));

            var expr = BinaryenStore(handle, bytes, offset, align, ptr.Handle, value.Handle, type);
            if (expr == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(expr);
        }

        public Expression Const(Literal value)
        {
            var expr = BinaryenConst(handle, value);
            if (expr == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(expr);
        }

        public Expression Unary(UnaryOperator op, Expression value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var expr = BinaryenUnary(handle, op, value.Handle);
            if (expr == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(expr);
        }

        public Expression Binary(BinaryOperator op, Expression left, Expression right)
        {
            if (left == null || right == null)
                throw new ArgumentNullException(left == null ? nameof(left) : nameof(right));

            var expr = BinaryenBinary(handle, op, left.Handle, right.Handle);
            if (expr == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(expr);
        }

        public Expression Select(Expression condition, Expression ifTrue, Expression ifFalse)
        {
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));

            if (ifTrue == null)
                throw new ArgumentNullException(nameof(ifTrue));

            if (ifFalse == null)
                throw new ArgumentNullException(nameof(ifFalse));

            var expr = BinaryenSelect(handle, condition.Handle, ifTrue.Handle, ifFalse.Handle);
            if (expr == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(expr);
        }

        /// <summary>
        /// Creates a drop <see cref="Expression"/>.
        /// </summary>
        /// <param name="value">The value to drop.</param>
        /// <returns>An <see cref="Expression"/> instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        /// <exception cref="OutOfMemoryException">the expression could not be created.</exception>
        public Expression Drop(Expression value)
        {
            if (value == null)
                throw new ArgumentNullException();

            var expr = BinaryenDrop(handle, value.Handle);
            if (expr == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(expr);
        }

        /// <summary>
        /// Creates a return <see cref="Expression"/> with an optional value.
        /// </summary>
        /// <param name="value">The optional return value; if null, nothing is returned.</param>
        /// <returns>An <see cref="Expression"/> instance.</returns>
        /// <exception cref="OutOfMemoryException">the expression could not be created.</exception>
        public Expression Return(Expression value = null)
        {
            var expr = BinaryenReturn(handle, value == null ? IntPtr.Zero : value.Handle);
            if (expr == null)
                throw new OutOfMemoryException();

            return new Expression(expr);
        }

        public Expression Host(HostOperator op, string name, IEnumerable<Expression> operands)
        {
            // name can be NULL
            var operandHandles = operands.Select(x => x.Handle).ToArray();

            var host = BinaryenHost(handle, op, name, operandHandles, (uint)operandHandles.Length);
            if (host == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(host);
        }

        /// <summary>
        /// Creates a no-operation (nop) <see cref="Expression"/>.
        /// </summary>
        /// <returns>An <see cref="Expression"/> instance.</returns>
        /// <exception cref="OutOfMemoryException">the expression could not be created.</exception>
        public Expression Nop()
        {
            var expr = BinaryenNop(handle);
            if (expr == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(expr);
        }

        /// <summary>
        /// Creates an unreachable <see cref="Expression"/> that will always trap.
        /// </summary>
        /// <returns>An <see cref="Expression"/> instance.</returns>
        /// <exception cref="OutOfMemoryException">the expression could not be created.</exception>
        public Expression Unreachable()
        {
            var expr = BinaryenUnreachable(handle);
            if (expr == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(expr);
        }

        public Expression AtomicLoad(uint bytes, uint offset, Type type, Expression ptr)
        {
            if (ptr == null)
                throw new ArgumentNullException(nameof(ptr));

            var expr = BinaryenAtomicLoad(handle, bytes, offset, type, ptr.Handle);
            if (expr == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(expr);
        }

        public Expression AtomicStore(uint bytes, uint offset, Expression ptr, Expression value, Type type)
        {
            if (ptr == null || value == null)
                throw new ArgumentNullException(ptr == null ? nameof(ptr) : nameof(value));

            var expr = BinaryenAtomicStore(handle, bytes, offset, ptr.Handle, value.Handle, type);
            if (expr == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(expr);
        }

        public Expression AtomicReadModifyWrite(AtomicOperator op, uint bytes, uint offset, Expression ptr, Expression value, Type type)
        {
            if (ptr == null || value == null)
                throw new ArgumentNullException(ptr == null ? nameof(ptr) : nameof(value));

            var expr = BinaryenAtomicRMW(handle, op, bytes, offset, ptr.Handle, value.Handle, type);
            if (expr == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(expr);
        }

        public Expression AtomicCompareExchange(uint bytes, uint offset, Expression ptr, Expression expected, Expression replacement, Type type)
        {
            if (ptr == null)
                throw new ArgumentNullException(nameof(ptr));

            if (expected == null)
                throw new ArgumentNullException(nameof(expected));

            if (replacement == null)
                throw new ArgumentNullException(nameof(replacement));

            var expr = BinaryenAtomicCmpxchg(handle, bytes, offset, ptr.Handle, expected.Handle, replacement.Handle, type);
            if (expr == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(expr);
        }

        public Expression AtomicWait(Expression ptr, Expression expected, Expression timeout, Type type)
        {
            if (ptr == null)
                throw new ArgumentNullException(nameof(ptr));

            if (expected == null)
                throw new ArgumentNullException(nameof(expected));

            if (timeout == null)
                throw new ArgumentNullException(nameof(timeout));

            var expr = BinaryenAtomicWait(handle, ptr.Handle, expected.Handle, timeout.Handle, type);
            if (expr == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(expr);
        }

        public Expression AtomicWake(Expression ptr, Expression wakeCount)
        {
            if (ptr == null || wakeCount == null)
                throw new ArgumentNullException(ptr == null ? nameof(ptr) : nameof(wakeCount));

            var expr = BinaryenAtomicWake(handle, ptr.Handle, wakeCount.Handle);
            if (expr == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(expr);
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

        // Operations

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern void BinaryenModulePrint(IntPtr module);

        // Expression creation

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr BinaryenBlock(IntPtr module, string name, IntPtr[] children, uint numChildren, Type type);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenIf(IntPtr module, IntPtr condition, IntPtr ifTrue, IntPtr ifFalse);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr BinaryenLoop(IntPtr module, string @in, IntPtr body);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr BinaryenBreak(IntPtr module, string name, IntPtr condition, IntPtr value);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr BinaryenSwitch(IntPtr module, string[] names, uint numNames, string defaultName, IntPtr condition, IntPtr value);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr BinaryenCall(IntPtr module, string target, IntPtr[] operands, uint numOperands, Type returnType);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr BinaryenCallImport(IntPtr module, string target, IntPtr[] operands, uint numOperands, Type returnType);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr BinaryenCallIndirect(IntPtr module, IntPtr target, IntPtr[] operands, uint numOperands, string type);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenGetLocal(IntPtr module, uint index, Type type);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenSetLocal(IntPtr module, uint index, IntPtr value);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenTeeLocal(IntPtr module, uint index, IntPtr value);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr BinaryenGetGlobal(IntPtr module, string name, Type type);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr BinaryenSetGlobal(IntPtr module, string name, IntPtr value);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenLoad(IntPtr module, uint bytes, sbyte signed, uint offset, uint align, Type type, IntPtr ptr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenStore(IntPtr module, uint bytes, uint offset, uint align, IntPtr ptr, IntPtr value, Type type);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenConst(IntPtr module, Literal value);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenUnary(IntPtr module, UnaryOperator op, IntPtr value);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenBinary(IntPtr module, BinaryOperator op, IntPtr left, IntPtr right);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenSelect(IntPtr module, IntPtr condition, IntPtr ifTrue, IntPtr ifFalse);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenDrop(IntPtr module, IntPtr value);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenReturn(IntPtr module, IntPtr value);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr BinaryenHost(IntPtr module, HostOperator op, string name, IntPtr[] operands, uint numOperands);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenNop(IntPtr module);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenUnreachable(IntPtr module);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenAtomicLoad(IntPtr module, uint bytes, uint offset, Type type, IntPtr ptr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenAtomicStore(IntPtr module, uint bytes, uint offset, IntPtr ptr, IntPtr value, Type type);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenAtomicRMW(IntPtr module, AtomicOperator op, uint bytes, uint offset, IntPtr ptr, IntPtr value, Type type);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenAtomicCmpxchg(IntPtr module, uint bytes, uint offset, IntPtr ptr, IntPtr expected, IntPtr replacement, Type type);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenAtomicWait(IntPtr module, IntPtr ptr, IntPtr expected, IntPtr timeout, Type type);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenAtomicWake(IntPtr module, IntPtr ptr, IntPtr wakeCount);

        // Functions

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr BinaryenAddFunction(IntPtr module, string name, IntPtr type, Type[] varTypes, uint numVarTypes, IntPtr body);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr BinaryenGetFunction(IntPtr module, string name);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void BinaryenRemoveFunction(IntPtr module, string name);

        // Start function

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern void BinaryenSetStart(IntPtr module, IntPtr start);

        #endregion
    }
}
