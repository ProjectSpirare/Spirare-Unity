using System;
using System.Collections;
using System.Collections.Generic;
using Wasm.Interpret;

namespace Spirare.WasmBinding.CsWasm
{
    public class ArgsBinding : BindingBase
    {
        private readonly WasmBinding.ArgsBinding argsBinding;

        public ArgsBinding(Element element, ContentsStore store, List<string> args, List<string> envs) : base(element, store)
        {
            argsBinding = new WasmBinding.ArgsBinding(args, envs);
        }

        public override PredefinedImporter GenerateImporter()
        {
            var importer = new PredefinedImporter();

            importer.DefineFunction("args_get",
                 new DelegateFunctionDefinition(
                     ValueType.PointerAndPointer,
                     ValueType.Short,
                     ArgsGet
                     ));
            importer.DefineFunction("args_sizes_get",
                 new DelegateFunctionDefinition(
                     ValueType.PointerAndPointer,
                     ValueType.Short,
                     ArgsSizesGet
                     ));
            importer.DefineFunction("environ_get",
                 new DelegateFunctionDefinition(
                     ValueType.PointerAndPointer,
                     ValueType.Short,
                     EnvironGet
                     ));
            importer.DefineFunction("environ_sizes_get",
                 new DelegateFunctionDefinition(
                     ValueType.PointerAndPointer,
                     ValueType.Short,
                     EnvironSizesGet
                     ));
            return importer;
        }

        private IReadOnlyList<object> ArgsGet(IReadOnlyList<object> arg)
        {
            return Invoke(arg, argsBinding.ArgsGet);
        }

        private IReadOnlyList<object> ArgsSizesGet(IReadOnlyList<object> arg)
        {
            return Invoke(arg, argsBinding.ArgsSizesGet);
        }

        private IReadOnlyList<object> EnvironGet(IReadOnlyList<object> arg)
        {
            return Invoke(arg, argsBinding.EnvironGet);
        }

        private IReadOnlyList<object> EnvironSizesGet(IReadOnlyList<object> arg)
        {
            return Invoke(arg, argsBinding.EnvironSizesGet);
        }
    }
}
