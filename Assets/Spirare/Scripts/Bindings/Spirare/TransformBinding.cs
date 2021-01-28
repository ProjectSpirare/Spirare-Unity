using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wasm.Interpret;

namespace Spirare
{
    public class TransformBinding : BindingBase
    {
        public TransformBinding(Element element, ContentsStore store) : base(element, store)
        {
        }

        public override PredefinedImporter GenerateImporter()
        {
            var importer = new PredefinedImporter();

            // Local position
            foreach (Vector3ElementType axis in Enum.GetValues(typeof(Vector3ElementType)))
            {
                importer.DefineFunction($"transform_get_local_position_{axis}",
                     new DelegateFunctionDefinition(
                         ValueType.ObjectId,
                         ValueType.Float,
                         arg => GetTransformValue(arg, t => t.localPosition.GetSpecificValue(axis))
                         ));
            }

            importer.DefineFunction("transform_set_local_position",
                 new DelegateFunctionDefinition(
                     ValueType.IdAndVector3,
                     ValueType.Unit,
                     SetLocalPosition
                     ));

            // World position
            foreach (Vector3ElementType axis in Enum.GetValues(typeof(Vector3ElementType)))
            {
                importer.DefineFunction($"transform_get_world_position_{axis}",
                     new DelegateFunctionDefinition(
                         ValueType.ObjectId,
                         ValueType.Float,
                         arg => GetTransformValue(arg, t => t.position.GetSpecificValue(axis))
                         ));
            }

            importer.DefineFunction("transform_set_world_position",
                 new DelegateFunctionDefinition(
                     ValueType.IdAndVector3,
                     ValueType.Unit,
                     SetWorldPosition
                     ));

            // Local scale
            foreach (Vector3ElementType axis in Enum.GetValues(typeof(Vector3ElementType)))
            {
                importer.DefineFunction($"transform_get_local_scale_{axis}",
                     new DelegateFunctionDefinition(
                         ValueType.ObjectId,
                         ValueType.Float,
                         arg => GetTransformValue(arg, t => t.localScale.GetSpecificValue(axis, directional: false))
                         ));
            }

            importer.DefineFunction("transform_set_local_scale",
                 new DelegateFunctionDefinition(
                     ValueType.IdAndVector3,
                     ValueType.Unit,
                     SetLocalScale
                     ));

            // World scale
            foreach (Vector3ElementType axis in Enum.GetValues(typeof(Vector3ElementType)))
            {
                importer.DefineFunction($"transform_get_world_scale_{axis}",
                     new DelegateFunctionDefinition(
                         ValueType.ObjectId,
                         ValueType.Float,
                         arg => GetTransformValue(arg, t => t.lossyScale.GetSpecificValue(axis, directional: false))
                         ));
            }

            importer.DefineFunction("transform_set_world_scale",
                 new DelegateFunctionDefinition(
                     ValueType.IdAndVector3,
                     ValueType.Unit,
                     SetWorldScale
                     ));

            // Local rotation
            var quaternionElements = Enum.GetValues(typeof(QuaternionElementType));
            foreach (QuaternionElementType axis in quaternionElements)
            {
                importer.DefineFunction($"transform_get_local_rotation_{axis}",
                     new DelegateFunctionDefinition(
                         ValueType.ObjectId,
                         ValueType.Float,
                         arg => GetTransformValue(arg, t => t.localRotation.GetSpecificValue(axis))
                         ));
            }

            importer.DefineFunction("transform_set_local_rotation",
                 new DelegateFunctionDefinition(
                     ValueType.IdAndQuaternion,
                     ValueType.Unit,
                     SetLocalRotation
                     ));

            // World position
            foreach (QuaternionElementType axis in quaternionElements)
            {
                importer.DefineFunction($"transform_get_world_rotation_{axis}",
                     new DelegateFunctionDefinition(
                         ValueType.ObjectId,
                         ValueType.Float,
                         arg => GetTransformValue(arg, t => t.rotation.GetSpecificValue(axis))
                         ));
            }

            importer.DefineFunction("transform_set_world_rotation",
                 new DelegateFunctionDefinition(
                     ValueType.IdAndQuaternion,
                     ValueType.Unit,
                     SetWorldRotation
                     ));
            return importer;
        }

        private IReadOnlyList<object> InvalidFloatValue
        {
            get => ReturnValue.FromObject(float.NaN);
        }

        private IReadOnlyList<object> GetTransformValue(IReadOnlyList<object> arg, Func<Transform, float> func)
        {
            var parser = new ArgumentParser(arg);

            if (!TryGetElementWithArg(parser, store, out var element))
            {
                return InvalidFloatValue;
            }
            var value = func.Invoke(element.GameObject.transform);
            return ReturnValue.FromObject(value);
        }

        private IReadOnlyList<object> SetVector3(IReadOnlyList<object> arg, Action<Transform, Vector3> action, bool directional = true)
        {
            var parser = new ArgumentParser(arg);
            if (!TryGetElementWithArg(parser, store, out var element))
            {
                return ReturnValue.Unit;
            }

            if (!parser.TryReadVector3(out var position, directional))
            {
                return ReturnValue.Unit;
            }
            action?.Invoke(element.GameObject.transform, position);
            return ReturnValue.Unit;
        }

        private IReadOnlyList<object> SetQuaternion(IReadOnlyList<object> arg, Action<Transform, Quaternion> action)
        {
            var parser = new ArgumentParser(arg);
            if (!TryGetElementWithArg(parser, store, out var element))
            {
                return ReturnValue.Unit;
            }

            if (!parser.TryReadQuaternion(out var quaternion))
            {
                return ReturnValue.Unit;
            }
            action?.Invoke(element.GameObject.transform, quaternion);
            return ReturnValue.Unit;
        }


        private IReadOnlyList<object> SetLocalPosition(IReadOnlyList<object> arg)
        {
            return SetVector3(arg, (t, v) => t.localPosition = v);
        }

        private IReadOnlyList<object> SetWorldPosition(IReadOnlyList<object> arg)
        {
            return SetVector3(arg, (t, v) => t.position = v);
        }

        private IReadOnlyList<object> SetLocalRotation(IReadOnlyList<object> arg)
        {
            return SetQuaternion(arg, (t, q) => t.localRotation = q);
        }

        private IReadOnlyList<object> SetWorldRotation(IReadOnlyList<object> arg)
        {
            return SetQuaternion(arg, (t, q) => t.rotation = q);
        }

        private IReadOnlyList<object> SetLocalScale(IReadOnlyList<object> arg)
        {
            return SetVector3(arg, (t, v) => t.localScale = v, directional: false);
        }

        private IReadOnlyList<object> SetWorldScale(IReadOnlyList<object> arg)
        {
            return SetVector3(arg, SetWorldScale, directional: false);
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
    }
}
