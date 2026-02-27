using System;
using System.Collections.Generic;
using UnityEngine;

namespace TimelineExporter
{
    /// <summary>
    /// Serialized clip data exported from Unity Timeline.
    /// Contains timing, type, and reference information for runtime playback.
    /// </summary>
    [Serializable]
    public class TimelineClipData
    {
        [SerializeField] private string id;
        [SerializeField] private double startTime;
        [SerializeField] private double duration;
        [SerializeField] private double clipIn;
        [SerializeField] private double timeScale = 1.0;
        [SerializeField] private double mixInDuration;
        [SerializeField] private double mixOutDuration;
        [SerializeField] private string clipType;
        [SerializeField] private string displayName;
        [SerializeField] private List<string> referenceIds = new List<string>();
        [SerializeField] private string customData;

        public string Id
        {
            get => id;
            set => id = value;
        }

        public double StartTime
        {
            get => startTime;
            set => startTime = value;
        }

        public double Duration
        {
            get => duration;
            set => duration = value;
        }

        public double ClipIn
        {
            get => clipIn;
            set => clipIn = value;
        }

        /// <summary>
        /// Blend-in duration in seconds. Used for weight when overlapping with previous clip.
        /// </summary>
        public double MixInDuration
        {
            get => mixInDuration;
            set => mixInDuration = value;
        }

        /// <summary>
        /// Blend-out duration in seconds. Used for weight when overlapping with next clip.
        /// </summary>
        public double MixOutDuration
        {
            get => mixOutDuration;
            set => mixOutDuration = value;
        }

        public double TimeScale
        {
            get => timeScale;
            set => timeScale = value;
        }

        public string ClipType
        {
            get => clipType;
            set => clipType = value;
        }

        public string DisplayName
        {
            get => displayName;
            set => displayName = value;
        }

        public List<string> ReferenceIds
        {
            get => referenceIds;
            set => referenceIds = value ?? new List<string>();
        }

        public string CustomData
        {
            get => customData;
            set => customData = value;
        }

        public double EndTime => startTime + duration;
    }
}
