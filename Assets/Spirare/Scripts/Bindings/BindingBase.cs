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

    internal enum QuaternionElementType
    {
        x,
        y,
        z,
        w
    }

    internal static class QuaternionExtension
    {
        public static float GetSpecificValue(this Quaternion quaternion, QuaternionElementType element)
        {
            switch (element)
            {
                case QuaternionElementType.x:
                    return quaternion.x;
                case QuaternionElementType.y:
                    return quaternion.y;
                case QuaternionElementType.z:
                    return quaternion.z;
                case QuaternionElementType.w:
                    return quaternion.w;
                default:
                    return float.NaN;
            }
        }
    }
}
