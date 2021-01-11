﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;

namespace Spirare
{
    internal class Poml
    {
        public PomlScene Scene;
    }

    internal class PomlScene
    {
        public List<PomlElement> Elements = new List<PomlElement>();
    }

    internal enum PomlElementType
    {
        None = 0,
        Element,
        Primitive,
        Model,
        Script
    }

    internal class PomlElement
    {
        public PomlElementType ElementType;
        public Vector3 Position;
        public Vector3 Scale;
        public string Src;
        public List<PomlElement> Children = new List<PomlElement>();
    }

    internal class PomlParser
    {
        public bool TryParse(XmlDocument xml, string basePath, out Poml poml)
        {
            var scene = xml.SelectSingleNode("//scene");
            var pomlScene = ParseScene(scene, basePath);

            /*
            var resource = xml.SelectSingleNode("//resource");
            LoadResource(resource);
            */

            poml = new Poml()
            {
                Scene = pomlScene
            };
            return true;
        }

        /*
        private void LoadResource(XmlNode resourceNode)
        {
            Debug.Log("LoadResource");
            if (resourceNode == null)
            {
                return;
            }
            foreach (XmlNode node in resourceNode.ChildNodes)
            {
                var t = InstantiateNode(path, node, resourceRoot);
                if (t == null)
                {
                    continue;
                }

                var id = ReadAttribute(node, "id", null);

                var resource = new Resource()
                {
                    Id = id,
                    GameObject = t.gameObject
                };
                contentsStore.RegisterResource(resource);

            }
        }
*/

        private PomlScene ParseScene(XmlNode scene, string basePath)
        {
            var elements = new List<PomlElement>();
            foreach (XmlNode node in scene.ChildNodes)
            {
                Debug.Log(node.ToString());
                Debug.Log(node.Name);
                Debug.Log(node.NodeType);

                var element = ParseElement(node, basePath);
                elements.Add(element);
            }

            var pomlScene = new PomlScene()
            {
                Elements = elements
            };
            return pomlScene;
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
            var elementType = GetElementType(node);

            var position = ReadVector3Attribute(node, "position", 0);
            var scale = ReadVector3Attribute(node, "scale", 1);

            // TODO rotation

            var src = node.GetAttribute("src");
            src = GetAbsolutePath(src, basePath);

            // child elements
            var childElements = new List<PomlElement>();
            foreach (XmlNode child in node.ChildNodes)
            {
                var childElement = ParseElement(child, basePath);
                childElements.Add(childElement);
            }

            var pomlElement = new PomlElement()
            {
                ElementType = elementType,
                Position = position,
                Scale = scale,
                Src = src,
                Children = childElements
            };
            return pomlElement;
        }

        private string GetAbsolutePath(string path, string basePath)
        {
            if (!Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out Uri uri))
            {
                // not a valid URI
                return path;
            }
            if (uri.IsAbsoluteUri)
            {
                return path;
            }
            return Path.Combine(basePath, "..", path);
        }
        /*
        private string GetAbsolutePath(string basePath, string relativePath)
        {
            var path = Path.Combine(basePath, "..", relativePath);
            return path;
        }
        */

        private float GetFloatAttribute(XmlNode node, string key, float defaultValue = 0)
        {
            var stringValue = node.GetAttribute(key);
            if (float.TryParse(stringValue, out var value))
            {
                return value;
            }
            return defaultValue;
        }

        private Vector3 ReadVector3Attribute(XmlNode node, string key, float defaultValue = 0)
        {
            var x = GetFloatAttribute(node, $"{key}.x", defaultValue);
            var y = GetFloatAttribute(node, $"{key}.y", defaultValue);
            var z = GetFloatAttribute(node, $"{key}.z", defaultValue);
            return new Vector3(x, y, z);
        }

        /*
        protected virtual WasmBehaviour AttatchScript(string path, XmlNode node, Transform parent)
        {
            if (!node.TryGetAttribute("src", out var src))
            {
                return null;
            }

            var srcPath = GetAbsolutePath(path, src);
            Debug.Log(srcPath);

            var args = ReadAttribute(node, "args", "");
            Debug.Log(args);

            var separator = new char[] { ' ' };
            var argsList = args.Split(separator, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            var wasm = parent.gameObject.AddComponent<WasmFromUrl>();
            if (srcPath.StartsWith("http"))
            {
                _ = wasm.LoadWasmFromUrl(srcPath, contentsStore, argsList);
            }
            else
            {
                wasm.LoadWasm(srcPath, contentsStore, argsList);
            }
            return wasm;
        }

        private Transform InstantiateElement(XmlNode node, Transform parent)
        {
            var go = new GameObject();
            return go.transform;
        }

        private Transform InstantiateModel(XmlNode node, Transform parent)
        {
            if (!node.TryGetAttribute("src", out var src))
            {
                return InstantiateElement(node, parent);
            }

            var srcPath = GetAbsolutePath(path, src);
            Debug.Log(srcPath);
            var go = new GameObject();
            var gltf = go.AddComponent<GltfEntity>();
            gltf.Load(srcPath);
            return go.transform;
        }

        protected virtual Transform InstantiatePrimitive(XmlNode node, Transform parent)
        {
            var type = node.Attributes["type"];
            if (type == null)
            {
                return null;
            }

            PrimitiveType primitiveType = PrimitiveType.Cube;
            switch (type.Value.ToLower())
            {
                case "cube":
                    primitiveType = PrimitiveType.Cube;
                    break;
                case "sphere":
                    primitiveType = PrimitiveType.Sphere;
                    break;
                case "cylinder":
                    primitiveType = PrimitiveType.Cylinder;
                    break;
            }

            var go = GameObject.CreatePrimitive(primitiveType);
            return go.transform;
        }

        private string GetAbsolutePath(string basePath, string relativePath)
        {
            var path = Path.Combine(basePath, "..", relativePath);
            return path;
        }
        */
    }
}