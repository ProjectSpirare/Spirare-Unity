using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
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

            importer.DefineFunction("sock_connect",
                 new DelegateFunctionDefinition(
                     new WasmValueType[] { WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32 },
                     new WasmValueType[] { WasmValueType.Int32, },
                     Connect
                     ));

            importer.DefineFunction("sock_send",
                 new DelegateFunctionDefinition(
                     new WasmValueType[] { WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32 },
                     new WasmValueType[] { WasmValueType.Int32, },
                     Send
                     ));

            importer.DefineFunction("sock_recv",
                 new DelegateFunctionDefinition(
                     new WasmValueType[] { WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32, WasmValueType.Int32 },
                     new WasmValueType[] { WasmValueType.Int32, },
                     Receive
                     ));
            return importer;
        }

        private IReadOnlyList<object> Invalid
        {
            get => ReturnValue.FromObject(-1);
        }
        private IReadOnlyList<object> ErrorResult
        {
            get => ReturnValue.FromObject(0);
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
