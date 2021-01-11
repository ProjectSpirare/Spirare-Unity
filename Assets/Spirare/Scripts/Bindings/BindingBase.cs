using System;
using System.Collections;
using System.Collections.Generic;
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
        public static float GetSpecificValue(this Vector3 vector3, Vector3ElementType element, bool toSpirareCoorinates = true)
        {
            var vector = toSpirareCoorinates ? vector3.ToSpirareCoordinate() : vector3;

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

        private static Vector3 ToSpirareCoordinate(this Vector3 vector3)
        {
            return CoordinateUtility.ToSpirareCoordinate(vector3);
        }
    }

    internal enum QuaternionElementType
    {
        x,
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
