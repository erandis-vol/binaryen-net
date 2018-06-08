# binaryen.NET
This is a .NET binding of [binaryen](https://github.com/WebAssembly/binaryen).

## Usage

```csharp
using Binaryen;

namespace Example
{
    static class Program
    {
        static void Main(string[] args)
        {
            // Create a module
            using (var module = new Module())
            {
                // Create the "add" function
                module.AddFunction(
                    "add",
                    module.AddFunctionType("iii", ValueType.Int32, new[] { ValueType.Int32, ValueType.Int32 }),
                    module.Block(null, new[] {
                        module.SetLocal(2,
                            module.Binary(
                                BinaryOperator.AddInt32,
                                module.GetLocal(0, ValueType.Int32),
                                module.GetLocal(1, ValueType.Int32)
                            )
                        ),
                        module.Return(
                            module.GetLocal(2, ValueType.Int32)
                        )
                    })
                );
                
                // Create an export for our function
                module.AddFunctionExport("add", "add");
                
                // Optimize the module using default passes and levls
                module.Optimize();
                
                // Validate the module
                if (!module.Validate())
                {
                    // Validation error
                }
                
                // Generate binary format
                var binary = module.Emit();
                
                // Save it to a file
                System.IO.File.WriteAllBytes("add.wasm", binary.Bytes);
            }
        }
    }
}
```


