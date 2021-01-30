using System;
using System.Collections;
using System.Collections.Generic;

namespace Spirare.WasmBinding
{
    internal class SpirareBindingBase
    {
        protected Element thisElement;
        protected ContentsStore store;

        public SpirareBindingBase(Element element, ContentsStore store)
        {
            thisElement = element;
            this.store = store;
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
    }
}