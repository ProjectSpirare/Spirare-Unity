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

        private int Argc => args.Count;

        private static readonly int Success = 0;
        private static readonly int Error = 1;

        public ArgsBinding(List<string> args, List<string> envs)
        {
            this.args = args;
            this.envs = envs;
        }

        public int ArgsGet(ArgumentParser parser, MemoryReader memoryReader)
        {
            if (!parser.TryReadPointer(out uint argvOffset, out uint argvBufferOffset))
            {
                return Error;
            }

            foreach (var argString in args)
            {
                if (!memoryReader.TryWrite(argvOffset, BindingUtility.InterpretAsInt(argvBufferOffset)))
                {
                    return Error;
                }
                argvOffset += 4;

                if (!memoryReader.TryWriteString(argString, ref argvBufferOffset))
                {
                    return Error;
                }
            }

            return Success;
        }

        public int ArgsSizesGet(ArgumentParser parser, MemoryReader memoryReader)
        {
            if (!parser.TryReadPointer(out uint argvOffset, out uint argvBufferOffset))
            {
                return Error;
            }

            var dataSize = 0;
            foreach (var argString in args)
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(argString);
                dataSize += bytes.Length + 1;
            }

            if (!memoryReader.TryWrite(argvOffset, Argc) ||
                 !memoryReader.TryWrite(argvBufferOffset, dataSize))
            {
                return Error;
            }
            return Success;
        }

        /*

        private IReadOnlyList<object> EnvironGet(IReadOnlyList<object> arg)
        {
            var memory32 = Memory32;
            var parser = new ArgumentParser(arg, ModuleInstance);
            if (!parser.TryReadPointer(out uint argvOffset, out uint argvBufferOffset))
            {
                return ErrorResult;
            }

            foreach (var env in envs)
            {
                memory32[argvOffset] = ArgumentParser.InterpretAsInt(argvBufferOffset);
                argvOffset += 4;

                argvBufferOffset = WriteStringToMemory(Memory8, env, argvBufferOffset);
            }

            return SuccessResult;
        }

        private IReadOnlyList<object> EnvironSizesGet(IReadOnlyList<object> arg)
        {
            var memory32 = Memory32;

            var parser = new ArgumentParser(arg, ModuleInstance);
            if (!parser.TryReadPointer(out uint argvOffset, out uint argvBufferOffset))
            {
                return ErrorResult;
            }

            var dataSize = 0;
            foreach (var env in envs)
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(env);
                dataSize += bytes.Length + 1;
            }

            memory32[argvOffset] = Argc;
            memory32[argvBufferOffset] = dataSize;

            return SuccessResult;
        }


        private uint WriteStringToMemory(LinearMemoryAsInt8 memory, string text, uint offset)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(text);
            offset = WriteBytesToMemory(memory, bytes, offset);
            var nullBytes = new byte[] { 0 };
            offset = WriteBytesToMemory(memory, nullBytes, offset);
            return offset;
        }
        private uint WriteBytesToMemory(LinearMemoryAsInt8 memory, byte[] data, uint offset)
        {
            foreach (var byteData in data)
            {
                memory[offset] = (sbyte)byteData;
                offset += 1;
            }
            return offset;
        }
        */
    }
}
