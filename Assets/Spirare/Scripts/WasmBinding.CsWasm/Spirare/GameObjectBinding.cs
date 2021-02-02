using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Wasm.Interpret;

namespace Spirare.WasmBinding.CsWasm
{
    public class GameObjectBinding : BindingBase
    {
        private readonly WasmBinding.GameObjectBinding gameObjectBinding;

        public GameObjectBinding(Element element, ContentsStore store, SynchronizationContext context, Thread mainThread)
            : base(element, store, context, mainThread)
        {
            gameObjectBinding = new WasmBinding.GameObjectBinding(element, store, context, mainThread);
        }

        public override PredefinedImporter GenerateImporter()
        {
            var importer = new PredefinedImporter();
            importer.DefineFunction("element_spawn_object",
                 new DelegateFunctionDefinition(
                     ValueType.Int,
                     ValueType.Int,
                     arg => Invoke(arg, SpawnObject)
                     ));

            importer.DefineFunction("element_get_element_index_by_id",
                 new DelegateFunctionDefinition(
                     ValueType.String,
                     ValueType.Int,
                     arg => Invoke(arg, GetElementIndexById)
                     ));

            importer.DefineFunction("element_get_resource_index_by_id",
                 new DelegateFunctionDefinition(
                     ValueType.String,
                     ValueType.Int,
                     arg => Invoke(arg, GetResourceIndexById)
                     ));
            return importer;
        }

        private object SpawnObject(ArgumentParser parser)
        {
            return gameObjectBinding.SpawnObject(parser);
        }

        private object GetElementIndexById(ArgumentParser parser)
        {
            return gameObjectBinding.GetElementIndexById(parser);
        }

        private object GetResourceIndexById(ArgumentParser parser)
        {
            return gameObjectBinding.GetResourceIndexById(parser);
        }
    }
}
