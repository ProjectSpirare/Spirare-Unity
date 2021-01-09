using System;
using System.Collections;
using System.Collections.Generic;
using Wasm.Interpret;

namespace Spirare
{
    public class ArgsBinding : BindingBase
    {
        private readonly List<string> args;

        public ArgsBinding(Element element, ContentsStore store, List<string> args) : base(element, store)
        {
            this.args = args;
        }

        public override PredefinedImporter GenerateImporter()
        {
            var importer = new PredefinedImporter();

            importer.DefineFunction("args_get",
                 new DelegateFunctionDefinition(
                     ValueType.PointerAndPointer,
                     ValueType.Short,
                     ArgsGet
                     ));
            importer.DefineFunction("args_sizes_get",
                 new DelegateFunctionDefinition(
                     ValueType.PointerAndPointer,
                     ValueType.Short,
                     ArgsSizesGet
                     ));
            return importer;
        }

        private IReadOnlyList<object> ErrorResult
        {
            get => ReturnValue.FromObject(0);
        }

        private IReadOnlyList<object> SuccessResult
        {
            get => ReturnValue.FromObject(0);
        }

        private IReadOnlyList<object> ArgsGet(IReadOnlyList<object> arg)
        {
            var memory = ModuleInstance.Memories[0];
            var memory8 = memory.Int8;
            var memory32 = memory.Int32;

            var parser = new ArgumentParser(arg, ModuleInstance);
            if (!parser.TryReadUInt(out uint argvOffset))
            {
                return ErrorResult;
            }
            if (!parser.TryReadUInt(out uint argvBufferOffset))
            {
                return ErrorResult;
            }

            foreach (var argString in args)
            {
                memory32[argvOffset] = ArgumentParser.InterpretAsInt(argvBufferOffset);
                argvOffset += 4;

                argvBufferOffset = WriteStringToMemory(memory8, argString, argvBufferOffset);
            }

            return SuccessResult;
        }

        private IReadOnlyList<object> ArgsSizesGet(IReadOnlyList<object> arg)
        {
            var memory = ModuleInstance.Memories[0];
            var memory8 = memory.Int8;
            var memory32 = memory.Int32;

            var parser = new ArgumentParser(arg, ModuleInstance);
            if (!parser.TryReadUInt(out uint argvOffset))
            {
                return ErrorResult;
            }
            if (!parser.TryReadUInt(out uint argvBufferOffset))
            {
                return ErrorResult;
            }

            var argc = args.Count;
            var dataSize = 0;
            foreach (var argString in args)
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(argString);
                dataSize += bytes.Length + 1;
            }

            memory32[argvOffset] = argc;
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
    }
}
