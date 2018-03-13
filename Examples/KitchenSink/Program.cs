using Binaryen;
using Exception = System.Exception;
using static System.Console;

namespace KitchenSink
{
    class Program
    {
        // Kitchen sink, tests the whole API
        // https://github.com/WebAssembly/binaryen/blob/master/test/example/c-api-kitchen-sink.c

        static void Main(string[] args)
        {
            try
            {
                TestTypes();
                TestCore();
                TestUnreachable();
                TestRelooper();
                TestBinaries();
                TestInterpret();
                TestNonvalid();
            }
            catch (Exception ex)
            {
                WriteLine("ERROR:");
                WriteLine(ex);
            }

#if DEBUG
            WriteLine("Press any key to continue...");
            ReadKey(true);
#endif
        }

        // Tests

        static void TestTypes()
        {
            WriteLine("Type.None: {0}", (int)ValueType.None);
            WriteLine("Type.Int32: {0}", (int)ValueType.Int32);
            WriteLine("Type.Int64: {0}", (int)ValueType.Int64);
            WriteLine("Type.Float32: {0}", (int)ValueType.Float32);
            WriteLine("Type.Float64: {0}", (int)ValueType.Float64);
            WriteLine("Type.Unreachable: {0}", (int)ValueType.Unreachable);
            WriteLine("Type.Auto: {0}", (int)ValueType.Auto);
        }

