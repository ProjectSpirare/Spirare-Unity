using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Wasm.Interpret;

namespace Spirare.WasmBinding.CsWasm
{
    public class TextBinding : BindingBase
    {
        private readonly WasmBinding.TextBinding textBinding;

        public TextBinding(Element element, ContentsStore store, SynchronizationContext context, Thread mainThread)
            : base(element, store, context, mainThread)
        {
            textBinding = new WasmBinding.TextBinding(element, store, context, mainThread);
        }

        public override PredefinedImporter GenerateImporter()
        {
            var importer = new PredefinedImporter();

            importer.DefineFunction("text_set_text",
                 new DelegateFunctionDefinition(
                     ValueType.IdAndString,
                     ValueType.Unit,
                     SetText
                     ));
            return importer;
        }

        private IReadOnlyList<object> SetText(IReadOnlyList<object> arg)
        {
            return Invoke(arg, textBinding.SetText);
        }
    }
}
