using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using static Spirare.PomlElement;

namespace Spirare
{
    internal class PomlParser
    {
        public bool TryParse(string xml, string basePath, out Poml poml)
        {
            try
            {
                poml = ParseXml(xml, basePath);
                return true;
            }
            catch (XmlException)
            {
                try
                {
                    var modifiedXml = $"<root>{xml}</root>";
                    poml = ParseXml(modifiedXml, basePath);
                    return true;
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }

            poml = null;
            return false;
        }

        private Poml ParseXml(string xml, string basePath)
        {
            var parseXml = xml;
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(parseXml);

            var scene = xmlDocument.SelectSingleNode("//scene");
            var pomlScene = ParseScene(scene, basePath);

            var resource = xmlDocument.SelectSingleNode("//resource");
            var pomlResource = ParseResource(resource, basePath);

            var poml = new Poml()
            {
                Scene = pomlScene,
                Resource = pomlResource
            };

            return poml;
        }

        private PomlScene ParseScene(XmlNode scene, string basePath)
        {
            var elements = ParseElements(scene, basePath);

            var pomlScene = new PomlScene()
            {
                Elements = elements
            };
            return pomlScene;
        }

        private PomlResource ParseResource(XmlNode resource, string basePath)
        {
            var elements = ParseElements(resource, basePath);

            var pomlResource = new PomlResource()
            {
                Elements = elements
            };
            return pomlResource;
        }

        private List<PomlElement> ParseElements(XmlNode rootNode, string basePath)
        {
            var elements = new List<PomlElement>();
            if (rootNode == null)
            {
                return elements;
            }

            foreach (XmlNode node in rootNode.ChildNodes)
            {
                var element = ParseElement(node, basePath);
                elements.Add(element);
            }
            return elements;
        }


        private PomlElementType GetElementType(XmlNode node)
        {
            var tag = node.Name.ToLower();
            switch (tag)
            {
                case "element":
                    return PomlElementType.Element;
                case "primitive":
                    return PomlElementType.Primitive;
                case "model":
                    return PomlElementType.Model;
                case "text":
                    return PomlElementType.Text;
                case "script":
                    return PomlElementType.Script;
                case "#comment":
                    return PomlElementType.None;
                default:
                    Debug.LogWarning($"Tag:{tag} is invalid");
                    return PomlElementType.None;
            }
        }

        protected PomlElement ParseElement(XmlNode node, string basePath)
        {
            // child elements
            var childElements = new List<PomlElement>();
            foreach (XmlNode child in node.ChildNodes)
            {
                var childElement = ParseElement(child, basePath);
                childElements.Add(childElement);
            }

            var pomlElement = InitElement(node, basePath);

            // Read common attributes
            var id = node.GetAttribute("id", null);

            var attribute = ReadElementAttributeAttribute(node);

            // transform
            var position = ReadVector3Attribute(node, "position", 0, directional: true);
            var scale = ReadVector3Attribute(node, "scale", 1, directional: false);
            var rotation = ReadRotationAttribute(node);

            var src = node.GetAttribute("src", null);
            if (src != null)
            {
                src = FilePathUtility.GetAbsolutePath(src, basePath);
            }

            pomlElement.Children = childElements;
            pomlElement.Attribute = attribute;
            pomlElement.Id = id;
            pomlElement.Position = position;
            pomlElement.Scale = scale;
            pomlElement.Rotation = rotation;
            pomlElement.Src = src;
            return pomlElement;
        }

        private ElementAttributeType ReadElementAttributeAttribute(XmlNode node)
        {
            var attributeString = node.GetAttribute("attribute");

            var separator = new char[] { ' ' };
            var attributeArray = attributeString.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            var attribute = ElementAttributeType.None;
            foreach (var attributeToken in attributeArray)
            {
                switch (attributeToken.ToLower())
                {
                    case "static":
                        attribute |= ElementAttributeType.Static;
                        break;
                    case "equipable":
                    case "equippable":
                        attribute |= ElementAttributeType.Equipable;
                        break;
                }
            }
            return attribute;
        }

        private Quaternion ReadRotationAttribute(XmlNode node)
        {
            if (node.TryGetAttribute("rotation", out var attribute))
            {
                var values = ReadFloatArray(attribute);
                if (values.Count < 4)
                {
                    return Quaternion.identity;
                }

                var x = values[0];
                var y = values[1];
                var z = values[2];
                var w = values[3];
                var rotation = CoordinateUtility.ToUnityCoordinate(x, y, z, w);
                rotation.Normalize();
                return rotation;
            }

            return Quaternion.identity;
        }


        protected PomlElement InitElement(XmlNode node, string basePath)
        {
            var elementType = GetElementType(node);
            switch (elementType)
            {
                case PomlElementType.Primitive:
                    return InitPrimitiveElement(node, basePath);
                case PomlElementType.Script:
                    return InitScriptElement(node, basePath);
                case PomlElementType.Text:
                    return InitTextElement(node);
                default:
                    var pomlElement = new PomlElement(elementType);
                    return pomlElement;
            }
        }

        protected PomlPrimitiveElement.PomlPrimitiveElementType ConvertToPrimitiveElementType(string type)
        {
            switch (type.ToLower())
            {
                case "cube":
                    return PomlPrimitiveElement.PomlPrimitiveElementType.Cube;
                case "sphere":
                    return PomlPrimitiveElement.PomlPrimitiveElementType.Sphere;
                case "cylinder":
                    return PomlPrimitiveElement.PomlPrimitiveElementType.Cylinder;
                case "plane":
                    return PomlPrimitiveElement.PomlPrimitiveElementType.Plane;
                case "capsule":
                    return PomlPrimitiveElement.PomlPrimitiveElementType.Capsule;
                default:
                    return PomlPrimitiveElement.PomlPrimitiveElementType.None;
            }
        }

        protected PomlElement InitPrimitiveElement(XmlNode node, string basePath)
        {
            var primitiveTypeString = node.GetAttribute("shape", "");
            var primitiveType = ConvertToPrimitiveElementType(primitiveTypeString);

            return new PomlPrimitiveElement()
            {
                PrimitiveType = primitiveType
            };
        }

        private PomlElement InitTextElement(XmlNode node)
        {
            var text = node.GetAttribute("text");

            return new PomlTextElement()
            {
                Text = text
            };
        }

        protected PomlElement InitScriptElement(XmlNode node, string basePath)
        {
            var args = node.GetAttribute("args", "");

            var separator = new char[] { ' ' };
            var argsList = args.Split(separator, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            return new PomlScriptElement()
            {
                Args = argsList
            };
        }

        private float ReadFloatAttribute(XmlNode node, string key, float defaultValue = 0)
        {
            var stringValue = node.GetAttribute(key);
            if (float.TryParse(stringValue, out var value))
            {
                return value;
            }
            return defaultValue;
        }

        private Vector3 ReadVector3Attribute(XmlNode node, string key, float defaultValue, bool directional = true)
        {
            if (!node.TryGetAttribute(key, out var attribute))
            {
                return new Vector3(defaultValue, defaultValue, defaultValue);
            }

            var values = ReadFloatArray(attribute);
            var x = GetValueByIndex(values, 0, defaultValue);
            var y = GetValueByIndex(values, 1, defaultValue);
            var z = GetValueByIndex(values, 2, defaultValue);
            var vector = CoordinateUtility.ToUnityCoordinate(x, y, z, directional);
            return vector;
        }


        private static List<float> ReadFloatArray(string text)
        {
            var values = new List<float>();
            var separator = new char[] { ',', ' ' };
            var tokens = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            foreach (var token in tokens)
            {
                if (!float.TryParse(token, out var value))
                {
                    break;
                }
                values.Add(value);
            }
            return values;
        }

        private static float GetValueByIndex(List<float> list, int index, float defaultValue)
        {
            if (list.Count > index)
            {
                return list[index];
            }
            return defaultValue;
        }
    }
}
