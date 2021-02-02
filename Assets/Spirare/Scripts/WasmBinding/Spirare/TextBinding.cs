using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Spirare.WasmBinding
{
    internal class TextBinding : SpirareBindingBase
    {
        private static readonly int Success = 0;
        private static readonly int Error = 1;

        public TextBinding(Element element, ContentsStore store, SynchronizationContext context, Thread mainThread)
            : base(element, store, context, mainThread)
        {
        }

        public int SetText(ArgumentParser parser)
        {
            if (!TryGetElementWithArg(parser, out var element))
            {
                return Error;
            }

            if (!parser.TryReadString(out var text))
            {
                return Error;
            }

            RunOnUnityThread(() =>
            {
                element.GameObject.SendMessage("SetText", text);
            });

            return Success;
        }
    }
}
