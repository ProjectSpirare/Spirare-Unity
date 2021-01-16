using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spirare
{
    public class PomlElementComponent : MonoBehaviour
    {
        public PomlElement PomlElement { set; get; }

        public bool Equipable
        {
            get => PomlElement?.Attribute.HasFlag(ElementAttributeType.Equipable) ?? false;
        }
    }
}
