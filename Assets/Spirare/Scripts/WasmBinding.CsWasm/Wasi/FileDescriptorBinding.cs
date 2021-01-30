using System;
using System.Collections;
using System.Collections.Generic;
using Wasm;
using Wasm.Interpret;

namespace Spirare.WasmBinding.CsWasm
{
    public class FileDescriptorBinding : BindingBase
    {
        private readonly WasmBinding.FileDescriptorBinding fileDescriptorBinding = new WasmBinding.FileDescriptorBinding();

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
                     new WasmValueType[] { WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32 },
                     new WasmValueType[] { WasmValueType.Int32 },
                     Write
                     ));


            importer.DefineFunction("sock_connect",
                new DelegateFunctionDefinition(
                    new WasmValueType[] { WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32 },
                    new WasmValueType[] { WasmValueType.Int32, },
                    Connect
                    ));

            importer.DefineFunction("sock_send",
                 new DelegateFunctionDefinition(
                     new WasmValueType[] { WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32 },
                     new WasmValueType[] { WasmValueType.Int32, },
                     Send
                     ));

            importer.DefineFunction("sock_recv",
                 new DelegateFunctionDefinition(
                     new WasmValueType[] { WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32 },
                     new WasmValueType[] { WasmValueType.Int32, },
                     Receive
                     ));

            return importer;
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

        private IReadOnlyList<object> Connect(IReadOnlyList<object> arg)
        {
            var parser = new ArgumentParser(arg, ModuleInstance);
            var result = fileDescriptorBinding.Connect(parser, MemoryReader);
            return ReturnValue.FromObject(result);
        }

        private IReadOnlyList<object> Receive(IReadOnlyList<object> arg)
        {
            var parser = new ArgumentParser(arg, ModuleInstance);
            var result = fileDescriptorBinding.Receive(parser, MemoryReader);
            return ReturnValue.FromObject(result);
        }

        private IReadOnlyList<object> Send(IReadOnlyList<object> arg)
        {
            var parser = new ArgumentParser(arg, ModuleInstance);
            var result = fileDescriptorBinding.Send(parser, MemoryReader);
            return ReturnValue.FromObject(result);
        }

    }
}
