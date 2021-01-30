using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spirare.WasmBinding
{
    internal class SpirareBindingBase
    {
        protected Element thisElement;
        protected ContentsStore store;

        public SpirareBindingBase(Element element, ContentsStore store)
        {
            thisElement = element;
            this.store = store;
        }


        protected bool TryGetElementWithArg(ArgumentParser parser, out Element element)
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

    internal class TransformBinding1 : SpirareBindingBase
    {
        public TransformBinding1(Element element, ContentsStore store) : base(element, store)
        {

        }

        private float InvalidFloatValue
        {
            get => float.NaN;
        }

        public float GetPosition(ArgumentParser parser, Vector3ElementType axis, CoordinateType coordinate)
        {
            if (coordinate == CoordinateType.world)
            {
                return GetTransformValue(parser, t => t.position.GetSpecificValue(axis));
            }
            else
            {
                return GetTransformValue(parser, t => t.localPosition.GetSpecificValue(axis));
            }
        }

        public void SetPosition(ArgumentParser parser, CoordinateType coordinate)
        {
            var directional = true;
            if (coordinate == CoordinateType.world)
            {
                SetVector3(parser, SetWorldPosition, directional);
            }
            else
            {
                SetVector3(parser, SetLocalPosition, directional);
            }
        }

        public float GetScale(ArgumentParser parser, Vector3ElementType axis, CoordinateType coordinate)
        {
            var directional = false;
            if (coordinate == CoordinateType.world)
            {
                return GetTransformValue(parser, t => t.lossyScale.GetSpecificValue(axis, directional));
            }
            else
            {
                return GetTransformValue(parser, t => t.localScale.GetSpecificValue(axis, directional));
            }
        }


        public void SetScale(ArgumentParser parser, CoordinateType coordinate)
        {
            var directional = false;
            if (coordinate == CoordinateType.world)
            {
                SetVector3(parser, SetWorldScale, directional);
            }
            else
            {
                SetVector3(parser, SetLocalScale, directional);
            }
        }

        private void SetLocalPosition(Transform transform, Vector3 position)
        {
            transform.localPosition = position;
        }

        private void SetWorldPosition(Transform transform, Vector3 position)
        {
            transform.position = position;
        }

        private void SetLocalScale(Transform transform, Vector3 scale)
        {
            transform.localScale = scale;
        }

        private void SetWorldScale(Transform transform, Vector3 scale)
        {
            transform.localScale = Vector3.one;
            var lossyScale = transform.lossyScale;

            Vector3 localScale;
            try
            {
                var x = scale.x / lossyScale.x;
                var y = scale.y / lossyScale.y;
                var z = scale.z / lossyScale.z;
                localScale = new Vector3(x, y, z);
            }
            catch (Exception)
            {
                localScale = scale;
            }
            transform.localScale = localScale;
        }

        private float GetTransformValue(ArgumentParser parser, Func<Transform, float> func)
        {
            if (!TryGetElementWithArg(parser, out var element))
            {
                return InvalidFloatValue;
            }
            var value = func.Invoke(element.GameObject.transform);
            return value;
        }

        private void SetVector3(ArgumentParser parser, Action<Transform, Vector3> action, bool directional = true)
        {
            if (!TryGetElementWithArg(parser, out var element))
            {
                return;
            }

            if (!parser.TryReadVector3(out var position, directional))
            {
                return;
            }
            action?.Invoke(element.GameObject.transform, position);
        }
    }
}
