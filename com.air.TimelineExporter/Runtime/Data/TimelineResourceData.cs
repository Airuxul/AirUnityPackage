using System;
using UnityEngine;

namespace TimelineExporter
{
    /// <summary>
    /// Unified resource entry for all assets referenced by Timeline (clip assets, prefabs, etc.).
    /// Multiple clips referencing the same asset share one entry. Used for Resources.Load at runtime.
    /// </summary>
    [Serializable]
    public class TimelineResourceData
    {
        [SerializeField] private string id;
        [SerializeField] private string assetPath;
        [SerializeField] private string assetType;
        [SerializeField] private string guid;
        [SerializeField] private string resourcePath;
        [SerializeField] private string resourceType;

        /// <summary>
        /// Unique id for lookup. Same as Guid when available.
        /// </summary>
        public string Id
        {
            get => id;
            set => id = value;
        }

        /// <summary>
        /// Full asset path in project (e.g. Assets/Resources/Prefabs/Square.prefab).
        /// </summary>
        public string AssetPath
        {
            get => assetPath;
            set => assetPath = value;
        }

        /// <summary>
        /// Full type name of the asset.
        /// </summary>
        public string AssetType
        {
            get => assetType;
            set => assetType = value;
        }

        /// <summary>
        /// Asset GUID for deduplication.
        /// </summary>
        public string Guid
        {
            get => guid;
            set => guid = value;
        }

        /// <summary>
        /// Path for Resources.Load when asset is under Assets/Resources/. Empty otherwise.
        /// </summary>
        public string ResourcePath
        {
            get => resourcePath;
            set => resourcePath = value;
        }

        /// <summary>
        /// Resource kind: ClipAsset, Prefab, etc.
        /// </summary>
        public string ResourceType
        {
            get => resourceType;
            set => resourceType = value;
        }
    }
}
