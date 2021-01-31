using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
        protected SynchronizationContext context;
        protected Thread mainThread;

        public BindingBase(Element element, ContentsStore store)
        {
            Importer = GenerateImporter();
            this.thisElement = element;
            this.store = store;
        }

        public BindingBase(Element element, ContentsStore store, SynchronizationContext context, Thread mainThread) : this(element, store)
        {
            this.context = context;
            this.mainThread = mainThread;
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

        protected IReadOnlyList<object> Invoke(IReadOnlyList<object> arg, Action<ArgumentParser> action)
        {
            var parser = new ArgumentParser(arg, ModuleInstance);
            action.Invoke(parser);
            return ReturnValue.Unit;
        }

        protected IReadOnlyList<object> Invoke(IReadOnlyList<object> arg, Func<ArgumentParser, object> func)
        {
            var parser = new ArgumentParser(arg, ModuleInstance);
            var value = func.Invoke(parser);
            return ReturnValue.FromObject(value);
        }
    }
}
