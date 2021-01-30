using System;
using System.Collections;
using System.Collections.Generic;
using Wasm;
using Wasm.Interpret;

namespace Spirare
{
    public class FileDescriptorBinding : BindingBase
    {
        private readonly FileDescriptorBinding1 fileDescriptorBinding = new FileDescriptorBinding1();

        public FileDescriptorBinding(Element element, ContentsStore store) : base(element, store)
        {
        }

        public override PredefinedImporter GenerateImporter()
        {
            var importer = new PredefinedImporter();


            importer.DefineFunction("fd_close",
                 new DelegateFunctionDefinition(
                     new WasmValueType[] { WasmValueType.Int32, },
                     new WasmValueType[] { WasmValueType.Int32, },
                     Close
                     ));
            importer.DefineFunction("fd_read",
                 new DelegateFunctionDefinition(
                     new WasmValueType[] { WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32 },
                     new WasmValueType[] { WasmValueType.Int32 },
                     Read
                     ));
            importer.DefineFunction("fd_write",
                 new DelegateFunctionDefinition(
                     new WasmValueType[] { WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32 }, new WasmValueType[] { WasmValueType.Int32 },
                     Write
                     ));

            return importer;
        }


        private IReadOnlyList<object> Invalid
        {
            get => ReturnValue.FromObject(-1);
        }
        private IReadOnlyList<object> ErrorResult
        {
            get => ReturnValue.FromObject(1);
        }

        private IReadOnlyList<object> Read(IReadOnlyList<object> arg)
        {
            var parser = new ArgumentParser(arg, ModuleInstance);
            var result = fileDescriptorBinding.Read(parser, MemoryReader);
            return ReturnValue.FromObject(result);
        }

        private IReadOnlyList<object> Write(IReadOnlyList<object> arg)
        {
            var parser = new ArgumentParser(arg, ModuleInstance);
            var result = fileDescriptorBinding.Write(parser, MemoryReader);
            return ReturnValue.FromObject(result);
        }

        private IReadOnlyList<object> Close(IReadOnlyList<object> arg)
        {
            var parser = new ArgumentParser(arg, ModuleInstance);
            var result = fileDescriptorBinding.Close(parser, MemoryReader);
            return ReturnValue.FromObject(result);
        }
    }
}
