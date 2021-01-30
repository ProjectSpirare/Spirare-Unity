using Spirare.WasmBinding.CsWasm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Wasm;
using Wasm.Interpret;

namespace Spirare
{
    public enum WasmEventType
    {
        Select = 0,
        Equip,
        Unequip,
        Use,
    }

    public class WasmBehaviour : MonoBehaviour
    {
        private ModuleInstance module;
        private FunctionDefinition startFunction;
        private FunctionDefinition updateFunction;
        private FunctionDefinition onEquipFunction;
        private FunctionDefinition onUnequipFunction;
        private FunctionDefinition onUseFunction;
        private FunctionDefinition onSelectFunction;

        protected virtual void Start()
        {
            startFunction?.Invoke(ReturnValue.Unit);
        }

        protected virtual void Update()
        {
            updateFunction?.Invoke(ReturnValue.Unit);
        }

        public void InvokeEvent(WasmEventType eventType)
        {
            switch (eventType)
            {
                case WasmEventType.Select:
                    InvokeOnSelect();
                    break;
                case WasmEventType.Equip:
                    InvokeOnEquip();
                    break;
                case WasmEventType.Unequip:
                    InvokeOnUnequip();
                    break;
                case WasmEventType.Use:
                    InvokeOnUse();
                    break;
                default:
                    break;
            }
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
                //"proc_exit",
                //"fd_read",
                //"fd_write",
                //"fd_close",
                "fd_prestat_get",
                "fd_prestat_dir_name",
                //"environ_sizes_get",
                //"environ_get",
                // "random_get",
                //"env.abort",
                "abort",
            };

            importer.DefineFunction("proc_exit",
                 new DelegateFunctionDefinition(
                     new WasmValueType[] { WasmValueType.Int32 },
                     new WasmValueType[] { },
                     x => new object[0]
                     ));
            importer.DefineFunction("clock_time_get",
                 new DelegateFunctionDefinition(
                     new WasmValueType[] { WasmValueType.Int32, WasmValueType.Int64, WasmValueType.Int32 },
                     new WasmValueType[] { WasmValueType.Int32 },
                     GetTime
                     ));
            foreach (var wasiFunction in wasiFunctions)
            {
                importer.DefineFunction(wasiFunction,
                     new DelegateFunctionDefinition(
                         new WasmValueType[] { },
                         new WasmValueType[] { },
                         x => ReturnValue.FromObject(0)
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

            var argsBinding = new ArgsBinding(element, store, args, null);
            importer.IncludeDefinitions(argsBinding.Importer);

            var fileDescriptorBinding = new FileDescriptorBinding(element, store);
            importer.IncludeDefinitions(fileDescriptorBinding.Importer);

            var socketBinding = new SocketBinding(element, store);
            importer.IncludeDefinitions(socketBinding.Importer);

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

            try
            {
                module = ModuleInstance.Instantiate(file, importer);

                argsBinding.ModuleInstance = module;
                fileDescriptorBinding.ModuleInstance = module;
                socketBinding.ModuleInstance = module;

                gameObjectBinding.ModuleInstance = module;
                debugBinding.ModuleInstance = module;

                var exportedFunctions = module.ExportedFunctions;
                exportedFunctions.TryGetValue("start", out startFunction);
                exportedFunctions.TryGetValue("update", out updateFunction);
                exportedFunctions.TryGetValue("on_use", out onUseFunction);
                exportedFunctions.TryGetValue("on_select", out onSelectFunction);
                exportedFunctions.TryGetValue("on_equip", out onEquipFunction);
                exportedFunctions.TryGetValue("on_unequip", out onEquipFunction);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private IReadOnlyList<object> GetTime(IReadOnlyList<object> arg)
        {
            Debug.Log("clock");
            return ReturnValue.FromObject(0);
        }

        private void InvokeOnSelect()
        {
            onSelectFunction?.Invoke(ReturnValue.Unit);
        }

        private void InvokeOnUse()
        {
            onUseFunction?.Invoke(ReturnValue.Unit);
        }

        private void InvokeOnEquip()
        {
            onEquipFunction?.Invoke(ReturnValue.Unit);
        }

        private void InvokeOnUnequip()
        {
            onUnequipFunction?.Invoke(ReturnValue.Unit);
        }


    }
}
