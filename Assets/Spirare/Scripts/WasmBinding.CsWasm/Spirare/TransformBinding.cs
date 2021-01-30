using System;
using System.Collections;
using System.Collections.Generic;
using Wasm.Interpret;

namespace Spirare.WasmBinding.CsWasm
{
    public class TransformBinding : BindingBase
    {
        private readonly WasmBinding.TransformBinding transformBinding;

        public TransformBinding(Element element, ContentsStore store) : base(element, store)
        {
            transformBinding = new WasmBinding.TransformBinding(element, store);
        }

        public override PredefinedImporter GenerateImporter()
        {
            var importer = new PredefinedImporter();

            var coordinateValues = Enum.GetValues(typeof(CoordinateType));
            var vector3ElementValues = Enum.GetValues(typeof(Vector3ElementType));
            var quaternionElementType = Enum.GetValues(typeof(QuaternionElementType));

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
                             arg => Invoke(arg, parser => GetPosition(parser, axis, coordinate))
                             ));
                }

                importer.DefineFunction($"transform_set_{coordinate}_position",
                     new DelegateFunctionDefinition(
                         ValueType.IdAndVector3,
                         ValueType.Unit,
                         arg => Invoke(arg, parser => SetPosition(parser, coordinate))
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
                             arg => Invoke(arg, parser => GetScale(parser, axis, coordinate))
                             ));
                }

                importer.DefineFunction($"transform_set_{coordinate}_scale",
                     new DelegateFunctionDefinition(
                         ValueType.IdAndVector3,
                         ValueType.Unit,
                         arg => Invoke(arg, parser => SetScale(parser, coordinate))
                         ));
            }

            // Rotation
            foreach (CoordinateType coordinate in coordinateValues)
            {
                foreach (QuaternionElementType axis in quaternionElementType)
                {
                    var functionName = $"transform_get_{coordinate}_rotation_{axis}";
                    importer.DefineFunction(functionName,
                         new DelegateFunctionDefinition(
                             ValueType.ObjectId,
                             ValueType.Float,
                             arg => Invoke(arg, parser => GetRotation(parser, axis, coordinate))
                             ));
                }

                importer.DefineFunction($"transform_set_{coordinate}_rotation",
                     new DelegateFunctionDefinition(
                         ValueType.IdAndQuaternion,
                         ValueType.Unit,
                         arg => Invoke(arg, parser => SetRotation(parser, coordinate))
                         ));
            }

            return importer;
        }

        private object GetPosition(ArgumentParser parser, Vector3ElementType axis, CoordinateType coordinate)
        {
            return transformBinding.GetPosition(parser, axis, coordinate);
        }

        private void SetPosition(ArgumentParser parser, CoordinateType coordinate)
        {
            transformBinding.SetPosition(parser, coordinate);
        }

        private object GetScale(ArgumentParser parser, Vector3ElementType axis, CoordinateType coordinate)
        {
            return transformBinding.GetScale(parser, axis, coordinate);
        }

        private void SetScale(ArgumentParser parser, CoordinateType coordinate)
        {
            transformBinding.SetScale(parser, coordinate);
        }

        private object GetRotation(ArgumentParser parser, QuaternionElementType axis, CoordinateType coordinate)
        {
            return transformBinding.GetRotation(parser, axis, coordinate);
        }

        private void SetRotation(ArgumentParser parser, CoordinateType coordinate)
        {
            transformBinding.SetRotation(parser, coordinate);
        }
    }
}
