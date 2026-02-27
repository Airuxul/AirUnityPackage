using System;
using System.Collections.Generic;
using UnityEngine;

namespace TimelineExporter
{
    /// <summary>
    /// Serialized track data exported from Unity Timeline.
    /// Contains track metadata and clips for runtime playback.
    /// </summary>
    [Serializable]
    public class TimelineTrackData
    {
        [SerializeField] private string id;
        [SerializeField] private string name;
        [SerializeField] private string trackType;
        [SerializeField] private bool muted;
        [SerializeField] private string bindingType;
        [SerializeField] private List<TimelineClipData> clips = new();
        [SerializeField] private List<TimelineTrackData> childTracks = new();

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

        public string TrackType
        {
            get => trackType;
            set => trackType = value;
        }

        public bool Muted
        {
            get => muted;
            set => muted = value;
        }

        public string BindingType
        {
            get => bindingType;
            set => bindingType = value;
        }

        public List<TimelineClipData> Clips
        {
            get => clips;
            set => clips = value ?? new List<TimelineClipData>();
        }

        public List<TimelineTrackData> ChildTracks
        {
            get => childTracks;
            set => childTracks = value ?? new List<TimelineTrackData>();
        }
    }
}
