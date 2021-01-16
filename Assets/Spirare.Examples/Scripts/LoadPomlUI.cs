using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Spirare.Examples
{
    public class LoadPomlUI : MonoBehaviour
    {
        [SerializeField]
        private Button button = null;

        [SerializeField]
        private InputField inputField = null;

        void Awake()
        {
            button.onClick.AddListener(() =>
            {
                var url = inputField.text;
                _ = Load(url);
            });
        }

        private async Task Load(string url)
        {
            var pomlLoader = new GameObject();
            var loader = pomlLoader.AddComponent<PomlLoader>();
            await loader.LoadAsync(url);
        }
    }
}
