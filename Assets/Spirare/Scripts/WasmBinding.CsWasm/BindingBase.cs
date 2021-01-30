using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wasm.Interpret;

namespace Spirare
{
    public abstract class BindingBase
    {
        public ModuleInstance ModuleInstance { set; get; }
        public PredefinedImporter Importer { private set; get; }

        private MemoryReader memoryReader;
        protected MemoryReader MemoryReader
        {
            get
            {
                if (memoryReader == null)
                {
                    memoryReader = new MemoryReader(ModuleInstance.Memories[0]);
                }
                return memoryReader;
            }
        }

        protected Element thisElement;
        protected ContentsStore store;

        public BindingBase(Element element, ContentsStore store)
        {
            Importer = GenerateImporter();
            this.thisElement = element;
            this.store = store;
        }

        public abstract PredefinedImporter GenerateImporter();

        protected bool TryGetElementWithArg(ArgumentParser parser, ContentsStore store, out Element element)
        {
            if (!parser.TryReadInt(out var elementIndex))
            {
                element = null;
                return false;
            }

            if (elementIndex == 0)
            {
                element = thisElement;
                return true;
            }

            if (!store.TryGetElementByElementIndex(elementIndex, out element))
            {
                return false;
            }

            return true;
        }
    }
}
