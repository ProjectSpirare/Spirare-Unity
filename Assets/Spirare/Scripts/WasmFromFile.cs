using UnityEngine;

namespace Spirare
{
    public class WasmFromFile : WasmBehaviour
    {
        [SerializeField]
        private string filePath = null;

        protected override void Awake()
        {
            base.Awake();
            LoadWasm(filePath);
        }
    }
}
