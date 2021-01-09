using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wasm.Interpret;

namespace UniWasm
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
                         arg => GetTransformValue(arg, t => t.localScale.GetSpecificValue(axis))
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
                         arg => GetTransformValue(arg, t => t.lossyScale.GetSpecificValue(axis))
                         ));
            }

            importer.DefineFunction("transform_set_world_scale",
                 new DelegateFunctionDefinition(
                     ValueType.IdAndVector3,
                     ValueType.Unit,
                     SetWorldScale
                     ));

            /*
            importer.DefineFunction("transform_get_local_position_y",
                 new DelegateFunctionDefinition(
                     ValueType.ObjectId,
                     ValueType.Float,
                     _ => new object[] { transform.localPosition.y }
                     ));
            importer.DefineFunction("transform_get_local_position_z",
                 new DelegateFunctionDefinition(
                     ValueType.ObjectId,
                     ValueType.Float,
                     _ => new object[] { transform.localPosition.z }
                     ));
            // local rotation
            importer.DefineFunction("transform_set_local_rotation",
                 new DelegateFunctionDefinition(
                     ValueType.Quaternion,
                     ValueType.Unit,
                     SetLocalRotation
                     ));
            importer.DefineFunction("transform_get_local_rotation",
                 new DelegateFunctionDefinition(
                     ValueType.Unit,
                     ValueType.Quaternion,
                     GetLocalRotation
                     ));

            // world rotation
            importer.DefineFunction("transform_set_world_rotation",
                 new DelegateFunctionDefinition(
                     ValueType.IdAndQuaternion,
                     ValueType.Unit,
                     SetWorldRotation
                     ));
            importer.DefineFunction("transform_get_world_rotation_x",
                 new DelegateFunctionDefinition(
                     ValueType.ObjectId,
                     ValueType.Float,
                     _ => new object[] { transform.rotation.x }
                     ));
            importer.DefineFunction("transform_get_world_rotation_y",
                 new DelegateFunctionDefinition(
                     ValueType.ObjectId,
                     ValueType.Float,
                     _ => new object[] { transform.rotation.y }
                     ));
            importer.DefineFunction("transform_get_world_rotation_z",
                 new DelegateFunctionDefinition(
                     ValueType.ObjectId,
                     ValueType.Float,
                     _ => new object[] { transform.rotation.z }
                     ));
            importer.DefineFunction("transform_get_world_rotation_w",
                 new DelegateFunctionDefinition(
                     ValueType.ObjectId,
                     ValueType.Float,
                     _ => new object[] { transform.rotation.w }
                     ));

            importer.DefineFunction("transform_set_local_scale",
                 new DelegateFunctionDefinition(
                     ValueType.Vector3,
                     ValueType.Unit,
                     SetLocalScale
                     ));
            */

            /*
            importer.DefineFunction("transform_get_local_scale_x",
                 new DelegateFunctionDefinition(
                     ValueType.ObjectId,
                     ValueType.Float,
                     _ => new object[] { transform.localScale.x }
                     ));
            importer.DefineFunction("transform_get_local_scale_y",
                 new DelegateFunctionDefinition(
                     ValueType.ObjectId,
                     ValueType.Float,
                     _ => new object[] { transform.localScale.y }
                     ));
            importer.DefineFunction("transform_get_local_scale_z",
                 new DelegateFunctionDefinition(
                     ValueType.ObjectId,
                     ValueType.Float,
                     _ => new object[] { transform.localScale.z }
                     ));
            */
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

        private IReadOnlyList<object> SetVector3(IReadOnlyList<object> arg, Action<Transform, Vector3> action)
        {
            var parser = new ArgumentParser(arg);
            if (!TryGetElementWithArg(parser, store, out var element))
            {
                return ReturnValue.Unit;
            }

            if (!parser.TryReadVector3(out var position))
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
            return SetVector3(arg, (t, v) => t.localScale = v);
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

        private IReadOnlyList<object> SetWorldScale(IReadOnlyList<object> arg)
        {
            return SetVector3(arg, SetWorldScale);
        }
    }
}
