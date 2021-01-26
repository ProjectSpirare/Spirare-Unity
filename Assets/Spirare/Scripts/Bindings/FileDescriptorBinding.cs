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
            if (!parser.TryReadVectoredBuffer(out var buffer))
            {
                return ErrorResult;
            }
            /*
            if (!parser.TryReadUInt(out uint iovs))
            {
                return ErrorResult;
            }
            if (!parser.TryReadUInt(out uint iovsLen))
            {
                return ErrorResult;
            }
            */
            if (!parser.TryReadUInt(out uint nwritten))
            {
                return ErrorResult;
            }

            /*
            var totalSize = 0;
            var ptr = iovs;
            //Debug.Log(iovs);
            //Debug.Log($"iovs: {iovs}, iovsLen: {iovsLen}");
            for (uint i = 0; i < iovsLen; i++)
            {
                var start = memory32[iovs + i * 8];
                var size = memory32[iovs + i * 8 + 4];
                //Debug.Log($"start: {start}, size: {size}");

                //size = 200;
                var data = new byte[size];
                string value;
                try
                {
                    for (var j = 0; j < size; j++)
                    {
                        var index = (uint)(start + j);
                        data[j] = (byte)memory.Int8[index];
                    }
                    value = Encoding.UTF8.GetString(data);
                    Debug.Log(value);
                    totalSize += size;
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                    value = "";
                }
            }
            */

            try
            {
                var text = Encoding.UTF8.GetString(buffer);
                Debug.Log(text);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }

            memory32[nwritten] = buffer.Length;
            //Debug.Log($"fd: {fd}, iovsLen: {iovsLen}");
            return ReturnValue.FromObject(0);
        }

        private IReadOnlyList<object> Close(IReadOnlyList<object> arg)
        {
            Debug.Log("Close");
            return ReturnValue.FromObject(1);
        }
    }
}
