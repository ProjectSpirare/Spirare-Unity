﻿using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Wasm.Interpret;

namespace Spirare.WasmBinding.CsWasm
{
    public class PhysicsBinding : BindingBase
    {
        // private Dictionary<int, Rigidbody> rigidbodyDictionary = new Dictionary<int, Rigidbody>();
        private readonly WasmBinding.PhysicsBinding physicsBinding;

        public PhysicsBinding(Element element, ContentsStore store, SynchronizationContext context, Thread mainThread)
            : base(element, store, context, mainThread)
        {
            physicsBinding = new WasmBinding.PhysicsBinding(element, store, context, mainThread);
        }

        public override PredefinedImporter GenerateImporter()
        {
            var importer = new PredefinedImporter();

            importer.DefineFunction("physics_set_world_velocity",
                new DelegateFunctionDefinition(
                ValueType.IdAndVector3,
                ValueType.Unit,
                arg => Invoke(arg, SetWorldVelocity)
                // SetWorldVelocity
            ));
            return importer;
        }

        private void SetWorldVelocity(ArgumentParser parser)
        {
            physicsBinding.SetWorldVelocity(parser);
        }

        /*
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
            // rigidbody = null;

            return true;
        }
        */
    }
}
