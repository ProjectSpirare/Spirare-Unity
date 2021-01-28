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

                var socketDescriptor = socket.Handle.ToInt32();
                sockets[socketDescriptor] = socket;

                var memory32 = ModuleInstance.Memories[0].Int32;
                memory32[fdPointer] = socketDescriptor;
                return ReturnValue.FromObject(0);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                return ErrorResult;
            }
        }

        private IReadOnlyList<object> Receive(IReadOnlyList<object> arg)
        {
            var parser = new ArgumentParser(arg, ModuleInstance);
            if (!parser.TryReadInt(out var fd))
            {
                return Invalid;
            }

            if (!parser.TryReadUInt(out uint iovs) || !parser.TryReadUInt(out uint iovsLen))
            {
                return Invalid;
            }

            if (!parser.TryReadInt(out var riFlags))
            {
                return Invalid;
            }

            if (!parser.TryReadPointer(out var roDataLengthPointer))
            {
                return Invalid;
            }

            if (!parser.TryReadInt(out var roFlags))
            {
                return Invalid;
            }

            if (!sockets.TryGetValue(fd, out var socket))
            {
                return Invalid;
            }

            var receivedLengthSum = 0;
            var memory32 = ModuleInstance.Memories[0].Int32;
            var memory8 = ModuleInstance.Memories[0].Int8;
            try
            {
                for (uint i = 0; i < iovsLen; i++)
                {
                    var start = memory32[iovs + i * 8];
                    var length = memory32[iovs + i * 8 + 4];

                    var buffer = new byte[length];
                    var receivedLength = socket.Receive(buffer);

                    for (var j = 0; j < receivedLength; j++)
                    {
                        memory8[(uint)(start + j)] = (sbyte)buffer[j];
                    }

                    receivedLengthSum += receivedLength;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                return ErrorResult;
            }
            finally
            {
                memory32[roDataLengthPointer] = receivedLengthSum;

            }

            return ReturnValue.FromObject(0);
        }

        private IReadOnlyList<object> Send(IReadOnlyList<object> arg)
        {
            var parser = new ArgumentParser(arg, ModuleInstance);
            if (!parser.TryReadInt(out var fd))
            {
                return Invalid;
            }

            if (!parser.TryReadVectoredBuffer(out IList<ArraySegment<byte>> buffer))
            {
                return Invalid;
            }
            if (!parser.TryReadInt(out var siFlags))
            {
                return Invalid;
            }
            if (!parser.TryReadPointer(out var soDataLengthPointer))
            {
                return Invalid;
            }


            if (!sockets.TryGetValue(fd, out var socket))
            {
                return Invalid;
            }


            try
            {
                var sentMessageLength = socket.Send(buffer);
                var memory32 = ModuleInstance.Memories[0].Int32;
                memory32[soDataLengthPointer] = sentMessageLength;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                return ErrorResult;
            }

            return ReturnValue.FromObject(0);
        }
    }
}