        static void TestCore()
        {
            // Module creation
            using (var module = new Module())
            {
                // Literals and constants
                var constI32 = module.Const(1);
                var constI64 = module.Const(2L);
                var constF32 = module.Const(3.14f);
                var constF64 = module.Const(2.1828);
                var constF32Bits = module.Const(Literal.Float32Bits(0xFFFF1234));
                var constF64Bits = module.Const(Literal.Float64Bits(0xFFFF12345678ABCDL));

                var switchValueNames = new[] { "the-value" };
                var switchBodyNames = new[] { "the-nothing" };

                var callOperands2 = new[] { MakeInt32(module, 13), MakeFloat64(module, 3.7) };
                var callOperands4 = new[] { MakeInt32(module, 13), MakeInt64(module, 37L), MakeFloat32(module, 1.3f), MakeFloat64(module, 3.7) };
                var callOperands4b = new[] { MakeInt32(module, 13), MakeInt64(module, 37L), MakeFloat32(module, 1.3f), MakeFloat64(module, 3.7) };

                var parameters = new[] { ValueType.Int32, ValueType.Int64, ValueType.Float32, ValueType.Float64 };
                var iiIfF = module.AddFunctionType("iiIfF", ValueType.Int32, parameters);

                var temp1 = MakeInt32(module, 1);
                var temp2 = MakeInt32(module, 2);
                var temp3 = MakeInt32(module, 3);
                var temp4 = MakeInt32(module, 4);
                var temp5 = MakeInt32(module, 5);
                var temp6 = MakeInt32(module, 0);
                var temp7 = MakeInt32(module, 1);
                var temp8 = MakeInt32(module, 0);
                var temp9 = MakeInt32(module, 1);
                var temp10 = MakeInt32(module, 1);
                var temp11 = MakeInt32(module, 3);
                var temp12 = MakeInt32(module, 5);
                var temp13 = MakeInt32(module, 10);
                var temp14 = MakeInt32(module, 11);
                var temp15 = MakeInt32(module, 110);
                var temp16 = MakeInt64(module, 111);

                var valueList = new[] {
                    // Unary
                    MakeUnary(module, UnaryOperator.ClzInt32, ValueType.Int32),
                    MakeUnary(module, UnaryOperator.ClzInt64, ValueType.Int64),
                    MakeUnary(module, UnaryOperator.PopcntInt32, ValueType.Int32),
                    MakeUnary(module, UnaryOperator.NegFloat32, ValueType.Float32),
                    MakeUnary(module, UnaryOperator.AbsFloat64, ValueType.Float64),
                    MakeUnary(module, UnaryOperator.CeilFloat32, ValueType.Float32),
                    MakeUnary(module, UnaryOperator.FloorFloat64, ValueType.Float64),
                    MakeUnary(module, UnaryOperator.TruncFloat32, ValueType.Float32),
                    MakeUnary(module, UnaryOperator.NearestFloat32, ValueType.Float32),
                    MakeUnary(module, UnaryOperator.SqrtFloat64, ValueType.Float64),
                    MakeUnary(module, UnaryOperator.EqZInt32, ValueType.Int32),
                    MakeUnary(module, UnaryOperator.ExtendSInt32, ValueType.Int32),
                    MakeUnary(module, UnaryOperator.ExtendUInt32, ValueType.Int32),
                    MakeUnary(module, UnaryOperator.WrapInt64, ValueType.Int64),
                    MakeUnary(module, UnaryOperator.TruncSFloat32ToInt32, ValueType.Float32),
                    MakeUnary(module, UnaryOperator.TruncSFloat32ToInt64, ValueType.Float32),
                    MakeUnary(module, UnaryOperator.TruncUFloat32ToInt32, ValueType.Float32),
                    MakeUnary(module, UnaryOperator.TruncUFloat32ToInt64, ValueType.Float32),
                    MakeUnary(module, UnaryOperator.TruncSFloat64ToInt32, ValueType.Float64),
                    MakeUnary(module, UnaryOperator.TruncSFloat64ToInt64, ValueType.Float64),
                    MakeUnary(module, UnaryOperator.TruncUFloat64ToInt32, ValueType.Float64),
                    MakeUnary(module, UnaryOperator.TruncUFloat64ToInt64, ValueType.Float64),
                    MakeUnary(module, UnaryOperator.ReinterpretFloat32, ValueType.Float32),
                    MakeUnary(module, UnaryOperator.ReinterpretFloat64, ValueType.Float64),
                    MakeUnary(module, UnaryOperator.ConvertSInt32ToFloat32, ValueType.Int32),
                    MakeUnary(module, UnaryOperator.ConvertSInt32ToFloat64, ValueType.Int32),
                    MakeUnary(module, UnaryOperator.ConvertUInt32ToFloat32, ValueType.Int32),
                    MakeUnary(module, UnaryOperator.ConvertUInt32ToFloat64, ValueType.Int32),
                    MakeUnary(module, UnaryOperator.ConvertSInt64ToFloat32, ValueType.Int64),
                    MakeUnary(module, UnaryOperator.ConvertSInt64ToFloat64, ValueType.Int64),
                    MakeUnary(module, UnaryOperator.ConvertUInt64ToFloat32, ValueType.Int64),
                    MakeUnary(module, UnaryOperator.ConvertUInt64ToFloat64, ValueType.Int64),
                    MakeUnary(module, UnaryOperator.PromoteFloat32, ValueType.Float32),
                    MakeUnary(module, UnaryOperator.DemoteFloat64, ValueType.Float64),
                    MakeUnary(module, UnaryOperator.ReinterpretInt32, ValueType.Int32),
                    MakeUnary(module, UnaryOperator.ReinterpretInt64, ValueType.Int64),

                    // Binary
                    MakeBinary(module, BinaryOperator.AddInt32, ValueType.Int32),
                    MakeBinary(module, BinaryOperator.SubFloat64, ValueType.Float64),
                    MakeBinary(module, BinaryOperator.DivSInt32, ValueType.Int32),
                    MakeBinary(module, BinaryOperator.DivUInt64, ValueType.Int64),
                    MakeBinary(module, BinaryOperator.RemSInt64, ValueType.Int64),
                    MakeBinary(module, BinaryOperator.RemUInt32, ValueType.Int32),
                    MakeBinary(module, BinaryOperator.AndInt32, ValueType.Int32),
                    MakeBinary(module, BinaryOperator.OrInt64, ValueType.Int64),
                    MakeBinary(module, BinaryOperator.XorInt32, ValueType.Int32),
                    MakeBinary(module, BinaryOperator.ShlInt64, ValueType.Int64),
                    MakeBinary(module, BinaryOperator.ShrUInt64, ValueType.Int64),
                    MakeBinary(module, BinaryOperator.ShrSInt32, ValueType.Int32),
                    MakeBinary(module, BinaryOperator.RotLInt32, ValueType.Int32),
                    MakeBinary(module, BinaryOperator.RotRInt64, ValueType.Int64),
                    MakeBinary(module, BinaryOperator.DivFloat32, ValueType.Float32),
                    MakeBinary(module, BinaryOperator.CopySignFloat64, ValueType.Float64),
                    MakeBinary(module, BinaryOperator.MinFloat32, ValueType.Float32),
                    MakeBinary(module, BinaryOperator.MaxFloat64, ValueType.Float64),
                    MakeBinary(module, BinaryOperator.EqInt32, ValueType.Int32),
                    MakeBinary(module, BinaryOperator.NeFloat32, ValueType.Float32),
                    MakeBinary(module, BinaryOperator.LtSInt32, ValueType.Int32),
                    MakeBinary(module, BinaryOperator.LtUInt64, ValueType.Int64),
                    MakeBinary(module, BinaryOperator.LeSInt64, ValueType.Int64),
                    MakeBinary(module, BinaryOperator.LeUInt32, ValueType.Int32),
                    MakeBinary(module, BinaryOperator.GtSInt64, ValueType.Int64),
                    MakeBinary(module, BinaryOperator.GtUInt32, ValueType.Int32),
                    MakeBinary(module, BinaryOperator.GeSInt32, ValueType.Int32),
                    MakeBinary(module, BinaryOperator.GeUInt64, ValueType.Int64),
                    MakeBinary(module, BinaryOperator.LtFloat32, ValueType.Float32),
                    MakeBinary(module, BinaryOperator.LeFloat64, ValueType.Float64),
                    MakeBinary(module, BinaryOperator.GtFloat64, ValueType.Float64),
                    MakeBinary(module, BinaryOperator.GeFloat32, ValueType.Float32),

                    // All the rest
                    module.Block(ValueType.Auto), // Block with no name and no type
                    module.If(temp1, temp2, temp3),
                    module.If(temp4, temp5),
                    module.Loop("in", MakeInt32(module, 0)),
                    module.Loop(MakeInt32(module, 0)),
                    module.Break("the-value", temp6, temp7),
                    module.Break("the-nothing", condition: MakeInt32(module, 2)),
                    module.Break("the-value", value: MakeInt32(module, 3)),
                    module.Break("the-nothing"),
                    module.Switch(switchValueNames, "the-value", temp8, temp9),
                    module.Switch(switchBodyNames, "the-nothing", MakeInt32(module, 2)),
                    module.Unary(UnaryOperator.EqZInt32, // Check the output type of the call node
                        module.Call("kitchen()sinker", callOperands4, ValueType.Int32)
                    ),
                    module.Unary(UnaryOperator.EqZInt32, // check the output type of the call node
                        module.Unary(UnaryOperator.TruncSFloat32ToInt32,
                            module.CallImport("an-imported", callOperands2, ValueType.Float32)
                        )
                    ),
                    module.Unary(UnaryOperator.EqZInt32, // Check the output type of the call node
                        module.CallIndirect(MakeInt32(module, 2449), callOperands4b, "iiIfF")
                    ),
                    module.Drop(module.GetLocal(0, ValueType.Int32)),
                    module.SetLocal(0, MakeInt32(module, 101)),
                    module.Drop(module.TeeLocal(0, MakeInt32(module, 102))),
                    module.Load(4, false, 0, 0, ValueType.Int32, MakeInt32(module, 1)),
                    module.Load(2, true, 2, 1, ValueType.Int64, MakeInt32(module, 8)),
                    module.Load(4, false, 0, 0, ValueType.Float32, MakeInt32(module, 2)),
                    module.Load(8, false, 2, 8, ValueType.Float64, MakeInt32(module, 9)),
                    module.Store(4, 0, 0, temp13, temp14, ValueType.Int32),
                    module.Store(8, 2, 4, temp15, temp16, ValueType.Int64),
                    module.Select(temp10, temp11, temp12),
                    module.Return(MakeInt32(module, 1337)),
                    // TODO: Host
                    module.Nop(),
                    module.Unreachable(),
                };

                // Test printing a standalone expression
                valueList[3].Print();

                // Make the main body of our function, one block with a return value, and one without
                var value = module.Block("the-value", valueList, ValueType.Auto);
                var droppedValue = module.Drop(value);
                var nothing = module.Block("the-nothing", droppedValue, ValueType.Auto);
                var bodyList = new[] { nothing, MakeInt32(module, 42) };
                var body = module.Block("the-body", bodyList, ValueType.Auto);

                // Create the function
                var localTypes = new[] { ValueType.Int32 };
                var sinker = module.AddFunction("kitchen()sinker", iiIfF, localTypes, body);

                // Globals
                module.AddGlobal("a-global", ValueType.Int32, false, MakeInt32(module, 7));
                module.AddGlobal("a-mutable-global", ValueType.Float32, true, MakeFloat32(module, 7.5f));

                // Imports
                var iparamsList = new[] { ValueType.Int32, ValueType.Float64 };
                var fiF = module.AddFunctionType("fiF", ValueType.Float32, iparamsList);
                module.AddFunctionImport("an-imported", "module", "base", fiF);

                // Exports
                module.AddFunctionExport("kitchen()sinker", "kitchen_sinker");

                // Function table. One per module.
                var functions = new[] { sinker };
                module.SetFunctionTable(functions);

                // Memory. One per module.
                var segments = new[] {
                    new MemorySegment("hello, world", module.Const(10))
                };
                module.SetMemory(1, 256, "mem", segments);

                // Start function. One per module.
                var v = module.AddFunctionType("v", ValueType.None);
                var starter = module.AddFunction("starter", v, module.Nop());
                module.SetStart(starter);

                // Unnamed function type
                var noname = module.AddFunctionType(ValueType.None);

                // A bunch of our code needs drop(), auto-add it
                module.AutoDrop();

                // Verify it validates (note that the using block ensures disposal even with exceptions)
                if (!module.Validate()) throw new Exception("Module failed validation.");

                // Print it out
                module.Print();

                // Clean up the module (via using), which owns all the objects we created above
            }
        }

