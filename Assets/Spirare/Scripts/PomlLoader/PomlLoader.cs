using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
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
        private readonly HttpClient httpClient = new HttpClient();

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

            if (!string.IsNullOrEmpty(path))
            {
                _ = LoadAsync(path);
            }
        }

        public async Task<bool> LoadAsync(string uri)
        {
            var xml = await GetContentAsync(uri);
            var result = LoadXml(xml, uri);
            return result;
        }

        public bool LoadXml(string xml, string uri)
        {
            if (parser.TryParse(xml, uri, out var poml))
            {
                LoadScene(poml.Scene);
                LoadResource(poml.Resource);
                return true;
            }

            return false;
        }

        private async Task<string> GetContentAsync(string uriString)
        {
            try
            {
                var uri = new Uri(uriString);
                if (uri.IsFile)
                {
                    var text = File.ReadAllText(uriString);
                    return text;
                }
                else
                {
                    var response = await httpClient.GetAsync(uri);
                    if (!response.IsSuccessStatusCode)
                    {
                        return null;
                    }
                    var text = await response.Content.ReadAsStringAsync();
                    return text;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return null;
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
                var t = LoadElement(element, resourceRoot);
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

            var pomlElementComponent = t.gameObject.AddComponent<PomlElementComponent>();
            pomlElementComponent.PomlElement = element;

            t.SetParent(parent, false);

            t.localPosition = element.Position;
            t.localRotation = element.Rotation;
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
    }
}
