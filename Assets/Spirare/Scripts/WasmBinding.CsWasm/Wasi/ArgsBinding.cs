using System;
using System.Collections;
using System.Collections.Generic;
using Wasm.Interpret;

namespace Spirare.WasmBinding.CsWasm
{
    public class ArgsBinding : BindingBase
    {
        private readonly WasmBinding.ArgsBinding argsBinding;

        private readonly List<string> args;
        private readonly List<string> envs;

        private int Argc => args.Count;

        private LinearMemoryAsInt8 Memory8 => ModuleInstance.Memories[0].Int8;
        private LinearMemoryAsInt32 Memory32 => ModuleInstance.Memories[0].Int32;

        public ArgsBinding(Element element, ContentsStore store, List<string> args, List<string> envs) : base(element, store)
        {
            this.args = args;
            this.envs = envs;
            argsBinding = new WasmBinding.ArgsBinding(args, envs);
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
            importer.DefineFunction("environ_get",
                 new DelegateFunctionDefinition(
                     ValueType.PointerAndPointer,
                     ValueType.Short,
                     EnvironGet
                     ));
            importer.DefineFunction("environ_sizes_get",
                 new DelegateFunctionDefinition(
                     ValueType.PointerAndPointer,
                     ValueType.Short,
                     EnvironSizesGet
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
            return Invoke(arg, argsBinding.ArgsGet);
        }

        private IReadOnlyList<object> ArgsSizesGet(IReadOnlyList<object> arg)
        {
            return Invoke(arg, argsBinding.ArgsSizesGet);
        }

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
                memory32[argvOffset] = BindingUtility.InterpretAsInt(argvBufferOffset);
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
    }
}
