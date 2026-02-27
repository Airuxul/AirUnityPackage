using System;
using System.Collections.Generic;
using UnityEngine;

namespace TimelineExporter
{
    /// <summary>
    /// Root serialized data structure for exported Timeline.
    /// Contains tracks, references, and metadata for runtime playback simulation.
    /// </summary>
    [Serializable]
    public class TimelineData
    {
        [SerializeField] private string id;
        [SerializeField] private string name;
        [SerializeField] private double duration;
        [SerializeField] private double frameRate = 60.0;
        [SerializeField] private List<TimelineTrackData> tracks = new();
        [SerializeField] private List<TimelineReferenceData> references = new();
        [SerializeField] private List<TimelineResourceData> resources = new();
        [SerializeField] private string timelineAssetPath;
        [SerializeField] private string timelineResourcePath;
        [SerializeField] private int version = 1;

        /// <summary>
        /// Full asset path for Editor (e.g. Assets/Resources/TestTimeline.playable).
        /// Used by AssetDatabase.LoadAssetAtPath.
        /// </summary>
        public string TimelineAssetPath
        {
            get => timelineAssetPath;
            set => timelineAssetPath = value;
        }

        /// <summary>
        /// Path for Resources.Load when asset is under Assets/Resources/ (e.g. TestTimeline).
        /// Empty if not in Resources.
        /// </summary>
        public string TimelineResourcePath
        {
            get => timelineResourcePath;
            set => timelineResourcePath = value;
        }

        public string Id
        {
            get => id;
            set => id = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public double Duration
        {
            get => duration;
            set => duration = value;
        }

        public double FrameRate
        {
            get => frameRate;
            set => frameRate = value;
        }

        public List<TimelineTrackData> Tracks
        {
            get => tracks;
            set => tracks = value ?? new List<TimelineTrackData>();
        }

        /// <summary>
        /// Unified resource table. All clip/prefab references deduplicated by Guid.
        /// </summary>
        public List<TimelineResourceData> Resources
        {
            get
            {
                if (resources.Count == 0 && references.Count > 0)
                    MigrateReferencesToResources();
                return resources;
            }
            set => resources = value ?? new List<TimelineResourceData>();
        }

        private void MigrateReferencesToResources()
        {
            foreach (var r in references)
            {
                resources.Add(new TimelineResourceData
                {
                    Id = r.Id,
                    AssetPath = r.AssetPath,
                    AssetType = r.AssetType,
                    Guid = r.Guid,
                    ResourcePath = GetResourcePathFromAssetPath(r.AssetPath),
                    ResourceType = "ClipAsset"
                });
            }
        }

        private static string GetResourcePathFromAssetPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return "";
            const string token = "/Resources/";
            var idx = path.IndexOf(token, StringComparison.OrdinalIgnoreCase);
            if (idx < 0) return "";
            var sub = path.Substring(idx + token.Length);
            return string.IsNullOrEmpty(sub) ? "" : System.IO.Path.ChangeExtension(sub, null);
        }

        public int Version
        {
            get => version;
            set => version = value;
        }
    }
}
