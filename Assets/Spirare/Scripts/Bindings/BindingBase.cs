using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Wasm.Interpret;

namespace Spirare
{
    public abstract class BindingBase
    {
        public ModuleInstance ModuleInstance { set; get; }
        public PredefinedImporter Importer { private set; get; }

        protected Element thisElement;
        protected ContentsStore store;

        public BindingBase(Element element, ContentsStore store)
        {
            Importer = GenerateImporter();
            this.thisElement = element;
            this.store = store;
        }

        public abstract PredefinedImporter GenerateImporter();

        protected bool TryGetElementWithArg(ArgumentParser parser, ContentsStore store, out Element element)
        {
            if (!parser.TryReadInt(out var elementIndex))
            {
                element = null;
                return false;
            }

            if (elementIndex == 0)
            {
                element = thisElement;
                return true;
            }

            if (!store.TryGetElementByElementIndex(elementIndex, out element))
            {
                return false;
            }

            return true;
        }
    }

    internal enum Vector3ElementType
    {
        x,
        y,
        z
    }

    internal static class Vector3Extension
    {
        public static float GetSpecificValue(this Vector3 vector3, Vector3ElementType element)
        {
            switch (element)
            {
                case Vector3ElementType.x:
                    return vector3.x;
                case Vector3ElementType.y:
                    return vector3.y;
                case Vector3ElementType.z:
                    return vector3.z;
                default:
                    return float.NaN;
            }
        }
    }

    /*
    private float GetValueForSpecificAxis(Vector3 vector3, Vector3AxisType axis)
    {
        switch (axis)
        {
            case Vector3AxisType.x:
                return vector3.x;
            case Vector3AxisType.y:
                return vector3.y;
            case Vector3AxisType.z:
                return vector3.z;
            default:
                return float.NaN;
        }
    }
    */
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

        public bool TryReadVector3(out Vector3 vector)
        {
            if (TryReadFloat(out var x) &&
                TryReadFloat(out var y) &&
                TryReadFloat(out var z))
            {
                vector = new Vector3(x, y, z);
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
                quaternion = new Quaternion(x, y, z, w);
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
    }
}
