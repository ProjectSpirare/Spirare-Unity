using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wasm.Interpret;

namespace Spirare
{
    public class PhysicsBinding : BindingBase
    {
        private Dictionary<int, Rigidbody> rigidbodyDictionary = new Dictionary<int, Rigidbody>();

        public PhysicsBinding(Element element, ContentsStore store) : base(element, store)
        {
        }

        public override PredefinedImporter GenerateImporter()
        {
            var importer = new PredefinedImporter();

            importer.DefineFunction("physics_set_world_velocity",
                new DelegateFunctionDefinition(
                ValueType.IdAndVector3,
                ValueType.Unit,
                SetWorldVelocity
            ));
            return importer;
        }

        private IReadOnlyList<object> SetWorldVelocity(IReadOnlyList<object> arg)
        {
            var parser = new ArgumentParser(arg);
            if (!TryGetElementWithArg(parser, store, out var element))
            {
                return ReturnValue.Unit;
            }

            if (!parser.TryReadVector3(out var velocity))
            {
                return ReturnValue.Unit;
            }

            if (!TryGetRigidbody(element, out var rigidbody))
            {
                return SpirareUtils.Unit;
            }

            rigidbody.velocity = velocity;

            return SpirareUtils.Unit;
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
