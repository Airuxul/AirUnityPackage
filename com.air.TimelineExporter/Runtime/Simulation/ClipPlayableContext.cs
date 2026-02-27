using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

namespace TimelineExporter
{
    /// <summary>
    /// IClipContext implementation. Combines playable context with clip/track data.
    /// </summary>
    public class ClipPlayableContext : IClipContext
    {
        public IPlayableContext Context { get; set; }
        public TimelineClipData Clip { get; set; }
        public TimelineTrackData Track { get; set; }
        public TimelineData TimelineData { get; set; }
        public Dictionary<string, GameObject> Bindings { get; set; }
        public Dictionary<string, TimelinePlayer> SubTimelinePlayers { get; set; }
        public Transform BindingRoot { get; set; }
        public TimelineAsset CachedTimelineAsset { get; set; }

        IPlayableHandle IClipContext.Playable => Context;
        TimelineData IClipContext.TimelineData => TimelineData;
        IFrameData IClipContext.FrameData => Context;
        TimelineClipData IClipContext.Clip => Clip;
        TimelineTrackData IClipContext.Track => Track;
        IReadOnlyDictionary<string, GameObject> IClipContext.Bindings => Bindings ?? new Dictionary<string, GameObject>();
        IReadOnlyDictionary<string, TimelinePlayer> IClipContext.SubTimelinePlayers => SubTimelinePlayers ?? new Dictionary<string, TimelinePlayer>();
        Transform IClipContext.BindingRoot => BindingRoot;
        TimelineAsset IClipContext.CachedTimelineAsset => CachedTimelineAsset;

        public bool TryRegisterBinding(string key, GameObject target)
        {
            if (Bindings == null || string.IsNullOrEmpty(key) || target == null) return false;
            Bindings[key] = target;
            return true;
        }
    }
}
