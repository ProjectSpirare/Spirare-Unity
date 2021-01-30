using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spirare
{
    internal static class CoordinateUtility
    {
        public static Vector3 ToSpirareCoordinate(Vector3 vector3, bool directional = true)
        {
            if (directional)
            {
                return new Vector3(vector3.z, -vector3.x, vector3.y);
            }
            else
            {
                return new Vector3(vector3.z, vector3.x, vector3.y);
            }
        }

        public static Vector3 ToUnityCoordinate(float x, float y, float z, bool directional = true)
        {
            if (directional)
            {
                return new Vector3(-y, z, x);
            }
            else
            {
                return new Vector3(y, z, x);
            }
        }

        public static Quaternion ToSpirareCoordinate(Quaternion rotation)
        {
            return new Quaternion(rotation.z, -rotation.x, rotation.y, -rotation.w);
        }

        public static Quaternion ToUnityCoordinate(float x, float y, float z, float w)
        {
            return new Quaternion(-y, z, x, -w);
        }
    }
}
