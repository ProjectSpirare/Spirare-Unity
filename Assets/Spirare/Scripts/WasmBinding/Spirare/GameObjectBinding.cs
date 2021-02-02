using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Spirare.WasmBinding
{
    internal class GameObjectBinding : SpirareBindingBase
    {
        public GameObjectBinding(Element element, ContentsStore store, SynchronizationContext context, Thread mainThread)
            : base(element, store, context, mainThread)
        {
        }

        private int InvalidResourceIndex
        {
            get => -1;
        }
        private int InvalidElementIndex
        {
            get => -1;
        }
        internal object GetElementIndexById(ArgumentParser parser)
        {
            if (!parser.TryReadString(out var id))
            {
                return InvalidElementIndex;
            }

            if (!store.TryGetElementIndexById(id, out var elementIndex))
            {
                return InvalidElementIndex;
            }

            return elementIndex;
        }

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

            var gameObject = RunOnUnityThread(() =>
            {
                var go = Object.Instantiate(resource.GameObject);
                go.transform.SetParent(root, false);
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