        static void TestUnreachable()
        {
            using (var module = new Module())
            {
                var i = module.AddFunctionType("i", ValueType.Int32);
                var I = module.AddFunctionType("I", ValueType.Int64);

                var body = module.CallIndirect(module.Unreachable(), "I");
                var fn = module.AddFunction("unreachable-fn", i, body);

                if (!module.Validate()) throw new Exception("Module failed validation.");

                module.Print();
            }
        }

        static void TestRelooper()
        {
            // TODO
        }

        static void TestBinaries()
        {
            Binary binary = null;

            // Create a module and write it to binary
            using (var module = new Module())
            {
                var parameters = new[] { ValueType.Int32, ValueType.Int32 };
                var iii = module.AddFunctionType("iii", ValueType.Int32, parameters);
                var x = module.GetLocal(0, ValueType.Int32);
                var y = module.GetLocal(1, ValueType.Int32);
                var add = module.Binary(BinaryOperator.AddInt32, x, y);
                var adder = module.AddFunction("adder", iii, add);

                Module.DebugInfo = true;
                binary = module.Emit();
                Module.DebugInfo = false;
            }

            if (binary == null) throw new Exception("Could not emit binary.");

            // Read the module from the binary
            using (var module = new Module(binary))
            {
                WriteLine("Module loaded from binary form:");
                module.Print();
            }
        }

