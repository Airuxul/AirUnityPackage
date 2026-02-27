using System;
using UnityEngine;

namespace TimelineExporter
{
    /// <summary>
    /// Serialized reference data for assets referenced by Timeline clips.
    /// Stores path, type, and optional runtime loadable identifier.
    /// </summary>
    [Serializable]
    public class TimelineReferenceData
    {
        [SerializeField] private string id;
        [SerializeField] private string assetPath;
        [SerializeField] private string assetType;
        [SerializeField] private string guid;

        public string Id
        {
            get => id;
            set => id = value;
        }

        public string AssetPath
        {
            get => assetPath;
            set => assetPath = value;
        }

        public string AssetType
        {
            get => assetType;
            set => assetType = value;
        }

        public string Guid
        {
            get => guid;
            set => guid = value;
        }
    }
}
