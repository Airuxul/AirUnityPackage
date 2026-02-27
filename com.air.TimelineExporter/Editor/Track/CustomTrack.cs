using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TimelineExporter.Editor
{
    /// <summary>
    /// Track for Timeline editing. Uses CustomTrackMixerBehaviour for blend. Runtime also uses TimelinePlayer.
    /// </summary>
    [TrackClipType(typeof(CustomClip))]
    public class CustomTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<CustomMixPlayableBehaviour>.Create(graph, inputCount);
        }
    }
}
