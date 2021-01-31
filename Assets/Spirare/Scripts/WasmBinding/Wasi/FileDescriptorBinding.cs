using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace Spirare.WasmBinding
{
    internal class FileDescriptorBinding
    {
        private readonly Dictionary<int, Socket> sockets = new Dictionary<int, Socket>();

        private int Success
        {
            get => 0;
        }
        private int Invalid
        {
            get => 1;
        }
        private int ErrorResult
        {
            get => 1;
        }


        public int Read(ArgumentParser parser, MemoryReader memoryReader)
        {
            throw new NotImplementedException();
        }

        public int Write(ArgumentParser parser, MemoryReader memoryReader)
        {
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

                if (fd == 1)
                {
                    Debug.Log(text);
                }
                else if (fd == 2)
                {
                    Debug.LogError(text);
                }

                if (!memoryReader.TryWrite(nwritten, buffer.Length))
                {
                    return ErrorResult;
                }
                return Success;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                return ErrorResult;
            }
        }

        public int Close(ArgumentParser parser, MemoryReader memoryReader)
        {
            if (!parser.TryReadInt(out var fd))
            {
                return Invalid;
            }

            if (!sockets.TryGetValue(fd, out var socket))
            {
                return Invalid;
            }
            try
            {
                socket.Dispose();
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }

            sockets.Remove(fd);
            return Success;
        }

        #region Socket methods
        public int Connect(ArgumentParser parser, MemoryReader memoryReader)
        {
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

                if (memoryReader.TryWrite(fdPointer, socketDescriptor))
                {
                    return Success;
                }
                return ErrorResult;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                return ErrorResult;
            }
        }

        public int Receive(ArgumentParser parser, MemoryReader memoryReader)
        {
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
            try
            {
                for (uint i = 0; i < iovsLen; i++)
                {
                    if (!memoryReader.TryRead(iovs + i * 8, out int start))
                    {
                        return ErrorResult;
                    }
                    if (!memoryReader.TryRead(iovs + i * 8 + 4, out int length))
                    {
                        return ErrorResult;
                    }

                    var buffer = new byte[length];
                    var receivedLength = socket.Receive(buffer);

                    if (!memoryReader.TryWrite((uint)start, buffer))
                    {
                        return ErrorResult;
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
                memoryReader.TryWrite(roDataLengthPointer, receivedLengthSum);
            }

            return Success;
        }

        public int Send(ArgumentParser parser, MemoryReader memoryReader)
        {
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

                if (!memoryReader.TryWrite(soDataLengthPointer, sentMessageLength))
                {
                    return ErrorResult;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                return ErrorResult;
            }

            return Success;
        }
        #endregion
    }
}
