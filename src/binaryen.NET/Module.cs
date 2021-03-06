﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Binaryen
{
    /// <summary>Represents a module.</summary>
    /// <remarks>
    /// <para>
    ///     Modules contain lists of functions, imports, exports and types. The Add* methods create them on the module.
    ///     The module owns them and will free their memory when disposed of.
    /// </para>
    /// 
    /// <para>
    ///     Expressions are also allocated within and freed by modules. They are not created by Add* methods,
    ///     since they are not added directly to the module, instead, they are arguments to other expressions
    ///     (and then they are the children of that AST node), or to a function
    ///     (and then they are the body of that function).
    /// </para>
    /// 
    /// <para>A module can also contain a function table for indirect calls, a memory, and a start method.</para>
    /// </remarks>
    public class Module : ManualBaseObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Module"/> class.
        /// </summary>
        public Module()
            : base(BinaryenModuleCreate())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Module"/> class from the specified s-expression.
        /// </summary>
        /// <param name="text">The s-expression.</param>
        public Module(string text)
            : base(BinaryenModuleParse(text))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Module"/> class from the specified binary.
        /// </summary>
        /// <param name="data">The binary module.</param>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null.</exception>
        /// <exception cref="OutOfMemoryException">the module could not be created.</exception>
        public Module(byte[] data)
            : base(BinaryenModuleRead(data ?? throw new ArgumentNullException(nameof(data)), (uint)data.Length))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Module"/> class from the specified binary.
        /// </summary>
        /// <param name="binary">The binary module.</param>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null.</exception>
        /// <exception cref="OutOfMemoryException">the module could not be created.</exception>
        public Module(Binary binary)
            : base(BinaryenModuleRead((binary ?? throw new ArgumentNullException(nameof(binary))).Bytes, (uint)binary.Bytes.Length))
        { }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                BinaryenModuleDispose(Handle);
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Prints the module to STDOUT. Useful for debugging.
        /// </summary>
        public void Print()
        {
            AssertNotDisposed();
            BinaryenModulePrint(Handle);
        }

        /// <summary>
        /// Prints the module to STDOUT in asm.js syntax.
        /// </summary>
        public void PrintAsmjs()
        {
            AssertNotDisposed();
            BinaryenModulePrintAsmjs(Handle);
        }

        /// <summary>
        /// Validates the module.
        /// </summary>
        /// <returns><c>true</c> the module is valid; otherwise, <c>false</c>.</returns>
        public bool Validate()
        {
            return BinaryenModuleValidate(Handle) != 0;
        }

        /// <summary>
        /// Runs the standard optimization passes on the module.
        /// </summary>
        public void Optimize()
        {
            AssertNotDisposed();
            BinaryenModuleOptimize(Handle);
        }

        /// <summary>
        /// Runs the specified optimization passes on the module.
        /// </summary>
        /// <param name="passes">The optimization passes to run.</param>
        public void RunPasses(IEnumerable<string> passes)
        {
            if (passes != null && passes.Any())
            {
                RunPasses(passes.ToArray());
            }
        }

        /// <summary>
        /// Runs the specified optimization passes on the module.
        /// </summary>
        /// <param name="passes">The optimization passes to run.</param>
        public void RunPasses(string[] passes)
        {
            if (passes != null && passes.Length > 0)
            {
                BinaryenModuleRunPasses(Handle, passes, (uint)passes.Length);
            }
        }

        /// <summary>
        /// Enables automatic insertion of <c>drop</c> operations where needed.
        /// Lets you not worry about dropping when creating code.
        /// </summary>
        public void AutoDrop()
        {
            BinaryenModuleAutoDrop(Handle);
        }

        /// <summary>
        /// Executes the module in the Binaryen interpreter.
        /// </summary>
        public void Interpret()
        {
            AssertNotDisposed();
            BinaryenModuleInterpret(Handle);
        }

        /// <summary>
        /// Returns the module in binary format.
        /// </summary>
        public Binary Emit()
        {
            return Emit(null);
        }

        /// <summary>
        /// Returns the module in binary format. If <paramref name="sourceMapUrl"/> is null, source map generation is skipped.
        /// </summary>
        /// <param name="sourceMapUrl">The source map.</param>
        public Binary Emit(string sourceMapUrl)
        {
            AssertNotDisposed();

            // TODO: Memory leak?
            var result = BinaryenModuleAllocateAndWrite(Handle, sourceMapUrl);

            var bytes = new byte[result.BinaryBytes];
            Marshal.Copy(result.Binary, bytes, 0, (int)result.BinaryBytes);

            var sourceMap = sourceMapUrl != null ? Marshal.PtrToStringAnsi(result.SourceMap) : null;

            return new Binary(bytes, sourceMap);
        }

        /// <summary>
        /// Adds the specified debug info file name to the module and returns its index.
        /// </summary>
        /// <param name="filename">The debug info file.</param>
        /// <returns>The index of the file name.</returns>
        public uint AddDebugInfoFileName(string filename)
        {
            return BinaryenModuleAddDebugInfoFileName(Handle, filename);
        }

        /// <summary>
        /// Gets the debug info file name at the specified index.
        /// </summary>
        public string GetDebugFileName(uint index)
        {
            return Marshal.PtrToStringAnsi(BinaryenModuleGetDebugInfoFileName(Handle, index));
        }

        /// <summary>
        /// Adds a new function type.
        /// </summary>
        /// <param name="result">The return type.</param>
        /// <returns>A <see cref="Signature"/> representing the function type.</returns>
        /// <exception cref="OutOfMemoryException">the type could not be created.</exception>
        public Signature AddFunctionType(ValueType result)
        {
            return AddFunctionType(null, result, null);
        }

        /// <summary>
        /// Adds a new function type.
        /// </summary>
        /// <param name="result">The return type.</param>
        /// <param name="parameters">The parameter types.</param>
        /// <returns>A <see cref="Signature"/> representing the function type.</returns>
        /// <exception cref="OutOfMemoryException">the type could not be created.</exception>
        public Signature AddFunctionType(ValueType result, IEnumerable<ValueType> parameters)
        {
            return AddFunctionType(null, result, parameters);
        }

        /// <summary>
        /// Adds a new function type.
        /// </summary>
        /// <param name="name">The name of the type.</param>
        /// <param name="result">The return type.</param>
        /// <returns>A <see cref="Signature"/> representing the function type.</returns>
        /// <exception cref="OutOfMemoryException">the type could not be created.</exception>
        public Signature AddFunctionType(string name, ValueType result)
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
        public Signature AddFunctionType(string name, ValueType result, IEnumerable<ValueType> parameters)
        {
            if (parameters != null && parameters.Any())
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
        public Signature AddFunctionType(string name, ValueType result, ValueType[] parameters)
        {
            IntPtr signatureRef;

            if (parameters == null)
            {
                signatureRef = BinaryenAddFunctionType(Handle, name, result, null, 0u);
            }
            else
            {
                signatureRef = BinaryenAddFunctionType(Handle, name, result, parameters, (uint)parameters.Length);
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
        public Signature GetFunctionTypeBySignature(ValueType result, ValueType[] parameters)
        {
            var sig = BinaryenGetFunctionTypeBySignature(Handle, result, parameters, (uint)parameters.Length);
            return sig == IntPtr.Zero ? null : new Signature(sig);
        }

        /// <summary>
        /// Adds a new function.
        /// </summary>
        /// <param name="name">The name of the function.</param>
        /// <param name="signature">The parametric signature of the function.</param>
        /// <param name="body">The function body.</param>
        /// <returns>A <see cref="Function"/> instance.</returns>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="signature"/> or <paramref name="body"/> is null.</exception>
        public Function AddFunction(string name, Signature signature, Expression body)
        {
            return AddFunction(name, signature, null, body);
        }

        /// <summary>
        /// Adds a new function.
        /// </summary>
        /// <param name="name">The name of the function.</param>
        /// <param name="signature">The parametric signature of the function.</param>
        /// <param name="varTypes">Additional local variable types, specified in order.</param>
        /// <param name="body">The function body.</param>
        /// <returns>A <see cref="Function"/> instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="signature"/> or <paramref name="body"/> is null.</exception>
        public Function AddFunction(string name, Signature signature, IEnumerable<ValueType> varTypes, Expression body)
        {
            if (varTypes.Any())
            {
                return AddFunction(name, signature, varTypes.ToArray(), body);
            }
            else
            {
                return AddFunction(name, signature, null, body);
            }
        }

        /// <summary>
        /// Adds a new function.
        /// </summary>
        /// <param name="name">The name of the function.</param>
        /// <param name="signature">The parametric signature of the function.</param>
        /// <param name="varTypes">Additional local variable types, specified in order.</param>
        /// <param name="body">The function body.</param>
        /// <returns>A <see cref="Function"/> instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="signature"/> or <paramref name="body"/> is null.</exception>
        public Function AddFunction(string name, Signature signature, ValueType[] varTypes, Expression body)
        {
            if (signature == null || body == null)
                throw new ArgumentNullException(signature == null ? nameof(signature) : nameof(body));

            IntPtr funcRef;

            if (varTypes == null)
            {
                funcRef = BinaryenAddFunction(Handle, name, signature.Handle, null, 0u, body.Handle);
            }
            else
            {
                funcRef = BinaryenAddFunction(Handle, name, signature.Handle, varTypes, (uint)varTypes.Length, body.Handle);
            }

            if (funcRef == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Function(funcRef);
        }

        /// <summary>
        /// Returns the function with the specified name.
        /// </summary>
        /// <param name="name">The name of the function.</param>
        /// <returns>A <see cref="Function"/> instance. If there is not such function, returns <c>null</c>.</returns>
        public Function GetFunction(string name)
        {
            var ptr = BinaryenGetFunction(Handle, name);
            if (ptr == IntPtr.Zero)
                return null;
            else
                return new Function(ptr);
        }

        /// <summary>
        /// Removes the function with the specified name.
        /// </summary>
        /// <param name="name">The name of the function.</param>
        public void RemoveFunction(string name)
        {
            BinaryenRemoveFunction(Handle, name);
        }

        /// <summary>
        /// Adds the specified function import.
        /// </summary>
        /// <param name="name">The internal name.</param>
        /// <param name="externalModuleName">The external module name.</param>
        /// <param name="externalBaseName">The external base name.</param>
        /// <param name="signature">The parametric signature of the function.</param>
        [Obsolete("Use AddFunctionImport instead.")]
        public Import AddImport(string name, string externalModuleName, string externalBaseName, Signature signature)
        {
            return AddFunctionImport(name, externalModuleName, externalBaseName, signature);
        }

        /// <summary>
        /// Adds the specified function import.
        /// </summary>
        /// <param name="name">The internal name.</param>
        /// <param name="externalModuleName">The external module name.</param>
        /// <param name="externalBaseName">The external base name.</param>
        /// <param name="signature">The parametric signature of the function.</param>
        /// <returns>An <see cref="Import"/> instance representing the import.</returns>
        /// <exception cref="ArgumentNullException">a parameter is null.</exception>
        /// <exception cref="OutOfMemoryException">the import could not be created.</exception>
        public Import AddFunctionImport(string name, string externalModuleName, string externalBaseName, Signature signature)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (externalModuleName == null)
                throw new ArgumentNullException(nameof(externalModuleName));

            if (externalBaseName == null)
                throw new ArgumentNullException(nameof(externalBaseName));

            if (signature == null)
                throw new ArgumentNullException(nameof(signature));

            var importRef = BinaryenAddFunctionImport(Handle, name, externalModuleName, externalBaseName, signature.Handle);
            if (importRef == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Import(importRef);
        }

        /// <summary>
        /// Adds the specified table import.
        /// </summary>
        /// <param name="name">The internal name.</param>
        /// <param name="externalModuleName">The external module name.</param>
        /// <param name="externalBaseName">The external base name.</param>
        /// <returns>An <see cref="Import"/> instance representing the import.</returns>
        /// <exception cref="ArgumentNullException">a parameter is null.</exception>
        /// <exception cref="OutOfMemoryException">the import could not be created.</exception>
        public Import AddTableImport(string name, string externalModuleName, string externalBaseName)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (externalModuleName == null)
                throw new ArgumentNullException(nameof(externalModuleName));

            if (externalBaseName == null)
                throw new ArgumentNullException(nameof(externalBaseName));

            var importRef = BinaryenAddTableImport(Handle, name, externalModuleName, externalBaseName);
            if (importRef == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Import(importRef);
        }

        /// <summary>
        /// Adds the specified memory import.
        /// </summary>
        /// <param name="name">The internal name.</param>
        /// <param name="externalModuleName">The external module name.</param>
        /// <param name="externalBaseName">The external base name.</param>
        /// <returns>An <see cref="Import"/> instance representing the import.</returns>
        /// <exception cref="ArgumentNullException">a parameter is null.</exception>
        /// <exception cref="OutOfMemoryException">the import could not be created.</exception>
        public Import AddMemoryImport(string name, string externalModuleName, string externalBaseName)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (externalModuleName == null)
                throw new ArgumentNullException(nameof(externalModuleName));

            if (externalBaseName == null)
                throw new ArgumentNullException(nameof(externalBaseName));

            var importRef = BinaryenAddMemoryImport(Handle, name, externalModuleName, externalBaseName);
            if (importRef == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Import(importRef);
        }

        /// <summary>
        /// Adds the specified global import.
        /// </summary>
        /// <param name="name">The internal name.</param>
        /// <param name="externalModuleName">The external module name.</param>
        /// <param name="externalBaseName">The external base name.</param>
        /// <param name="globalType">The global type.</param>
        /// <returns>An <see cref="Import"/> instance representing the import.</returns>
        /// <exception cref="ArgumentNullException">a parameter is null.</exception>
        /// <exception cref="OutOfMemoryException">the import could not be created.</exception>
        public Import AddGlobalImport(string name, string externalModuleName, string externalBaseName, ValueType globalType)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (externalModuleName == null)
                throw new ArgumentNullException(nameof(externalModuleName));

            if (externalBaseName == null)
                throw new ArgumentNullException(nameof(externalBaseName));

            var importRef = BinaryenAddGlobalImport(Handle, name, externalModuleName, externalModuleName, globalType);
            if (importRef == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Import(importRef);
        }

        /// <summary>
        /// Removes the specified import.
        /// </summary>
        /// <param name="name">The name of the import to remove.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is null.</exception>
        public void RemoveImport(string name)
        {
            BinaryenRemoveImport(Handle, name ?? throw new ArgumentNullException(nameof(name)));
        }

        /// <summary>
        /// Add the specified function export.
        /// </summary>
        /// <param name="name">The internal name.</param>
        /// <param name="externalName">The external name.</param>
        /// <returns>An <see cref="Export"/> instance representing the export.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> or <paramref name="externalName"/> is null.</exception>
        /// <exception cref="OutOfMemoryException">the export could not be created.</exception>
        [Obsolete("Use AddFunctionExport instead.")]
        public Export AddExport(string name, string externalName)
        {
            return AddFunctionExport(name, externalName);
        }

        /// <summary>
        /// Adds the specified function export.
        /// </summary>
        /// <param name="name">The internal name.</param>
        /// <param name="externalName">The external name.</param>
        /// <returns>An <see cref="Export"/> instance representing the export.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> or <paramref name="externalName"/> is null.</exception>
        /// <exception cref="OutOfMemoryException">the export could not be created.</exception>
        public Export AddFunctionExport(string name, string externalName)
        {
            if (name == null || externalName == null)
                throw new ArgumentNullException(name == null ? nameof(name) : nameof(externalName));

            var exportRef = BinaryenAddFunctionExport(Handle, name, externalName);
            if (exportRef == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Export(exportRef);
        }

        /// <summary>
        /// Adds the specified table export.
        /// </summary>
        /// <param name="name">The internal name.</param>
        /// <param name="externalName">The external name.</param>
        /// <returns>An <see cref="Export"/> instance representing the export.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> or <paramref name="externalName"/> is null.</exception>
        /// <exception cref="OutOfMemoryException">the export could not be created.</exception>
        public Export AddTableExport(string name, string externalName)
        {
            if (name == null || externalName == null)
                throw new ArgumentNullException(name == null ? nameof(name) : nameof(externalName));

            var exportRef = BinaryenAddTableExport(Handle, name, externalName);
            if (exportRef == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Export(exportRef);
        }

        /// <summary>
        /// Adds the specified memory export.
        /// </summary>
        /// <param name="name">The internal name.</param>
        /// <param name="externalName">The external name.</param>
        /// <returns>An <see cref="Export"/> instance representing the export.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> or <paramref name="externalName"/> is null.</exception>
        /// <exception cref="OutOfMemoryException">the export could not be created.</exception>
        public Export AddMemoryExport(string name, string externalName)
        {
            if (name == null || externalName == null)
                throw new ArgumentNullException(name == null ? nameof(name) : nameof(externalName));

            var exportRef = BinaryenAddMemoryExport(Handle, name, externalName);
            if (exportRef == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Export(exportRef);
        }

        /// <summary>
        /// Adds the specified global export.
        /// </summary>
        /// <param name="name">The internal name.</param>
        /// <param name="externalName">The external name.</param>
        /// <returns>An <see cref="Export"/> instance representing the export.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> or <paramref name="externalName"/> is null.</exception>
        /// <exception cref="OutOfMemoryException">the export could not be created.</exception>
        public Export AddGlobalExport(string name, string externalName)
        {
            if (name == null || externalName == null)
                throw new ArgumentNullException(name == null ? nameof(name) : nameof(externalName));

            var exportRef = BinaryenAddGlobalExport(Handle, name, externalName);
            if (exportRef == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Export(exportRef);
        }

        /// <summary>
        /// Removes the specified export.
        /// </summary>
        /// <param name="externalName">The external name of the export to remove.</param>
        public void RemoveExport(string externalName)
        {
            BinaryenRemoveExport(Handle, externalName);
        }

        /// <summary>
        /// Adds the specified global type.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="mutable">Determines whether the type is mutable.</param>
        /// <param name="init">The initial value expression.</param>
        public Global AddGlobal(string name, ValueType type, bool mutable, Expression init)
        {
            var globalRef = BinaryenAddGlobal(Handle, name, type, (sbyte)(mutable ? 1 : 0), init.Handle);
            if (globalRef == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Global(name, type, mutable, init);
        }

        /// <summary>
        /// Sets the function table for the module. There can only be one.
        /// </summary>
        /// <param name="functions">The functions.</param>
        /// <exception cref="ArgumentNullException"><paramref name="functions"/> is null.</exception>
        public void SetFunctionTable(IEnumerable<Function> functions)
        {
            if (functions != null && functions.Any())
            {
                SetFunctionTable(functions.ToArray());
            }
            else
            {
                throw new ArgumentNullException(nameof(functions));
            }
        }

        /// <summary>
        /// Sets the function table for the module. There can only be one.
        /// </summary>
        /// <param name="functions">The functions.</param>
        /// <exception cref="ArgumentNullException"><paramref name="functions"/> is null.</exception>
        public void SetFunctionTable(Function[] functions)
        {
            if (functions == null)
                throw new ArgumentNullException(nameof(functions));

            BinaryenSetFunctionTable(Handle, functions.Select(x => x.Handle).ToArray(), (uint)functions.Length);
        }

        /// <summary>
        /// Sets the memory for the module. There can only be one.
        /// </summary>
        /// <param name="initial">The initial size of the memory.</param>
        /// <param name="maximum">The maximum size of the memory.</param>
        /// <param name="exportName">The export name.</param>
        /// <param name="segments">The memory segments.</param>
        public void SetMemory(uint initial, uint maximum, string exportName, MemorySegment[] segments)
        {
            if (segments == null)
                throw new ArgumentNullException(nameof(segments));

            string BytesToString(byte[] bytes)
            {
                var sb = new StringBuilder();
                foreach (var b in bytes)
                {
                    sb.Append((char)b);
                }
                return sb.ToString();
            }

            var segmentData = segments.Select(x => BytesToString(x.Data)).ToArray();
            var segmentOffsets = segments.Select(x => x.Offset.Handle).ToArray();
            var segmentSizes = segments.Select(x => x.Size).ToArray();

            BinaryenSetMemory(Handle, initial, maximum, exportName, segmentData, segmentOffsets, segmentSizes, (uint)segments.Length);
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

            BinaryenSetStart(Handle, start.Handle);
        }

        /// <summary>
        /// Creates a block <see cref="Expression"/>.
        /// </summary>
        /// <param name="type">The result type. If set to <see cref="ValueType.Auto"/>, it will be determined automatically.</param>
        /// <returns>An <see cref="Expression"/> instance.</returns>
        /// <exception cref="OutOfMemoryException">the expression could not be created.</exception>
        public Expression Block(ValueType type = ValueType.Auto)
        {
            var blockRef = BinaryenBlock(Handle, null, null, 0u, type);
            if (blockRef == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(blockRef);
        }

        /// <summary>
        /// Creates a block <see cref="Expression"/>.
        /// </summary>
        /// <param name="label">The block label. Can be <c>null</c>.</param>
        /// <param name="children">The block body.</param>
        /// <param name="type">The result type. If set to <see cref="ValueType.Auto"/>, it will be determined automatically.</param>
        /// <returns>An <see cref="Expression"/> instance.</returns>
        /// <exception cref="OutOfMemoryException">the expression could not be created.</exception>
        public Expression Block(string label, Expression body, ValueType type = ValueType.Auto)
        {
            return Block(label, new [] { body }, type);
        }

        /// <summary>
        /// Creates a block <see cref="Expression"/>.
        /// </summary>
        /// <param name="label">The block label. Can be <c>null</c>.</param>
        /// <param name="children">The block body.</param>
        /// <param name="type">The result type. If set to <see cref="ValueType.Auto"/>, it will be determined automatically.</param>
        /// <returns>An <see cref="Expression"/> instance.</returns>
        /// <exception cref="OutOfMemoryException">the expression could not be created.</exception>
        public Expression Block(string label, IEnumerable<Expression> children, ValueType type = ValueType.Auto)
        {
            IntPtr blockRef;

            if (children != null && children.Any())
            {
                var childrenHandles = children.Select(x => x.Handle).ToArray();

                blockRef = BinaryenBlock(Handle, label, childrenHandles, (uint)childrenHandles.Length, type);
            }
            else
            {
                blockRef = BinaryenBlock(Handle, label, null, 0u, type);
            }

            if (blockRef == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(blockRef);
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

            var @if = BinaryenIf(Handle, conditionHandle, ifTrueHandle, ifFalseHandle);
            if (@if == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(@if);
        }

        /// <summary>
        /// Creates a loop <see cref="Expression"/>.
        /// </summary>
        /// <param name="body">The loop body.</param>
        /// <returns>An <see cref="Expression"/> instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="body"/> is null.</exception>
        /// <exception cref="OutOfMemoryException">the expression could not be created.</exception>
        public Expression Loop(Expression body)
        {
            return Loop(null, body);
        }

        /// <summary>
        /// Creates a loop <see cref="Expression"/>.
        /// </summary>
        /// <param name="label">The loop label. Can be <c>null</c>.</param>
        /// <param name="body">The loop body.</param>
        /// <returns>An <see cref="Expression"/> instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="body"/> is null.</exception>
        /// <exception cref="OutOfMemoryException">the expression could not be created.</exception>
        public Expression Loop(string label, Expression body)
        {
            if (body == null)
                throw new ArgumentNullException(nameof(body));

            var loop = BinaryenLoop(Handle, label, body.Handle);
            if (loop == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(loop);
        }

        public Expression Break(string label, Expression condition = null, Expression value = null)
        {
            // value AND condition can be NULL
            if (label == null)
                throw new ArgumentNullException(nameof(label));

            var @break = BinaryenBreak(Handle, label, condition == null ? IntPtr.Zero : condition.Handle,
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

            var @switch = BinaryenSwitch(Handle, labels, (uint)labels.Length, defaultLabel, condition.Handle,
                value == null ? IntPtr.Zero : value.Handle);
            if (@switch == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(@switch);
        }

        public Expression Call(string target, IEnumerable<Expression> operands, ValueType returnType)
        {
            var operandHandles = operands.Select(x => x.Handle).ToArray();

            var call = BinaryenCall(Handle, target, operandHandles, (uint)operandHandles.Length, returnType);
            if (call == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(call);
        }

        public Expression CallImport(string target, IEnumerable<Expression> operands, ValueType returnType)
        {
            var operandHandles = operands.Select(x => x.Handle).ToArray();

            var call = BinaryenCallImport(Handle, target, operandHandles, (uint)operandHandles.Length, returnType);
            if (call == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(call);
        }

        public Expression CallIndirect(Expression target, string type)
        {
            return CallIndirect(target, null, type);
        }

        public Expression CallIndirect(Expression target, IEnumerable<Expression> operands, string type)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            IntPtr callRef;

            if (operands != null && operands.Any())
            {
                var operandHandles = operands.Select(x => x.Handle).ToArray();

                callRef = BinaryenCallIndirect(Handle, target.Handle, operandHandles, (uint)operandHandles.Length, type);
            }
            else
            {
                callRef = BinaryenCallIndirect(Handle, target.Handle, null, 0u, type);
            }

            if (callRef == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(callRef);
        }

        /// <summary>
        /// Creates a get_local <see cref="Expression"/>. The type must be specified as the local may not have been declared yet.
        /// </summary>
        /// <param name="index">The local index.</param>
        /// <param name="type">The type of the local.</param>
        /// <returns>An <see cref="Expression"/> instance.</returns>
        /// <exception cref="OutOfMemoryException">the expression could not be created.</exception>
        public Expression GetLocal(uint index, ValueType type)
        {
            var expr = BinaryenGetLocal(Handle, index, type);
            if (expr == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(expr);
        }

        /// <summary>
        /// Creates a set_local <see cref="Expression"/> with the specified value.
        /// </summary>
        /// <param name="index">The local index.</param>
        /// <param name="value">The value to set.</param>
        /// <returns>An <see cref="Expression"/> instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        /// <exception cref="OutOfMemoryException">the expression could not be created.</exception>
        public Expression SetLocal(uint index, Expression value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var expr = BinaryenSetLocal(Handle, index, value.Handle);
            if (expr == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(expr);
        }

        public Expression TeeLocal(uint index, Expression value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var expr = BinaryenTeeLocal(Handle, index, value.Handle);
            if (expr == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(expr);
        }

        public Expression GetGlobal(string name, ValueType type)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var expr = BinaryenGetGlobal(Handle, name, type);
            if (expr == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(expr);
        }

        public Expression SetGlobal(string name, Expression value)
        {
            if (value == null || name == null)
                throw new ArgumentNullException(value == null ? nameof(value) : nameof(name));

            var expr = BinaryenSetGlobal(Handle, name, value.Handle);
            if (expr == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(expr);
        }

        public Expression Load(uint bytes, bool signed, uint offset, ValueType type, Expression ptr)
        {
            return Load(bytes, signed, offset, 0, type, ptr);
        }

        public Expression Load(uint bytes, bool signed, uint offset, uint align, ValueType type, Expression ptr)
        {
            if (ptr == null)
                throw new ArgumentNullException(nameof(ptr));

            var load = BinaryenLoad(Handle, bytes, (sbyte)(signed ? 1 : 0), offset, align, type, ptr.Handle);
            if (load == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(load);
        }

        public Expression Store(uint bytes, uint offset, Expression ptr, Expression value, ValueType type)
        {
            return Store(bytes, offset, 0, ptr, value, type);
        }

        public Expression Store(uint bytes, uint offset, uint align, Expression ptr, Expression value, ValueType type)
        {
            if (ptr == null || value == null)
                throw new ArgumentNullException(ptr == null ? nameof(ptr) : nameof(value));

            var expr = BinaryenStore(Handle, bytes, offset, align, ptr.Handle, value.Handle, type);
            if (expr == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(expr);
        }

        public Expression Const(int value)
        {
            return Const(Literal.Int32(value));
        }

        public Expression Const(long value)
        {
            return Const(Literal.Int64(value));
        }

        public Expression Const(float value)
        {
            return Const(Literal.Float32(value));
        }

        public Expression Const(double value)
        {
            return Const(Literal.Float64(value));
        }

        public Expression Const(Literal value)
        {
            var expr = BinaryenConst(Handle, value);
            if (expr == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(expr);
        }

        public Expression Unary(UnaryOperator op, Expression value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var expr = BinaryenUnary(Handle, op, value.Handle);
            if (expr == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(expr);
        }

        public Expression Binary(BinaryOperator op, Expression left, Expression right)
        {
            if (left == null || right == null)
                throw new ArgumentNullException(left == null ? nameof(left) : nameof(right));

            var expr = BinaryenBinary(Handle, op, left.Handle, right.Handle);
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

            var expr = BinaryenSelect(Handle, condition.Handle, ifTrue.Handle, ifFalse.Handle);
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

            var expr = BinaryenDrop(Handle, value.Handle);
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
            var expr = BinaryenReturn(Handle, value == null ? IntPtr.Zero : value.Handle);
            if (expr == null)
                throw new OutOfMemoryException();

            return new Expression(expr);
        }

        public Expression Host(HostOperator op, string name, IEnumerable<Expression> operands)
        {
            // name can be NULL
            var operandHandles = operands.Select(x => x.Handle).ToArray();

            var host = BinaryenHost(Handle, op, name, operandHandles, (uint)operandHandles.Length);
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
            var expr = BinaryenNop(Handle);
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
            var expr = BinaryenUnreachable(Handle);
            if (expr == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(expr);
        }

        public Expression AtomicLoad(uint bytes, uint offset, ValueType type, Expression ptr)
        {
            if (ptr == null)
                throw new ArgumentNullException(nameof(ptr));

            var expr = BinaryenAtomicLoad(Handle, bytes, offset, type, ptr.Handle);
            if (expr == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(expr);
        }

        public Expression AtomicStore(uint bytes, uint offset, Expression ptr, Expression value, ValueType type)
        {
            if (ptr == null || value == null)
                throw new ArgumentNullException(ptr == null ? nameof(ptr) : nameof(value));

            var expr = BinaryenAtomicStore(Handle, bytes, offset, ptr.Handle, value.Handle, type);
            if (expr == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(expr);
        }

        public Expression AtomicReadModifyWrite(AtomicOperator op, uint bytes, uint offset, Expression ptr, Expression value, ValueType type)
        {
            if (ptr == null || value == null)
                throw new ArgumentNullException(ptr == null ? nameof(ptr) : nameof(value));

            var expr = BinaryenAtomicRMW(Handle, op, bytes, offset, ptr.Handle, value.Handle, type);
            if (expr == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(expr);
        }

        public Expression AtomicCompareExchange(uint bytes, uint offset, Expression ptr, Expression expected, Expression replacement, ValueType type)
        {
            if (ptr == null)
                throw new ArgumentNullException(nameof(ptr));

            if (expected == null)
                throw new ArgumentNullException(nameof(expected));

            if (replacement == null)
                throw new ArgumentNullException(nameof(replacement));

            var expr = BinaryenAtomicCmpxchg(Handle, bytes, offset, ptr.Handle, expected.Handle, replacement.Handle, type);
            if (expr == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(expr);
        }

        public Expression AtomicWait(Expression ptr, Expression expected, Expression timeout, ValueType type)
        {
            if (ptr == null)
                throw new ArgumentNullException(nameof(ptr));

            if (expected == null)
                throw new ArgumentNullException(nameof(expected));

            if (timeout == null)
                throw new ArgumentNullException(nameof(timeout));

            var expr = BinaryenAtomicWait(Handle, ptr.Handle, expected.Handle, timeout.Handle, type);
            if (expr == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(expr);
        }

        public Expression AtomicWake(Expression ptr, Expression wakeCount)
        {
            if (ptr == null || wakeCount == null)
                throw new ArgumentNullException(ptr == null ? nameof(ptr) : nameof(wakeCount));

            var expr = BinaryenAtomicWake(Handle, ptr.Handle, wakeCount.Handle);
            if (expr == IntPtr.Zero)
                throw new OutOfMemoryException();

            return new Expression(expr);
        }

        /// <summary>
        /// Gets or sets the global optimize level. 0, 1, 2 correspond to -O0, -O1, -O2 (default), etc.
        /// </summary>
        public static int OptimzeLevel
        {
            get => BinaryenGetOptimizeLevel();
            set => BinaryenSetOptimizeLevel(value);
        }

        /// <summary>
        /// Gets or sets the global shrink level. 0, 1, 2 correspond to -O0, -Os (default), -Oz.
        /// </summary>
        public static int ShrinkLevel
        {
            get => BinaryenGetShrinkLevel();
            set => BinaryenSetShrinkLevel(value);
        }

        /// <summary>
        /// Gets or sets whether debug information is emitted to binaries.
        /// </summary>
        public static bool DebugInfo
        {
            get => BinaryenGetDebugInfo() != 0;
            set => BinaryenSetDebugInfo(value ? 1 : 0);
        }

        #region Imports

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenModuleCreate();

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern void BinaryenModuleDispose(IntPtr handle);

        // Operations

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr BinaryenModuleParse(string text);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern void BinaryenModulePrint(IntPtr module);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern void BinaryenModulePrintAsmjs(IntPtr module);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern int BinaryenModuleValidate(IntPtr module);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern void BinaryenModuleOptimize(IntPtr module);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern int BinaryenGetOptimizeLevel();

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern void BinaryenSetOptimizeLevel(int level);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern int BinaryenGetShrinkLevel();

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern void BinaryenSetShrinkLevel(int level);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern int BinaryenGetDebugInfo();

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern void BinaryenSetDebugInfo(int on);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern void BinaryenModuleRunPasses(IntPtr module, string[] passes, uint numPasses);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern void BinaryenModuleAutoDrop(IntPtr module);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenModuleRead(byte[] input, uint inputSize);

        [StructLayout(LayoutKind.Sequential)]
        private struct AllocateAndWriteResult
        {
            public IntPtr Binary;
            public uint BinaryBytes;
            public IntPtr SourceMap;
        }

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern AllocateAndWriteResult BinaryenModuleAllocateAndWrite(IntPtr module, string sourceMapUrl);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern void BinaryenModuleInterpret(IntPtr module);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern uint BinaryenModuleAddDebugInfoFileName(IntPtr module, string filename);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern /*const char**/ IntPtr BinaryenModuleGetDebugInfoFileName(IntPtr module, uint index);

        // Function types

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr BinaryenAddFunctionType(IntPtr module, string name, ValueType result, ValueType[] paramTypes, uint numParams);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenGetFunctionTypeBySignature(IntPtr module, ValueType result, ValueType[] paramTypes, uint numParams);

        // Expression creation

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr BinaryenBlock(IntPtr module, string name, IntPtr[] children, uint numChildren, ValueType type);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenIf(IntPtr module, IntPtr condition, IntPtr ifTrue, IntPtr ifFalse);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr BinaryenLoop(IntPtr module, string @in, IntPtr body);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr BinaryenBreak(IntPtr module, string name, IntPtr condition, IntPtr value);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr BinaryenSwitch(IntPtr module, string[] names, uint numNames, string defaultName, IntPtr condition, IntPtr value);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr BinaryenCall(IntPtr module, string target, IntPtr[] operands, uint numOperands, ValueType returnType);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr BinaryenCallImport(IntPtr module, string target, IntPtr[] operands, uint numOperands, ValueType returnType);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr BinaryenCallIndirect(IntPtr module, IntPtr target, IntPtr[] operands, uint numOperands, string type);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenGetLocal(IntPtr module, uint index, ValueType type);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenSetLocal(IntPtr module, uint index, IntPtr value);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenTeeLocal(IntPtr module, uint index, IntPtr value);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr BinaryenGetGlobal(IntPtr module, string name, ValueType type);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr BinaryenSetGlobal(IntPtr module, string name, IntPtr value);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenLoad(IntPtr module, uint bytes, sbyte signed, uint offset, uint align, ValueType type, IntPtr ptr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenStore(IntPtr module, uint bytes, uint offset, uint align, IntPtr ptr, IntPtr value, ValueType type);

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
        private static extern IntPtr BinaryenAtomicLoad(IntPtr module, uint bytes, uint offset, ValueType type, IntPtr ptr);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenAtomicStore(IntPtr module, uint bytes, uint offset, IntPtr ptr, IntPtr value, ValueType type);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenAtomicRMW(IntPtr module, AtomicOperator op, uint bytes, uint offset, IntPtr ptr, IntPtr value, ValueType type);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenAtomicCmpxchg(IntPtr module, uint bytes, uint offset, IntPtr ptr, IntPtr expected, IntPtr replacement, ValueType type);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenAtomicWait(IntPtr module, IntPtr ptr, IntPtr expected, IntPtr timeout, ValueType type);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr BinaryenAtomicWake(IntPtr module, IntPtr ptr, IntPtr wakeCount);

        // Functions

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr BinaryenAddFunction(IntPtr module, string name, IntPtr type, ValueType[] varTypes, uint numVarTypes, IntPtr body);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr BinaryenGetFunction(IntPtr module, string name);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void BinaryenRemoveFunction(IntPtr module, string name);

        // Imports

        //[DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        //private static extern /*WASM_DEPRECATED*/ IntPtr BinaryenAddImport(IntPtr module, string internalName, string externalModuleName, string externalBaseName, IntPtr type);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr BinaryenAddFunctionImport(IntPtr module, string internalName, string externalModuleName, string externalBaseName, IntPtr functionType);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr BinaryenAddTableImport(IntPtr module, string internalName, string externalModuleName, string externalBaseName);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr BinaryenAddMemoryImport(IntPtr module, string internalName, string externalModuleName, string externalBaseName);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr BinaryenAddGlobalImport(IntPtr module, string internalName, string externalModuleName, string externalBaseName, ValueType globalType);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void BinaryenRemoveImport(IntPtr module, string internalName);

        // Exports

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr BinaryenAddFunctionExport(IntPtr module, string internalName, string externalName);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr BinaryenAddTableExport(IntPtr module, string internalName, string externalName);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr BinaryenAddMemoryExport(IntPtr module, string internalName, string externalName);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr BinaryenAddGlobalExport(IntPtr module, string internalName, string externalName);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void BinaryenRemoveExport(IntPtr module, string externalName);

        // Globals

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr BinaryenAddGlobal(IntPtr module, string name, ValueType type, sbyte mutable_, IntPtr init);

        // Function table

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern void BinaryenSetFunctionTable(IntPtr module, IntPtr[] funcs, uint numFuncs);

        // Memory

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void BinaryenSetMemory(IntPtr module, uint initial, uint maximum, string exportName,
            string[] segments, IntPtr[] segmentOffsets, uint[] segmentSizes, uint numSegments);

        // Start function

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern void BinaryenSetStart(IntPtr module, IntPtr start);

        #endregion
    }
}
