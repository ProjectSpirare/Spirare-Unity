using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spirare
{
    public class Poml
    {
        public PomlScene Scene;
        public PomlResource Resource;
    }

    public class PomlScene
    {
        public List<PomlElement> Elements = new List<PomlElement>();
    }

    public class PomlResource
    {
        public List<PomlElement> Elements = new List<PomlElement>();
    }

    [Flags]
    public enum ElementAttributeType
    {
        None = 0,
        Static = 1,
        Equipable = 2,
    }

    public class PomlElement
    {
        public enum PomlElementType
        {
            None = 0,
            Element,
            Primitive,
            Model,
            Text,
            Script
        }

        public PomlElementType ElementType { get; protected set; }
        public ElementAttributeType Attribute;
        public string Id;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;
        public string Src;

        public List<PomlElement> Children = new List<PomlElement>();

        public PomlElement(PomlElementType elementType)
        {
            ElementType = elementType;
        }
    }

    public class PomlPrimitiveElement : PomlElement
    {
        public enum PomlPrimitiveElementType
        {
            None = 0,
            Cube,
            Sphere,
            Cylinder,
            Plane,
            Capsule
        }

        public PomlPrimitiveElementType PrimitiveType;

        public PomlPrimitiveElement() : base(PomlElementType.Primitive) { }
    }

    public class PomlTextElement : PomlElement
    {
        public string Text = "";

        public PomlTextElement() : base(PomlElementType.Text) { }
    }


    public class PomlScriptElement : PomlElement
    {
        public List<string> Args;

        public PomlScriptElement() : base(PomlElementType.Script) { }
    }
}
