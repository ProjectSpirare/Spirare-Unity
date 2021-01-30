using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wasm.Interpret;

namespace Spirare.WasmBinding.CsWasm
{
    public class TransformBinding : BindingBase
    {
        private readonly TransformBinding1 transformBinding;

        public TransformBinding(Element element, ContentsStore store) : base(element, store)
        {
            transformBinding = new TransformBinding1(element, store);
        }

        public override PredefinedImporter GenerateImporter()
        {
            var importer = new PredefinedImporter();

            var coordinateValues = Enum.GetValues(typeof(CoordinateType));
            var vector3ElementValues = Enum.GetValues(typeof(Vector3ElementType));

            // Position
            foreach (CoordinateType coordinate in coordinateValues)
            {
                foreach (Vector3ElementType axis in vector3ElementValues)
                {
                    var functionName = $"transform_get_{coordinate}_position_{axis}";
                    importer.DefineFunction(functionName,
                         new DelegateFunctionDefinition(
                             ValueType.ObjectId,
                             ValueType.Float,
                             arg => GetPosition(arg, axis, coordinate)
                             ));
                }

                importer.DefineFunction($"transform_set_{coordinate}_position",
                     new DelegateFunctionDefinition(
                         ValueType.IdAndVector3,
                         ValueType.Unit,
                         arg => SetPosition(arg, coordinate)
                         ));
            }

            // Scale
            foreach (CoordinateType coordinate in coordinateValues)
            {
                foreach (Vector3ElementType axis in vector3ElementValues)
                {
                    var functionName = $"transform_get_{coordinate}_scale_{axis}";
                    importer.DefineFunction(functionName,
                         new DelegateFunctionDefinition(
                             ValueType.ObjectId,
                             ValueType.Float,
                             arg => GetScale(arg, axis, coordinate)
                             ));
                }

                importer.DefineFunction($"transform_set_{coordinate}_scale",
                     new DelegateFunctionDefinition(
                         ValueType.IdAndVector3,
                         ValueType.Unit,
                         arg => SetScale(arg, coordinate)
                         ));
            }

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

            // World rotation
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

        private IReadOnlyList<object> GetPosition(IReadOnlyList<object> arg, Vector3ElementType axis, CoordinateType coordinate)
        {
            var parser = new ArgumentParser(arg);
            var value = transformBinding.GetPosition(parser, axis, coordinate);
            return ReturnValue.FromObject(value);
        }

        private IReadOnlyList<object> SetPosition(IReadOnlyList<object> arg, CoordinateType coordinate)
        {
            var parser = new ArgumentParser(arg);
            transformBinding.SetPosition(parser, coordinate);
            return ReturnValue.Unit;
        }

        private IReadOnlyList<object> GetScale(IReadOnlyList<object> arg, Vector3ElementType axis, CoordinateType coordinate)
        {
            var parser = new ArgumentParser(arg);
            var value = transformBinding.GetScale(parser, axis, coordinate);
            return ReturnValue.FromObject(value);
        }

        private IReadOnlyList<object> SetScale(IReadOnlyList<object> arg, CoordinateType coordinate)
        {
            var parser = new ArgumentParser(arg);
            transformBinding.SetScale(parser, coordinate);
            return ReturnValue.Unit;
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

        private IReadOnlyList<object> SetLocalRotation(IReadOnlyList<object> arg)
        {
            return SetQuaternion(arg, (t, q) => t.localRotation = q);
        }

        private IReadOnlyList<object> SetWorldRotation(IReadOnlyList<object> arg)
        {
            return SetQuaternion(arg, (t, q) => t.rotation = q);
        }

    }
}
