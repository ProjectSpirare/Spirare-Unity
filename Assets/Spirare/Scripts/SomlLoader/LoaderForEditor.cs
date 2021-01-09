using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniWasm
{
    public class LoaderForEditor : PomlLoader
    {
        protected override WasmBehaviour AttatchScript(string path, XmlNode node, Transform parent)
        {
            var wasm = base.AttatchScript(path, node, parent);
            if (wasm != null)
            {
                // event trigger
                var eventTrigger = gameObject.AddComponent<EventTrigger>();
                var entry = new EventTrigger.Entry()
                {
                    eventID = EventTriggerType.PointerDown,
                };
                entry.callback.AddListener(_ => wasm.InvokeOnUse());
                eventTrigger.triggers.Add(entry);
            }
            return wasm;
        }
    }
}
