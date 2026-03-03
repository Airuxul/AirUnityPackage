using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TimelineKit
{
    /// <summary>
    /// Generic base track that auto-creates a ScriptPlayable&lt;TMixer&gt; for the track mixer.
    /// Subclasses need only declare the type parameters and add [TrackClipType].
    /// No code body is required in the typical case.
    /// </summary>
    /// <typeparam name="TClip">The ClipEx-derived clip asset type this track accepts.</typeparam>
    /// <typeparam name="TMixer">The PlayableBehaviourEx-derived mixer behaviour type.</typeparam>
    [HideInMenu]
    public abstract class TrackEx<TClip, TMixer> : TrackAsset
        where TClip : ClipEx
        where TMixer : PlayableBehaviourEx, new()
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
            => ScriptPlayable<TMixer>.Create(graph, inputCount);
    }
}
