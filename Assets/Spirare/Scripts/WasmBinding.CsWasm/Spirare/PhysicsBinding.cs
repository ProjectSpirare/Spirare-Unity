using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Wasm.Interpret;

namespace Spirare.WasmBinding.CsWasm
{
    public class PhysicsBinding : BindingBase
    {
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
            ));
            return importer;
        }

        private void SetWorldVelocity(ArgumentParser parser)
        {
            physicsBinding.SetWorldVelocity(parser);
        }
    }
}
