using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Wasm.Interpret;

namespace Spirare
{
    public class ArgumentParser
    {
        private IReadOnlyList<object> arg;
        private ModuleInstance moduleInstance;
        private int index;

        internal LinearMemory Memory
        {
            get
            {
                return moduleInstance.Memories[0];
            }
        }

        public ArgumentParser(IReadOnlyList<object> arg, ModuleInstance moduleInstance = null)
        {
            this.arg = arg;
            this.moduleInstance = moduleInstance;
        }

        public bool TryReadInt(out int value)
        {
            if (!TryReadObject(out var valueObject))
            {
                value = 0;
                return false;
            }

            try
            {
                value = (int)valueObject;
                return true;
            }
            catch (Exception e)
            {
                value = 0;
                Debug.LogWarning(e);
                return false;
            }
        }

        public bool TryReadUInt(out uint value)
        {
            if (!TryReadObject(out var valueObject))
            {
                value = 0;
                return false;
            }

            try
            {
                var intValue = (int)valueObject;
                value = InterpretAsUint(intValue);
                return true;
            }
            catch (Exception e)
            {
                value = 0;
                Debug.LogWarning(e);
                return false;
            }
        }

        public bool TryReadPointer(out uint ptr)
        {
            return TryReadUInt(out ptr);
        }

        public bool TryReadPointer(out uint ptr0, out uint ptr1)
        {
            ptr1 = 0;
            return TryReadPointer(out ptr0) && TryReadPointer(out ptr1);
        }

        public bool TryReadLong(out long value)
        {
            if (!TryReadObject(out var valueObject))
            {
                value = 0;
                return false;
            }

            try
            {
                value = (long)(int)valueObject;
                return true;
            }
            catch (Exception e)
            {
                value = 0;
                Debug.LogWarning(e);
                return false;
            }
        }

        public bool TryReadFloat(out float value)
        {
            if (!TryReadObject(out var valueObject))
            {
                value = 0;
                return false;
            }

            try
            {
                value = (float)valueObject;
                return true;
            }
            catch (Exception e)
            {
                value = 0;
                Debug.LogWarning(e);
                return false;
            }
        }

        public bool TryReadString(out string value)
        {
            if (!TryReadInt(out var pointer) || !TryReadInt(out var length))
            {
                value = "";
                return false;
            }

            var memory = Memory;
            var data = new byte[length];

            try
            {
                for (var i = 0; i < length; i++)
                {
                    var index = (uint)(pointer + i);
                    data[i] = (byte)memory.Int8[index];
                }
                value = Encoding.UTF8.GetString(data);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                value = "";
                return false;
            }
        }

        public bool TryReadVectoredBuffer(out byte[] buffer)
        {
            buffer = new byte[0];

            if (!TryReadUInt(out uint iovs) || !TryReadUInt(out uint iovsLen))
            {
                return false;
            }

            try
            {
                var memory32 = Memory.Int32;
                for (uint i = 0; i < iovsLen; i++)
                {
                    var start = memory32[iovs + i * 8];
                    var length = memory32[iovs + i * 8 + 4];

                    var data = ReadBytes(start, length);
                    var offset = buffer.Length;
                    Array.Resize(ref buffer, offset + length);
                    Array.Copy(data, 0, buffer, offset, length);
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                return false;
            }
        }


        public bool TryReadVector3(out Vector3 vector, bool directional = true)
        {
            if (TryReadFloat(out var x) &&
                TryReadFloat(out var y) &&
                TryReadFloat(out var z))
            {
                vector = CoordinateUtility.ToUnityCoordinate(x, y, z, directional);
                return true;
            }

            vector = Vector3.zero;
            return false;
        }

        public bool TryReadQuaternion(out Quaternion quaternion)
        {
            if (TryReadFloat(out var x) &&
                TryReadFloat(out var y) &&
                TryReadFloat(out var z) &&
                TryReadFloat(out var w))
            {
                quaternion = CoordinateUtility.ToUnityCoordinate(x, y, z, w);
                return true;
            }

            quaternion = Quaternion.identity;
            return false;
        }

        public static uint InterpretAsUint(int value)
        {
            try
            {
                var bytes = BitConverter.GetBytes(value);
                return BitConverter.ToUInt32(bytes, 0);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static int InterpretAsInt(uint value)
        {
            try
            {
                var bytes = BitConverter.GetBytes(value);
                return BitConverter.ToInt32(bytes, 0);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private bool TryReadObject(out object value)
        {
            if (arg.Count <= index)
            {
                value = null;
                return false;
            }

            value = arg[index];
            index += 1;
            return true;
        }

        private byte[] ReadBytes(int start, int length)
        {
            var data = new byte[length];
            for (var j = 0; j < length; j++)
            {
                var index = (uint)(start + j);
                data[j] = (byte)Memory.Int8[index];
            }
            return data;
        }
    }
}
