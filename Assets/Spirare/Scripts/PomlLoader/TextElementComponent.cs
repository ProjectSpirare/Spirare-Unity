using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spirare
{
    public class TextElementComponent : MonoBehaviour
    {
        private TextMesh textMesh;
        private void Awake()
        {
            textMesh = gameObject.AddComponent<TextMesh>();
        }

        private void Start()
        {
            transform.localScale = 0.0046f * transform.localScale;
            textMesh.fontSize = 100;
        }

        public void SetText(string text)
        {
            textMesh.text = text;
        }
    }
}
