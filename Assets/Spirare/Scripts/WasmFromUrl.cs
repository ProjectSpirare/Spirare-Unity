using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

namespace Spirare
{
    public class WasmFromUrl : WasmBehaviour
    {
        [SerializeField]
        private string url = null;

        protected override void Awake()
        {
            base.Awake();
            if (!string.IsNullOrEmpty(url))
            {
                _ = LoadWasmFromUrl(url);
            }
        }

        public async Task LoadWasmFromUrl(string url, ContentsStore contentsStore = null, List<string> args = null)
        {
            var httpClient = new HttpClient();
            var result = await httpClient.GetAsync(url);
            if (result.IsSuccessStatusCode)
            {
                var stream = await result.Content.ReadAsStreamAsync();
                LoadWasm(stream, contentsStore, args);
            }
        }
    }
}
