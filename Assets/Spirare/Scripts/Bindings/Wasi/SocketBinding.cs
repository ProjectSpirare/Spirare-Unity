using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using Wasm;
using Wasm.Interpret;

namespace Spirare.WasmBinding.CsWasm
{
    public class SocketBinding : BindingBase
    {
        private readonly SocketBinding1 socketBinding = new SocketBinding1();

        public SocketBinding(Element element, ContentsStore store) : base(element, store)
        {
        }

        public override PredefinedImporter GenerateImporter()
        {
            var importer = new PredefinedImporter();

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

        private IReadOnlyList<object> Connect(IReadOnlyList<object> arg)
        {
            var parser = new ArgumentParser(arg, ModuleInstance);
            var result = socketBinding.Connect(parser, MemoryReader);
            return ReturnValue.FromObject(result);
        }

        private IReadOnlyList<object> Receive(IReadOnlyList<object> arg)
        {
            var parser = new ArgumentParser(arg, ModuleInstance);
            var result = socketBinding.Receive(parser, MemoryReader);
            return ReturnValue.FromObject(result);
        }

        private IReadOnlyList<object> Send(IReadOnlyList<object> arg)
        {
            var parser = new ArgumentParser(arg, ModuleInstance);
            var result = socketBinding.Send(parser, MemoryReader);
            return ReturnValue.FromObject(result);
        }
    }
}
