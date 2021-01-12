using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using static Spirare.PomlElement;

namespace Spirare
{
    public class PomlLoader : MonoBehaviour
    {
        [SerializeField]
        private string path = null;

        private Transform resourceRoot;

        private ContentsStore contentsStore;

        private static readonly PomlParser parser = new PomlParser();

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

            LoadPoml(xmlDocument);
        }

        protected void LoadPoml(XmlDocument xmlDocument)
        {
            if (parser.TryParse(xmlDocument, path, out var poml))
            {
                LoadScene(poml.Scene);
                LoadResource(poml.Resource);
            }
        }

        protected void LoadScene(PomlScene scene)
        {
            foreach (var element in scene.Elements)
            {
                LoadElement(element, transform);
            }
        }

        protected void LoadResource(PomlResource pomlResource)
        {
            foreach (var element in pomlResource.Elements)
            {
                var t = LoadElement(element, transform);
                if (t == null)
                {
                    continue;
                }

                var resource = new Resource()
                {
                    Id = element.Id,
                    GameObject = t.gameObject
                };
                contentsStore.RegisterResource(resource);
            }
        }

        private Transform LoadElement(PomlElement element, Transform parent)
        {
            var t = GenerateElement(element, parent);

            if (t == null)
            {
                return null;
            }

            t.SetParent(parent, false);

            t.localPosition = element.Position;
            t.localScale = element.Scale;

            // Load child elements
            foreach (var child in element.Children)
            {
                LoadElement(child, t);
            }

            return t;
        }

        private Transform GenerateElement(PomlElement element, Transform parent)
        {
            switch (element.ElementType)
            {
                case PomlElementType.Element:
                    return InstantiateEmptyElement(element);
                case PomlElementType.Primitive:
                    if (element is PomlPrimitiveElement primitiveElement)
                    {
                        return InstantiatePrimitive(primitiveElement);
                    }
                    return null;
                case PomlElementType.Model:
                    return InstantiateModel(element);
                case PomlElementType.Script:
                    if (element is PomlScriptElement scriptElement)
                    {
                        AttachScript(scriptElement, parent);
                    }
                    return null;
                default:
                    return null;
            }
        }

        protected virtual Transform InstantiateEmptyElement(PomlElement element)
        {
            var go = new GameObject();
            return go.transform;
        }

        protected virtual Transform InstantiatePrimitive(PomlPrimitiveElement element)
        {
            if (TryGetUnityPrimitiveType(element.PrimitiveType, out var unityPrimitiveType))
            {
                var go = GameObject.CreatePrimitive(unityPrimitiveType);
                return go.transform;
            }
            Debug.LogWarning($"Primitive type {element.PrimitiveType} is invalid");
            return null;
        }

        protected virtual Transform InstantiateModel(PomlElement element)
        {
            var go = new GameObject();

            // TODO create load gltf
            var gltf = go.AddComponent<GltfEntity>();
            var srcPath = element.Src;
            gltf.Load(srcPath);
            return go.transform;
        }

        protected virtual WasmBehaviour AttachScript(PomlScriptElement element, Transform parent)
        {
            var src = element.Src;
            var args = element.Args;

            var wasm = parent.gameObject.AddComponent<WasmFromUrl>();
            if (src.StartsWith("http"))
            {
                _ = wasm.LoadWasmFromUrl(src, contentsStore, args);
            }
            else
            {
                wasm.LoadWasm(src, contentsStore, args);
            }
            return wasm;
        }


        private bool TryGetUnityPrimitiveType(PomlPrimitiveElement.PomlPrimitiveElementType primitiveType, out PrimitiveType unityPrimitiveType)
        {
            unityPrimitiveType = GetUnityPrimitiveType(primitiveType);
            return Enum.IsDefined(typeof(PrimitiveType), unityPrimitiveType);
        }

        private PrimitiveType GetUnityPrimitiveType(PomlPrimitiveElement.PomlPrimitiveElementType primitiveType)
        {
            switch (primitiveType)
            {
                case PomlPrimitiveElement.PomlPrimitiveElementType.Cube:
                    return PrimitiveType.Cube;
                case PomlPrimitiveElement.PomlPrimitiveElementType.Sphere:
                    return PrimitiveType.Sphere;
                case PomlPrimitiveElement.PomlPrimitiveElementType.Cylinder:
                    return PrimitiveType.Cylinder;
                case PomlPrimitiveElement.PomlPrimitiveElementType.Plane:
                    return PrimitiveType.Quad;
                case PomlPrimitiveElement.PomlPrimitiveElementType.Capsule:
                    return PrimitiveType.Capsule;
                default:
                    return (PrimitiveType)(-1);
            }
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

        /*
        private void LoadScene(XmlNode scene)
        {
            foreach (XmlNode node in scene.ChildNodes)
            {
                InstantiateNode(path, node, transform);
            }
        }
        */

        /*
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
        */

        /*
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
        */






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
        */


        /*
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
