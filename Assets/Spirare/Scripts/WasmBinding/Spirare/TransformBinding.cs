using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spirare.WasmBinding
{
    internal class TransformBinding : SpirareBindingBase
    {
        public TransformBinding(Element element, ContentsStore store) : base(element, store)
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

        public float GetRotation(ArgumentParser parser, QuaternionElementType axis, CoordinateType coordinate)
        {
            if (coordinate == CoordinateType.world)
            {
                return GetTransformValue(parser, t => t.rotation.GetSpecificValue(axis));
            }
            else
            {
                return GetTransformValue(parser, t => t.localRotation.GetSpecificValue(axis));
            }
        }

        public void SetRotation(ArgumentParser parser, CoordinateType coordinate)
        {
            if (coordinate == CoordinateType.world)
            {
                SetQuaternion(parser, SetWorldRotation);
            }
            else
            {
                SetQuaternion(parser, SetLocalRotation);
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

        private void SetLocalRotation(Transform transform, Quaternion rotation)
        {
            transform.localRotation = rotation;
        }

        private void SetWorldRotation(Transform transform, Quaternion rotation)
        {
            transform.rotation = rotation;
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

        private void SetQuaternion(ArgumentParser parser, Action<Transform, Quaternion> action)
        {
            if (!TryGetElementWithArg(parser, out var element))
            {
                return;
            }

            if (!parser.TryReadQuaternion(out var rotation))
            {
                return;
            }
            action?.Invoke(element.GameObject.transform, rotation);
        }
    }
}