        static void TestInterpret()
        {
            // TODO
        }

        static void TestNonvalid()
        {
            // TODO
        }

        static void TestTracing()
        {
            Tracing.Enable();
            TestCore();
            TestRelooper();
            Tracing.Disable();
        }

        // Helpers

        static Expression MakeUnary(Module module, UnaryOperator op, ValueType type)
        {
            switch (type)
            {
                case ValueType.Int32:
                    return module.Unary(op, module.Const(-10));

                case ValueType.Int64:
                    return module.Unary(op, module.Const(-22L));

                case ValueType.Float32:
                    return module.Unary(op, module.Const(-33.612f));

                case ValueType.Float64:
                    return module.Unary(op, module.Const(-9005.841));

                default:
                    throw new Exception("Unexpected unary type.");
            }
        }

        static Expression MakeBinary(Module module, BinaryOperator op, ValueType type)
        {
            // Use temp vars to ensure optimization doesn't change the order of operation in our trace recording
            Expression temp;

            switch (type)
            {
                case ValueType.Int32:
                    temp = module.Const(-11);
                    return module.Binary(op, module.Const(-10), temp);

                case ValueType.Int64:
                    temp = module.Const(-23L);
                    return module.Binary(op, module.Const(-22L), temp);

                case ValueType.Float32:
                    temp = module.Const(-62.5f);
                    return module.Binary(op, module.Const(-33.612f), temp);

                case ValueType.Float64:
                    temp = module.Const(-9007.333);
                    return module.Binary(op, module.Const(-9005.841), temp);

                default:
                    throw new Exception("Unexpected binary type.");
            }
        }

        static Expression MakeInt32(Module module, int x) => module.Const(x);

        static Expression MakeInt64(Module module, long x) => module.Const(x);

        static Expression MakeFloat32(Module module, float x) => module.Const(x);

        static Expression MakeFloat64(Module module, double x) => module.Const(x);

        static Expression MakeSomething(Module module) => MakeInt32(module, 1337);

        static Expression MakeDroppedInt32(Module module, int x) => module.Drop(module.Const(x));
    }
}
