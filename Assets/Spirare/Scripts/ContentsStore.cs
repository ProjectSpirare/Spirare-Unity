using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Spirare
{
    public class Resource
    {
        public string Id { set; get; }
        public GameObject GameObject { get; internal set; }
    }

    public class Element
    {
        public string Id { set; get; }
        public GameObject GameObject { get; internal set; }
    }


    public class ContentsStore
    {
        // private int elementIndexCounter = 1;

        // public readonly Dictionary<string, GameObject> Objects = new Dictionary<string, GameObject>();
        public readonly List<Element> Elements = new List<Element>();

        public readonly List<Resource> Resources = new List<Resource>();

        public Transform RootTransform;

        public int RegisterElement(Element element)
        {
            Elements.Add(element);
            // var id = elementIndexCounter;
            // elementIndexCounter += 1;

            // Objects.Add(id.ToString(), gameObject);
            return Elements.Count;
        }

        public void RegisterResource(Resource resource)
        {
            Resources.Add(resource);
        }

        public bool TryGetElementByElementIndex(int elementIndex, out Element element)
        {
            if (elementIndex <= 0 || elementIndex > Elements.Count)
            {
                element = null;
                return false;
            }
            element = Elements[elementIndex - 1];
            return true;
        }

        public bool TryGetResourceIndexById(string id, out int resourceIndex)
        {
            resourceIndex = Resources.FindIndex(x => x.Id == id);
            if (resourceIndex == -1)
            {
                return false;
            }

            // resource index is one-based index
            resourceIndex += 1;
            return true;
        }

        public bool TryGetResourceByResourceIndex(int resourceIndex, out Resource resource)
        {
            if (resourceIndex <= 0 || resourceIndex > Resources.Count)
            {
                resource = null;
                return false;
            }
            resource = Resources[resourceIndex - 1];
            return true;
        }
    }
}
