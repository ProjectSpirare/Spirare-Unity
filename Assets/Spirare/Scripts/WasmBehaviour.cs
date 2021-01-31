using Spirare.WasmBinding.CsWasm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
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
        private enum WasmEventType
        {
            Start = 0,
            Update,
            OnEquip,
            OnUnequip,
            OnUse,
            OnSelect
        }

        private ModuleInstance module;
        private FunctionDefinition startFunction;
        private FunctionDefinition updateFunction;
        private FunctionDefinition onEquipFunction;
        private FunctionDefinition onUnequipFunction;
        private FunctionDefinition onUseFunction;
        private FunctionDefinition onSelectFunction;

        private Queue<WasmEventType> eventQueue = new Queue<WasmEventType>();

        private SynchronizationContext context;
        private Thread mainThread;

        private bool updateCalled = false;

        protected virtual void Awake()
        {
            context = SynchronizationContext.Current;
            mainThread = Thread.CurrentThread;
        }

        protected virtual void Start()
        {
            eventQueue.Enqueue(WasmEventType.Start);
            _ = EventLoop();
        }

        protected virtual void Update()
        {
            updateCalled = true;
        }

        private async Task EventLoop()
        {
            while (Application.isPlaying)
            {
                await Task.Run(async () =>
                {
                    while (eventQueue.Count > 0)
                    {
                        var wasmEvent = eventQueue.Dequeue();
                        InvokeEvent(wasmEvent);
                    }

                    if (updateCalled)
                    {
                        updateCalled = false;
                        InvokeEvent(WasmEventType.Update);
                    }

                    await Task.Delay(10);
                });
            }
        }

        public void InvokeEvent(Spirare.WasmEventType eventType)
        {
            switch (eventType)
            {
                case Spirare.WasmEventType.Select:
                    InvokeOnSelect();
                    break;
                case Spirare.WasmEventType.Equip:
                    InvokeOnEquip();
                    break;
                case Spirare.WasmEventType.Unequip:
                    InvokeOnUnequip();
                    break;
                case Spirare.WasmEventType.Use:
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
                "fd_prestat_get",
                "fd_prestat_dir_name",
                // "random_get",
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

            var debugBinding = new DebugBinding(element, store);
            importer.IncludeDefinitions(debugBinding.Importer);

            var gameObjectBinding = new GameObjectBinding(element, store, context, mainThread);
            importer.IncludeDefinitions(gameObjectBinding.Importer);

            var transformBinding = new TransformBinding(element, store, context, mainThread);
            importer.IncludeDefinitions(transformBinding.Importer);

            var physicsBinding = new PhysicsBinding(element, store, context, mainThread);
            importer.IncludeDefinitions(physicsBinding.Importer);

            var timeBinding = new TimeBinding(element, store);
            importer.IncludeDefinitions(timeBinding.Importer);

            try
            {
                module = ModuleInstance.Instantiate(file, importer);

                argsBinding.ModuleInstance = module;
                fileDescriptorBinding.ModuleInstance = module;

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
            eventQueue.Enqueue(WasmEventType.OnSelect);
        }

        private void InvokeOnUse()
        {
            eventQueue.Enqueue(WasmEventType.OnUse);
        }

        private void InvokeOnEquip()
        {
            eventQueue.Enqueue(WasmEventType.OnEquip);
        }

        private void InvokeOnUnequip()
        {
            eventQueue.Enqueue(WasmEventType.OnUnequip);
        }

        private void InvokeEvent(WasmEventType eventType)
        {
            switch (eventType)
            {
                case WasmEventType.Start:
                    startFunction?.Invoke(ReturnValue.Unit);
                    break;
                case WasmEventType.Update:
                    updateFunction?.Invoke(ReturnValue.Unit);
                    break;
                case WasmEventType.OnSelect:
                    onSelectFunction?.Invoke(ReturnValue.Unit);
                    break;
                case WasmEventType.OnEquip:
                    onEquipFunction?.Invoke(ReturnValue.Unit);
                    break;
                case WasmEventType.OnUnequip:
                    onUnequipFunction?.Invoke(ReturnValue.Unit);
                    break;
                case WasmEventType.OnUse:
                    onUseFunction?.Invoke(ReturnValue.Unit);
                    break;
            }
        }
    }
}

