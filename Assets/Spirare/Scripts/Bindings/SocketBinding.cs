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
    public class SocketBinding : BindingBase
    {
        private List<Socket> sockets = new List<Socket>();

        public SocketBinding(Element element, ContentsStore store) : base(element, store)
        {
        }

        public override PredefinedImporter GenerateImporter()
        {
            var importer = new PredefinedImporter();

            /*
            importer.DefineFunction("proc_exit",
                             new DelegateFunctionDefinition(
                         new WasmValueType[] { WasmValueType.Int32, WasmValueType.Int32 },
                         new WasmValueType[] { WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32},
                         //        ValueType.String,
                         //        ValueType.String,
                                 Connect
                                 ));
            importer.DefineFunction("environ_sizes_get",
                             new DelegateFunctionDefinition(
                         new WasmValueType[] { WasmValueType.Int32, WasmValueType.Int32 },
                         new WasmValueType[] { WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32},
                         //        ValueType.String,
                         //        ValueType.String,
                                 Connect
                                 ));
            importer.DefineFunction("environ_get",
                             new DelegateFunctionDefinition(
                         new WasmValueType[] { WasmValueType.Int32, WasmValueType.Int32 },
                         new WasmValueType[] { WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32},
                         //        ValueType.String,
                         //        ValueType.String,
                                 Connect
                                 ));
            */


            importer.DefineFunction("fd_close",
                             new DelegateFunctionDefinition(
                         new WasmValueType[] { WasmValueType.Int32, },
                         new WasmValueType[] { WasmValueType.Int32, },
                                 //        ValueType.String,
                                 //        ValueType.String,
                                 Close
                                 ));
            importer.DefineFunction("fd_read",
                             new DelegateFunctionDefinition(
                         new WasmValueType[] { WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32 },
                         new WasmValueType[] { WasmValueType.Int32 },
                                 //        ValueType.String,
                                 //        ValueType.String,
                                 Write
                                 ));
            importer.DefineFunction("fd_write",
                             new DelegateFunctionDefinition(
                         new WasmValueType[] { WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32 },
                         new WasmValueType[] { WasmValueType.Int32 },
                                 //        ValueType.String,
                                 //        ValueType.String,
                                 Write
                                 ));

            importer.DefineFunction("sock_connect",
                             new DelegateFunctionDefinition(
                         new WasmValueType[] { WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32 },
                         new WasmValueType[] { WasmValueType.Int32, },
                                 //        ValueType.String,
                                 //        ValueType.String,
                                 Connect
                                 ));

            importer.DefineFunction("sock_send",
                             new DelegateFunctionDefinition(
                         new WasmValueType[] { WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32 },
                         new WasmValueType[] { WasmValueType.Int32, },
                                 //        ValueType.String,
                                 //        ValueType.String,
                                 Send
                                 ));

            importer.DefineFunction("sock_recv",
                             new DelegateFunctionDefinition(
                         new WasmValueType[] { WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32 },
                         new WasmValueType[] { WasmValueType.Int32, },
                                 //       ValueType.String,
                                 //./       ValueType.String,
                                 Receive
                                 ));
            /*
            importer.DefineFunction("element_spawn_object",
                 new DelegateFunctionDefinition(
                     ValueType.Int,
                     ValueType.Int,
                     SpawnObject
                     ));

            importer.DefineFunction("element_get_resource_index_by_id",
                 new DelegateFunctionDefinition(
                     ValueType.String,
                     ValueType.Int,
                     GetResourceIndexById
                     ));
            */
            return importer;
        }

        private IReadOnlyList<object> Invalid
        {
            get => ReturnValue.FromObject(-1);
        }
        private IReadOnlyList<object> InvalidElementIndex
        {
            get => ReturnValue.FromObject(-1);
        }
        private IReadOnlyList<object> ErrorResult
        {
            get => ReturnValue.FromObject(0);
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
            if (!parser.TryReadUInt(out uint iovs))
            {
                return ErrorResult;
            }
            if (!parser.TryReadUInt(out uint iovsLen))
            {
                return ErrorResult;
            }
            if (!parser.TryReadUInt(out uint nwritten))
            {
                return ErrorResult;
            }

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


            memory32[nwritten] = totalSize;
            Debug.Log($"fd: {fd}, iovsLen: {iovsLen}");
            /*
            if (!parser.TryReadInt(out var fd))
            {
                return ReturnValue.Unit;
            }
            */

            /*
            if (!parser.TryReadString(out var message))
            {
                return ReturnValue.Unit;
            }

            if (!parser.TryReadInt(out var nwritten))
            {
                return ReturnValue.Unit;
            }
            */


            //Debug.Log(message);
            //Debug.Log("Write");

            return ReturnValue.FromObject(0);
        }
        private IReadOnlyList<object> Close(IReadOnlyList<object> arg)
        {
            Debug.Log("Close");
            return ReturnValue.FromObject(1);
        }


        private IReadOnlyList<object> Connect(IReadOnlyList<object> arg)
        {
            Debug.Log("Connect");
            var parser = new ArgumentParser(arg, ModuleInstance);
            if (!parser.TryReadInt(out var ipv4Addr))
            {
                return Invalid;
            }
            if (!parser.TryReadInt(out var port))
            {
                return Invalid;
            }
            Debug.Log(ipv4Addr);
            Debug.Log(port);
            Debug.Log($"{ipv4Addr:x10}");
            /*

            var ipe = new IPEndPoint(ipv4Addr, port);
            var socket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipe);

            if (!socket.Connected)
            {
                return Invalid;
            }
            sockets.Add(socket);
            //var descriptor = 1;
            var descriptor = sockets.Count;
            return ReturnValue.FromObject(new List<object> { descriptor, 0 });
            */
            //return arg;
            return ReturnValue.FromObject(0);
        }

        private IReadOnlyList<object> Receive(IReadOnlyList<object> arg)
        {
            /*
            var parser = new ArgumentParser(arg, ModuleInstance);
            if (!parser.TryReadInt(out var ipv4Addr))
            {
                return Invalid;
            }
            if (!parser.TryReadInt(out var port))
            {
                return Invalid;
            }
            return ReturnValue.FromObject(new List<object> { 0, 0 });
            //return ReturnValue.Unit;
            */
            //return arg;
            Debug.Log("recv");
            return ReturnValue.FromObject(0);
        }
        
        private IReadOnlyList<object> Send(IReadOnlyList<object> arg)
        {
            Debug.Log("send");
            /*
            var parser = new ArgumentParser(arg, ModuleInstance);
            if (!parser.TryReadInt(out var ipv4Addr))
            {
                return Invalid;
            }
            if (!parser.TryReadInt(out var port))
            {
                return Invalid;
            }
            return ReturnValue.FromObject(new List<object> { 0, 0 });
            //return ReturnValue.Unit;
            */
            //return arg;
            return ReturnValue.FromObject(0);
        }

    }
}
