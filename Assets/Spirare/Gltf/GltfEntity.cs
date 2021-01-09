using GLTFast;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GltfEntity : GltfAssetBase
{
    /*
    [Tooltip("URL to load the glTF from.")]
    public string url;

    [Tooltip("Automatically load at start.")]
    public bool loadOnStartup = true;

    [Tooltip("If checked, url is treated as relative StreamingAssets path.")]
    public bool streamingAsset = false;
    */

    /*
    protected virtual void Start()
    {
        Load(url)
        if (loadOnStartup && !string.IsNullOrEmpty(url))
        {
            // Automatic load on startup
            Load(
                streamingAsset
                    ? Path.Combine(Application.streamingAssetsPath, url)
                    : url
                );
        }
    }
    */

    /*
    public void Load(string url)
    {
        Load(url);
    }
    */

    protected override void OnLoadComplete(bool success)
    {
        if (success)
        {
            // Auto-Instantiate
            gLTFastInstance.InstantiateGltf(new CustomInstantiator(transform));
        }
        base.OnLoadComplete(success);
    }
}
