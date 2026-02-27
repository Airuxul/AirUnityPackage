using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

namespace TimelineExporter
{
    /// <summary>
    /// Clip-level context combining playable timing and clip/track data. Replaces object playerData.
    /// Null for graph-level callbacks (OnGraphStart/OnGraphStop).
    /// </summary>
    public interface IClipContext
    {
        IPlayableHandle Playable { get; }
        IFrameData FrameData { get; }
        TimelineClipData Clip { get; }
        TimelineTrackData Track { get; }
        IReadOnlyDictionary<string, GameObject> Bindings { get; }
        IReadOnlyDictionary<string, TimelinePlayer> SubTimelinePlayers { get; }
        Transform BindingRoot { get; }
        TimelineData TimelineData { get; }

        /// <summary>
        /// Cached TimelineAsset for sub-asset extraction (e.g. AnimationClip from Timeline).
        /// </summary>
        TimelineAsset CachedTimelineAsset { get; }

        /// <summary>
        /// Registers a binding for runtime-created objects (e.g. instantiated prefabs).
        /// </summary>
        bool TryRegisterBinding(string key, GameObject target);
    }
}
