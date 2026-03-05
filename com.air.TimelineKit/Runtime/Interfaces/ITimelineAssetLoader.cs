using UnityEngine;

namespace TimelineKit
{
    /// <summary>
    /// Abstraction for loading assets referenced in a Timeline at runtime.
    /// Implement to support different loading backends (Resources, Addressables, AssetBundles).
    /// </summary>
    public interface ITimelineAssetLoader
    {
        /// <summary>Load an asset synchronously by its key (e.g. Resources path).</summary>
        T Load<T>(string key) where T : Object;

        /// <summary>Release a previously loaded asset by its key.</summary>
        void Unload(string key);
    }
}
