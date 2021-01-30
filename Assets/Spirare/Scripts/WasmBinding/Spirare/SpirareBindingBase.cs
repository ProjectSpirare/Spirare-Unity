using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Spirare.WasmBinding
{
    internal class SpirareBindingBase
    {
        protected Element thisElement;
        protected ContentsStore store;

        private SynchronizationContext context = null;
        private Thread mainThread = null;

        public SpirareBindingBase(Element element, ContentsStore store, SynchronizationContext context, Thread mainThread)
        {
            thisElement = element;
            this.store = store;

            this.context = context;
            this.mainThread = mainThread;
        }

        protected bool TryGetElementWithArg(ArgumentParser parser, out Element element)
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

        protected T RunOnUnityThread<T>(Func<T> func)
        {
            var currentThread = Thread.CurrentThread;
            if (currentThread == mainThread)
            {
                return func.Invoke();
            }

            T result = default;

            context.Send(_ =>
            {
                result = func.Invoke();
            }, null);

            return result;
        }

        protected void RunOnUnityThread(Action action)
        {
            var currentThread = Thread.CurrentThread;
            if (currentThread == mainThread)
            {
                action.Invoke();
            }

            context.Post(_ =>
            {
                action.Invoke();
            }, null);
        }
    }
}