using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wasm.Interpret;

namespace Spirare
{
    public class DebugBinding : BindingBase
    {
        public DebugBinding(Element element, ContentsStore store) : base(element, store)
        {
        }

        public override PredefinedImporter GenerateImporter()
        {
            var importer = new PredefinedImporter();

            importer.DefineFunction("debug_log_info",
                 new DelegateFunctionDefinition(
                     ValueType.String,
                     ValueType.Unit,
                     LogInfo
                     ));
            return importer;
        }

        private IReadOnlyList<object> LogInfo(IReadOnlyList<object> arg)
        {
            var parser = new ArgumentParser(arg, ModuleInstance);
            if (!parser.TryReadString(out var message))
            {
                return ReturnValue.Unit;
            }
            Debug.Log(message);
            return ReturnValue.Unit;
        }
    }
}
