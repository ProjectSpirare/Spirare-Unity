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
        private readonly Dictionary<int, Socket> sockets = new Dictionary<int, Socket>();

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
            get => ReturnValue.FromObject(1);
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
            if (!parser.TryReadPointer(out var fdPointer))
            {
                return Invalid;
            }

            Debug.Log(ipv4Addr);
            Debug.Log(port);
            Debug.Log($"{ipv4Addr:x10}");

            try
            {
                var ipBytes = BitConverter.GetBytes(ipv4Addr);
                Array.Reverse(ipBytes);
                var address = new IPAddress(ipBytes);
                var ipe = new IPEndPoint(address, port);
                var socket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ipe);

                if (!socket.Connected)
                {
                    return Invalid;
                }

                //                sockets.Add(socket);
                //var socketDescriptor = sockets.Count;
                var socketDescriptor = socket.Handle.ToInt32();
                sockets[socketDescriptor] = socket;

                Debug.Log($"Socket descriptor: {socketDescriptor}");
                var memory32 = ModuleInstance.Memories[0].Int32;
                memory32[fdPointer] = socketDescriptor;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                return ErrorResult;
            }
            /*
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
            var parser = new ArgumentParser(arg, ModuleInstance);
            if (!parser.TryReadInt(out var fd))
            {
                return Invalid;
            }

            Debug.Log(fd);

            if (!sockets.TryGetValue(fd, out var socket))
            {
                return Invalid;
            }
            try
            {
                var buffer = new byte[100];
                // var socket = sockets[fd - 1];
                socket.Receive(buffer);

                var text = System.Text.Encoding.UTF8.GetString(buffer);
                Debug.Log(text);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }



            /*
            if (!parser.TryReadVectoredBuffer(out var buffer))
            {
                return Invalid;
            }
            */

            return ReturnValue.FromObject(0);
        }

        private IReadOnlyList<object> Send(IReadOnlyList<object> arg)
        {
            Debug.Log("send");
            var parser = new ArgumentParser(arg, ModuleInstance);
            if (!parser.TryReadInt(out var fd))
            {
                return Invalid;
            }

            if (!parser.TryReadVectoredBuffer(out IList<ArraySegment<byte>> buffer))
            {
                return Invalid;
            }

            Debug.Log(fd);
            if (!sockets.TryGetValue(fd, out var socket))
            {
                return Invalid;
            }
            try
            {
                socket.Send(buffer);

                /*
                var text = System.Text.Encoding.UTF8.GetString(buffer);
                Debug.Log(text);
                */

                // TODO 送信バイト数の記録

            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }

            /*
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
