using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using Wasm;
using Wasm.Interpret;

namespace Spirare
{
    public class FileDescriptorBinding : BindingBase
    {
        private List<Socket> sockets = new List<Socket>();

        public FileDescriptorBinding(Element element, ContentsStore store) : base(element, store)
        {
        }

        public override PredefinedImporter GenerateImporter()
        {
            var importer = new PredefinedImporter();


            importer.DefineFunction("fd_close",
                 new DelegateFunctionDefinition(
                     new WasmValueType[] { WasmValueType.Int32, },
                     new WasmValueType[] { WasmValueType.Int32, },
                     Close
                     ));
            importer.DefineFunction("fd_read",
                 new DelegateFunctionDefinition(
                     new WasmValueType[] { WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32 },
                     new WasmValueType[] { WasmValueType.Int32 },
                     Read
                     ));
            importer.DefineFunction("fd_write",
                 new DelegateFunctionDefinition(
                     new WasmValueType[] { WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32 }, new WasmValueType[] { WasmValueType.Int32 },
                     Write
                     ));

            return importer;
        }


        private IReadOnlyList<object> Invalid
        {
            get => ReturnValue.FromObject(-1);
        }
        private IReadOnlyList<object> ErrorResult
        {
            get => ReturnValue.FromObject(1);
        }

        private IReadOnlyList<object> Read(IReadOnlyList<object> arg)
        {
            throw new NotImplementedException();
        }

        private IReadOnlyList<object> Write(IReadOnlyList<object> arg)
        {
            var memory = ModuleInstance.Memories[0];
            var memory8 = memory.Int8;
            var memory32 = memory.Int32;

            var parser = new ArgumentParser(arg, ModuleInstance);
            if (!parser.TryReadInt(out int fd))
            {
                return ErrorResult;
            }
            if (!parser.TryReadVectoredBuffer(out byte[] buffer))
            {
                return ErrorResult;
            }
            if (!parser.TryReadUInt(out uint nwritten))
            {
                return ErrorResult;
            }

            try
            {
                var text = Encoding.UTF8.GetString(buffer);
                Debug.Log(text);
                memory32[nwritten] = buffer.Length;
                return ReturnValue.FromObject(0);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                return ErrorResult;
            }
        }

        private IReadOnlyList<object> Close(IReadOnlyList<object> arg)
        {
            Debug.Log("Close");
            return ReturnValue.FromObject(1);
        }
    }
}
