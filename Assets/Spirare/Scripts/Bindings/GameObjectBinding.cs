using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wasm.Interpret;

namespace UniWasm
{
    public class GameObjectBinding : BindingBase
    {
        public GameObjectBinding(Element element, ContentsStore store) : base(element, store)
        {
        }

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

        private IReadOnlyList<object> InvalidResourceIndex
        {
            get => ReturnValue.FromObject(-1);
        }
        private IReadOnlyList<object> InvalidElementIndex
        {
            get => ReturnValue.FromObject(-1);
        }

        private IReadOnlyList<object> GetResourceIndexById(IReadOnlyList<object> arg)
        {
            var parser = new ArgumentParser(arg, ModuleInstance);
            if (!parser.TryReadString(out var id))
            {
                return InvalidResourceIndex;
            }

            Debug.Log("GetResourceIndex");
            Debug.Log(id);

            if (!store.TryGetResourceIndexById(id, out var resourceIndex))
            {
                return InvalidResourceIndex;
            }

            Debug.Log(resourceIndex);
            return ReturnValue.FromObject(resourceIndex);
        }

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
            Debug.Log($"Spawn Object, ElementIndex: {elementIndex}");
            return ReturnValue.FromObject(elementIndex);
        }
    }
}
