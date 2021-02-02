using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wasm;

namespace Spirare
{
    public static class SpirareUtils
    {
        public static IReadOnlyList<object> Unit = new object[0];
    }

    public static class ReturnValue
    {
        public static IReadOnlyList<object> Unit = new object[0];
        public static IReadOnlyList<object> FromObject(object result1)
        {
            return new object[] { result1 };
        }
    }

    public static class ValueType
    {
        public static IReadOnlyList<WasmValueType> Unit = new WasmValueType[0];

        public static IReadOnlyList<WasmValueType> Float = new WasmValueType[]
        {
            WasmValueType.Float32,
        };

        public static IReadOnlyList<WasmValueType> ObjectId = new WasmValueType[]
        {
            WasmValueType.Int32,
        };

        public static IReadOnlyList<WasmValueType> Short = new WasmValueType[]
        {
            WasmValueType.Int32,
        };

        public static IReadOnlyList<WasmValueType> Int = new WasmValueType[]
        {
            WasmValueType.Int32,
        };


        public static IReadOnlyList<WasmValueType> Vector3 = new WasmValueType[]
        {
            WasmValueType.Float32,
            WasmValueType.Float32,
            WasmValueType.Float32,
        };

        public static IReadOnlyList<WasmValueType> IdAndVector3 = new WasmValueType[]
        {
            WasmValueType.Int32,
            WasmValueType.Float32,
            WasmValueType.Float32,
            WasmValueType.Float32,
        };

        public static IReadOnlyList<WasmValueType> Quaternion = new WasmValueType[]
        {
            WasmValueType.Float32,
            WasmValueType.Float32,
            WasmValueType.Float32,
            WasmValueType.Float32,
        };

        public static IReadOnlyList<WasmValueType> IdAndQuaternion = new WasmValueType[]
        {
            WasmValueType.Int32,
            WasmValueType.Float32,
            WasmValueType.Float32,
            WasmValueType.Float32,
            WasmValueType.Float32,
        };

        public static IReadOnlyList<WasmValueType> Int64 = new WasmValueType[]
        {
            WasmValueType.Int64,
        };

        public static IReadOnlyList<WasmValueType> String = new WasmValueType[]
        {
            WasmValueType.Int32,
            WasmValueType.Int32,
        };

        public static IReadOnlyList<WasmValueType> IdAndString = new WasmValueType[]
        {
            WasmValueType.Int32,
            WasmValueType.Int32,
            WasmValueType.Int32,
        };

        public static IReadOnlyList<WasmValueType> Pointer = new WasmValueType[]
        {
            WasmValueType.Int32,
        };

        public static IReadOnlyList<WasmValueType> PointerAndPointer = new WasmValueType[]
        {
            WasmValueType.Int32,
            WasmValueType.Int32,
        };
    }
}
