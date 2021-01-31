using System;
using System.Collections;
using System.Collections.Generic;

namespace Spirare.WasmBinding
{
    internal class WasmBindingBase
    {

    }

    internal class ArgsBinding : WasmBindingBase
    {
        private readonly List<string> args;
        private readonly List<string> envs;

        private static readonly int Success = 0;
        private static readonly int Error = 1;

        public ArgsBinding(List<string> args, List<string> envs)
        {
            this.args = args;
            this.envs = envs;
        }

        public int ArgsGet(ArgumentParser parser, MemoryReader memoryReader)
        {
            return WriteStringList(parser, memoryReader, args);
        }

        public int ArgsSizesGet(ArgumentParser parser, MemoryReader memoryReader)
        {
            return WriteLength(parser, memoryReader, args);
        }

        public int EnvironGet(ArgumentParser parser, MemoryReader memoryReader)
        {
            return WriteStringList(parser, memoryReader, envs);
        }

        public int EnvironSizesGet(ArgumentParser parser, MemoryReader memoryReader)
        {
            return WriteLength(parser, memoryReader, envs);
        }

        private int WriteStringList(ArgumentParser parser, MemoryReader memoryReader, List<string> textList)
        {
            if (!parser.TryReadPointer(out uint offset, out uint bufferOffset))
            {
                return Error;
            }

            foreach (var text in textList)
            {
                if (!memoryReader.TryWrite(offset, BindingUtility.InterpretAsInt(bufferOffset)))
                {
                    return Error;
                }
                offset += 4;

                if (!memoryReader.TryWriteString(text, ref bufferOffset))
                {
                    return Error;
                }
            }

            return Success;
        }

        private int WriteLength(ArgumentParser parser, MemoryReader memoryReader, List<string> textList)
        {
            if (!parser.TryReadPointer(out uint offset, out uint bufferOffset))
            {
                return Error;
            }

            var dataSize = 0;
            foreach (var text in textList)
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(text);
                dataSize += bytes.Length + 1;
            }

            if (!memoryReader.TryWrite(offset, textList.Count) ||
                 !memoryReader.TryWrite(bufferOffset, dataSize))
            {
                return Error;
            }
            return Success;
        }
    }
}
