using UnityEngine;

namespace UniWasm
{
    public class WasmFromFile : WasmBehaviour
    {
        [SerializeField]
        private string filePath = null;

        private void Awake()
        {
            LoadWasm(filePath);
        }
    }
}
