using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Wasm;
using Wasm.Interpret;

namespace Spirare
{
    public class WasmBehaviour : MonoBehaviour
    {
        private ModuleInstance module;
        private FunctionDefinition startFunction;
        private FunctionDefinition updateFunction;
        private FunctionDefinition onTouchStartFunction;
        private FunctionDefinition onUseFunction;

        protected virtual void Start()
        {
            startFunction?.Invoke(ReturnValue.Unit);
        }

        protected virtual void Update()
        {
            updateFunction?.Invoke(ReturnValue.Unit);
        }

        public void InvokeOnUse()
        {
            onUseFunction?.Invoke(new object[0]);
        }

        public void LoadWasm(string path, ContentsStore store = null, List<string> args = null)
        {
            var file = WasmFile.ReadBinary(path);
            LoadWasm(file, store, args);
        }

        public void LoadWasm(Stream stream, ContentsStore store = null, List<string> args = null)
        {
            var file = WasmFile.ReadBinary(stream);
            LoadWasm(file, store, args);
        }

        protected void LoadWasm(WasmFile file, ContentsStore store = null, List<string> args = null)
        {
            if (store == null)
            {
                store = new ContentsStore();
            }

            var importer = new PredefinedImporter();

            var wasiFunctions = new List<string>()
            {
                "proc_exit",
                "fd_read",
                "fd_write",
                "fd_prestat_get",
                "fd_prestat_dir_name",
                "environ_sizes_get",
                "environ_get",
                // "random_get",
                //"env.abort",
                "abort",
            };

            foreach (var wasiFunction in wasiFunctions)
            {
                importer.DefineFunction(wasiFunction,
                     new DelegateFunctionDefinition(
                         new WasmValueType[] { },
                         new WasmValueType[] { },
                         x => x
                         ));
            }
            importer.DefineFunction("random_get",
                 new DelegateFunctionDefinition(
                     ValueType.PointerAndPointer,
                     ValueType.Int,
                     x => ReturnValue.FromObject(0)
                     ));

            var element = new Element()
            {
                GameObject = gameObject
            };

            if (args == null)
            {
                args = new List<string>();
            }
            var scriptName = "";
            args.Insert(0, scriptName);

            var argsBinding = new ArgsBinding(element, store, args);
            importer.IncludeDefinitions(argsBinding.Importer);

            var debugBinding = new DebugBinding(element, store);
            importer.IncludeDefinitions(debugBinding.Importer);

            var gameObjectBinding = new GameObjectBinding(element, store);
            importer.IncludeDefinitions(gameObjectBinding.Importer);

            var transformBinding = new TransformBinding(element, store);
            importer.IncludeDefinitions(transformBinding.Importer);

            var physicsBinding = new PhysicsBinding(element, store);
            importer.IncludeDefinitions(physicsBinding.Importer);

            var timeBinding = new TimeBinding(element, store);
            importer.IncludeDefinitions(timeBinding.Importer);

            module = ModuleInstance.Instantiate(file, importer);

            argsBinding.ModuleInstance = module;
            gameObjectBinding.ModuleInstance = module;
            debugBinding.ModuleInstance = module;

            var exportedFunctions = module.ExportedFunctions;
            exportedFunctions.TryGetValue("start", out startFunction);
            exportedFunctions.TryGetValue("update", out updateFunction);
            exportedFunctions.TryGetValue("on_touch_start", out onTouchStartFunction);
            exportedFunctions.TryGetValue("on_use", out onUseFunction);
        }
    }
}
