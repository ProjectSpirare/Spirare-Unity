using UnityEngine;

namespace Spirare
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
