using UnityEngine;

namespace TimelineKit
{
    /// <summary>
    /// ITimelineAssetLoader implementation backed by Unity's Resources system.
    /// Use the resourcePath field from TimelineAssetReference as the key.
    /// </summary>
    public class ResourcesAssetLoader : ITimelineAssetLoader
    {
        public T Load<T>(string key) where T : Object
            => Resources.Load<T>(key);

        public void Unload(string key)
            => Resources.UnloadUnusedAssets();
    }
}
