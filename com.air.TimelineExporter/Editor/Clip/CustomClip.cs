using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TimelineExporter.Editor
{
    /// <summary>
    /// Editor-only clip for Timeline editing. Runtime uses serialized data + TimelinePlayer.
    /// </summary>
    [Serializable]
    public class CustomClip : PlayableAsset, ITimelineClipAsset
    {
        public ClipCaps clipCaps => ClipCaps.Blending;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<CustomPlayableBehaviour>.Create(graph);
        }
    }
}
