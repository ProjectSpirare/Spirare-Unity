using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spirare
{
    public class LoaderForEditor : PomlLoader
    {
        protected override WasmBehaviour AttachScript(PomlScriptElement element, Transform parent)
        {
            var wasm = base.AttachScript(element, parent);

            //var handler = gameObject.AddComponent<EventHandlerForNonVR>();
           // handler.OnUse += wasm.InvokeOnUse;

            return wasm;
        }
    }
}
