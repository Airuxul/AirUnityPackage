using UnityEngine;
using UnityEngine.Playables;

namespace TimelineKit
{
    /// <summary>
    /// Non-generic base clip used as a type constraint in TrackEx&lt;TClip, TMixer&gt;.
    /// For concrete clip assets prefer inheriting ClipEx&lt;TBehaviour&gt;.
    /// </summary>
    public abstract class ClipEx : PlayableAsset
    {
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
            => Playable.Create(graph);
    }

    /// <summary>
    /// Generic base clip that auto-creates a ScriptPlayable&lt;TBehaviour&gt; and calls SetupBehaviour.
    /// Subclasses only need to override SetupBehaviour to push serialized fields onto the behaviour.
    /// CreatePlayable is sealed to guarantee SetupBehaviour is always called.
    /// </summary>
    /// <typeparam name="TBehaviour">The PlayableBehaviourEx type this clip creates.</typeparam>
    public abstract class ClipEx<TBehaviour> : ClipEx
        where TBehaviour : PlayableBehaviourEx, new()
    {
        public sealed override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<TBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();
            behaviour.Owner = owner;
            SetupBehaviour(behaviour, owner);
            return playable;
        }

        /// <summary>
        /// Transfer serialized clip fields onto the behaviour before the playable enters the graph.
        /// <paramref name="owner"/> is the GameObject that owns the PlayableDirector — use it to
        /// retrieve components such as PlayableDirectorEx for asset preloading.
        /// Called once per playable creation.
        /// </summary>
        protected virtual void SetupBehaviour(TBehaviour behaviour, GameObject owner) { }
    }
}
