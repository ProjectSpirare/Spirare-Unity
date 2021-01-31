﻿using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Wasm.Interpret;

namespace Spirare.WasmBinding.CsWasm
{
    public class GameObjectBinding : BindingBase
    {
        /*
        public GameObjectBinding(Element element, ContentsStore store) : base(element, store)
        {
        }
        */
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
                     // SpawnObject
                     ));

            importer.DefineFunction("element_get_resource_index_by_id",
                 new DelegateFunctionDefinition(
                     ValueType.String,
                     ValueType.Int,
                     arg => Invoke(arg, GetResourceIndexById)
                     // GetResourceIndexById
                     ));
            return importer;
        }

        private object SpawnObject(ArgumentParser parser)
        {
            return gameObjectBinding.SpawnObject(parser);
            // throw new System.NotImplementedException();

            /*
            if (!parser.TryReadInt(out var resourceIndex))
            {
                return InvalidElementIndex;
            }

            if (!store.TryGetResourceByResourceIndex(resourceIndex, out var resource))
            {
                return InvalidElementIndex;
            }

            var root = store.RootTransform;
            var go = Object.Instantiate(resource.GameObject);
            go.transform.SetParent(root, false);

            var element = new Element()
            {
                GameObject = go
            };
            var elementIndex = store.RegisterElement(element);
            */

        }

        private IReadOnlyList<object> InvalidResourceIndex
        {
            get => ReturnValue.FromObject(-1);
        }
        private IReadOnlyList<object> InvalidElementIndex
        {
            get => ReturnValue.FromObject(-1);
        }
        private object GetResourceIndexById(ArgumentParser parser)
        {
            return gameObjectBinding.GetResourceIndexById(parser);
        }

        /*

        private IReadOnlyList<object> GetResourceIndexById(IReadOnlyList<object> arg)
        {
            var parser = new ArgumentParser(arg, ModuleInstance);
            if (!parser.TryReadString(out var id))
            {
                return InvalidResourceIndex;
            }

            if (!store.TryGetResourceIndexById(id, out var resourceIndex))
            {
                return InvalidResourceIndex;
            }

            return ReturnValue.FromObject(resourceIndex);
        }
        */

        /*
        private IReadOnlyList<object> SpawnObject(IReadOnlyList<object> arg)
        {
            var parser = new ArgumentParser(arg);
            if (!parser.TryReadInt(out var resourceIndex))
            {
                return InvalidElementIndex;
            }

            if (!store.TryGetResourceByResourceIndex(resourceIndex, out var resource))
            {
                return InvalidElementIndex;
            }

            var root = store.RootTransform;
            var go = Object.Instantiate(resource.GameObject);
            go.transform.SetParent(root, false);

            var element = new Element()
            {
                GameObject = go
            };
            var elementIndex = store.RegisterElement(element);
            go.name = $"{resource.Id}, {elementIndex}";
            return ReturnValue.FromObject(elementIndex);
        }
        */
    }
}
