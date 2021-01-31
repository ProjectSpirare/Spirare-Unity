using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Wasm.Interpret;

namespace Spirare.WasmBinding
{
    internal class GameObjectBinding : SpirareBindingBase
    {
        /*
        public GameObjectBinding(Element element, ContentsStore store) : base(element, store)
        {
        }
        */
        public GameObjectBinding(Element element, ContentsStore store, SynchronizationContext context, Thread mainThread)
            : base(element, store, context, mainThread)
        {
        }


        /*
        public override PredefinedImporter GenerateImporter()
        {
            var importer = new PredefinedImporter();
            importer.DefineFunction("element_spawn_object",
                 new DelegateFunctionDefinition(
                     ValueType.Int,
                     ValueType.Int,
                     SpawnObject
                     ));

            importer.DefineFunction("element_get_resource_index_by_id",
                 new DelegateFunctionDefinition(
                     ValueType.String,
                     ValueType.Int,
                     GetResourceIndexById
                     ));
            return importer;
        }
        */

        private int InvalidResourceIndex
        {
            get => -1;
        }
        private int InvalidElementIndex
        {
            get => -1;
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

        internal object GetResourceIndexById(ArgumentParser parser)
        {
            if (!parser.TryReadString(out var id))
            {
                return InvalidResourceIndex;
            }

            if (!store.TryGetResourceIndexById(id, out var resourceIndex))
            {
                return InvalidResourceIndex;
            }

            return resourceIndex;
        }


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

        public int SpawnObject(ArgumentParser parser)
        {
            if (!parser.TryReadInt(out var resourceIndex))
            {
                return InvalidElementIndex;
            }

            if (!store.TryGetResourceByResourceIndex(resourceIndex, out var resource))
            {
                return InvalidElementIndex;
            }

            var root = store.RootTransform;

            // GameObject go;
            var gameObject = RunOnUnityThread(() =>
            {
                var go = Object.Instantiate(resource.GameObject);
                go.transform.SetParent(root, false);
                // return go;
                return go;
            });


            var element = new Element()
            {
                GameObject = gameObject
            };
            var elementIndex = store.RegisterElement(element);
            return elementIndex;
        }
    }
}
