using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wasm.Interpret;

namespace Spirare.WasmBinding.CsWasm
{
    public class TimeBinding : BindingBase
    {
        public TimeBinding(Element element, ContentsStore store) : base(element, store)
        {
        }

        public override PredefinedImporter GenerateImporter()
        {
            var importer = new PredefinedImporter();

            importer.DefineFunction("time_get_time",
                 new DelegateFunctionDefinition(
                     ValueType.Unit,
                     ValueType.Float,
                     GetTime
                     ));

            importer.DefineFunction("time_get_delta_time",
                 new DelegateFunctionDefinition(
                     ValueType.Unit,
                     ValueType.Float,
                     GetDeltaTime
                     ));
            return importer;
        }

        private IReadOnlyList<object> GetTime(IReadOnlyList<object> args)
        {
            return new object[]
            {
                Time.time
            };
        }
        private IReadOnlyList<object> GetDeltaTime(IReadOnlyList<object> args)
        {
            return new object[]
            {
                Time.deltaTime
            };
        }

    }
}
