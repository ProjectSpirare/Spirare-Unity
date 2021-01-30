using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spirare.WasmBinding
{
    internal enum CoordinateType
    {
        world = 0,
        local
    }

    internal enum Vector3ElementType
    {
        x = 0,
        y,
        z
    }

    internal static class Vector3Extension
    {
        public static float GetSpecificValue(this Vector3 vector3, Vector3ElementType element,
            bool toSpirareCoorinates = true,
            bool directional = true)
        {
            var vector = toSpirareCoorinates ? vector3.ToSpirareCoordinate(directional) : vector3;

            switch (element)
            {
                case Vector3ElementType.x:
                    return vector.x;
                case Vector3ElementType.y:
                    return vector.y;
                case Vector3ElementType.z:
                    return vector.z;
                default:
                    return float.NaN;
            }
        }

        private static Vector3 ToSpirareCoordinate(this Vector3 vector3, bool directional = true)
        {
            return CoordinateUtility.ToSpirareCoordinate(vector3, directional);
        }
    }

    internal enum QuaternionElementType
    {
        x = 0,
        y,
        z,
        w
    }

    internal static class QuaternionExtension
    {
        public static float GetSpecificValue(this Quaternion quaternion, QuaternionElementType element, bool toSpirareCoordinate = true)
        {
            var rotation = toSpirareCoordinate ? quaternion.ToSpirareCoordinate() : quaternion;
            switch (element)
            {
                case QuaternionElementType.x:
                    return rotation.x;
                case QuaternionElementType.y:
                    return rotation.y;
                case QuaternionElementType.z:
                    return rotation.z;
                case QuaternionElementType.w:
                    return rotation.w;
                default:
                    return float.NaN;
            }
        }
        private static Quaternion ToSpirareCoordinate(this Quaternion rotation)
        {
            return CoordinateUtility.ToSpirareCoordinate(rotation);
        }
    }
}
