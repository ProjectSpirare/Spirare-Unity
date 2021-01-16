using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Spirare.Examples
{
    public class LoadWasmUI : MonoBehaviour
    {
        [SerializeField]
        WasmFromUrl wasmFromUrl = null;

        [SerializeField]
        private Button button = null;

        [SerializeField]
        private InputField inputField = null;

        void Awake()
        {
            button.onClick.AddListener(() =>
            {
                var url = inputField.text;
                _ = wasmFromUrl.LoadWasmFromUrl(url);
            });
        }
    }
}
