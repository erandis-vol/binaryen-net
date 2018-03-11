﻿using Binaryen;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            // "Hello World" type example: create a function that adds two i32s and returns the result
            // https://github.com/WebAssembly/binaryen/blob/master/test/example/c-api-hello-world.c

            using (var module = new Module())
            {
                // Create a function type for i32(i32, i32)
                var parameters = new Type[] { Type.Int32, Type.Int32 };
                var iii = module.AddFunctionType("iii", Type.Int32, parameters);

                // Get the 1 and 0 arguments, and add them
                var x = module.GetLocal(0, Type.Int32);
                var y = module.GetLocal(1, Type.Int32);
                var add = module.Binary(BinaryOperator.AddInt32, x, y);

                // Create the add function
                var adder = module.AddFunction("adder", iii, add);

                // Print it out
                module.Print();

                // The module is disposed, along with every object that it owns
            }
        }
    }
}
