using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Spirare.WasmBinding
{
    internal class PhysicsBinding : SpirareBindingBase
    {
        private Dictionary<int, Rigidbody> rigidbodyDictionary = new Dictionary<int, Rigidbody>();

        public PhysicsBinding(Element element, ContentsStore store, SynchronizationContext context, Thread mainThread)
            : base(element, store, context, mainThread)
        {
        }

        public void SetWorldVelocity(ArgumentParser parser)
        {
            if (!TryGetElementWithArg(parser, out var element))
            {
                return;
            }

            if (!parser.TryReadVector3(out var velocity))
            {
                return;
            }
            RunOnUnityThread(() =>
            {
                if (!TryGetRigidbody(element, out var rigidbody))
                {
                    return;
                }

                rigidbody.velocity = velocity;
            });
        }

        private bool TryGetRigidbody(Element element, out Rigidbody rigidbody)
        {
            var elementIndex = element.ElementIndex;
            if (!rigidbodyDictionary.TryGetValue(elementIndex, out rigidbody))
            {
                var gameObject = element.GameObject;
                rigidbody = gameObject.GetComponent<Rigidbody>();
                if (rigidbody == null)
                {
                    rigidbody = gameObject.AddComponent<Rigidbody>();
                    rigidbody.useGravity = false;
                }

                rigidbodyDictionary[elementIndex] = rigidbody;
            }

            return true;
        }
    }
}
