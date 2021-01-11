using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Spirare
{
    public class LoaderForEditor : PomlLoader
    {
        protected override WasmBehaviour AttachScript(PomlScriptElement element, Transform parent)
        {
            var wasm = base.AttachScript(element, parent);

            // event trigger
            var eventTrigger = gameObject.AddComponent<EventTrigger>();
            var entry = new EventTrigger.Entry()
            {
                eventID = EventTriggerType.PointerDown,
            };
            entry.callback.AddListener(_ => wasm.InvokeOnUse());
            eventTrigger.triggers.Add(entry);

            return wasm;
        }
    }
}
