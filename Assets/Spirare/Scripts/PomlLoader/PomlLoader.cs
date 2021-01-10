using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;

namespace Spirare
{
    public class PomlLoader : MonoBehaviour
    {
        [SerializeField]
        private string path = null;

        private Transform resourceRoot;

        private ContentsStore contentsStore;

        private void Awake()
        {
            contentsStore = new ContentsStore()
            {
                RootTransform = transform
            };

            var resourceObject = new GameObject("resource");
            resourceObject.transform.localScale = Vector3.zero;
            // resourceObject.SetActive(false);
            resourceRoot = resourceObject.transform;
            resourceRoot.SetParent(transform, false);
            LoadFromFile(path);
        }

        public void LoadFromFile(string path)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(path);

            var scene = xmlDocument.SelectSingleNode("//scene");
            LoadScene(scene);

            var resource = xmlDocument.SelectSingleNode("//resource");
            LoadResource(resource);
        }

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

                /*
                if (!string.IsNullOrEmpty(id))
                {
                    Debug.Log(id);
                    // contentsStore.ResourceObjects.Add(id, t.gameObject);
                    // contentsStore.ResourceObjects.Add(id, t.gameObject);
                }
                */
            }
        }

        private void LoadScene(XmlNode scene)
        {
            foreach (XmlNode node in scene.ChildNodes)
            {
                /*
                Debug.Log(node.ToString());
                Debug.Log(node.Name);
                Debug.Log(node.NodeType);
                */

                InstantiateNode(path, node, transform);

            }
        }

        protected Transform InstantiateNode(string path, XmlNode node, Transform parent)
        {
            var tag = node.Name.ToLower();
            Transform t = null;
            switch (tag)
            {
                case "primitive":
                    t = InstantiatePrimitive(node, parent);
                    break;
                case "element":
                    t = InstantiateElement(node, parent);
                    break;
                case "model":
                    t = InstantiateModel(node, parent);
                    break;
                case "script":
                    AttatchScript(path, node, parent);
                    break;
                case "#comment":
                    break;
                default:
                    Debug.LogWarning($"Tag:{tag} is invalid");
                    break;
            }

            if (t == null)
            {
                return null;
            }

            t.SetParent(parent, false);

            t.localPosition = ReadVector3(node, "position", 0);
            t.localScale = ReadVector3(node, "scale", 1);

            // child elements
            foreach (XmlNode child in node.ChildNodes)
            {
                InstantiateNode(path, child, t);
            }

            return t;
        }

        private string ReadAttribute(XmlNode node, string key, string defaultValue = "")
        {
            try
            {
                var value = node.Attributes[key];
                if (value == null)
                {
                    return defaultValue;
                }
                return value.Value;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                return null;
            }
        }

        private float ReadAttribute(XmlNode node, string key, float defaultValue = 0)
        {
            var stringValue = ReadAttribute(node, key, "");
            if (float.TryParse(stringValue, out var value))
            {
                return value;
            }
            return defaultValue;
        }

        private Vector3 ReadVector3(XmlNode node, string key, float defaultValue = 0)
        {
            var x = ReadAttribute(node, $"{key}.x", defaultValue);
            var y = ReadAttribute(node, $"{key}.y", defaultValue);
            var z = ReadAttribute(node, $"{key}.z", defaultValue);
            return new Vector3(x, y, z);
        }

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
    }

    static class XmlNodeExtensions
    {
        public static bool TryGetAttribute(this XmlNode node, string key, out string value)
        {
            var type = node.Attributes[key];
            if (type == null)
            {
                value = null;
                return false;
            }

            value = type.Value;
            return true;
        }
    }
}
