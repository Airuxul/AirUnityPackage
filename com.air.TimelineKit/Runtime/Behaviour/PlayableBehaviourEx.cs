using UnityEngine;
using UnityEngine.Playables;

namespace TimelineKit
{
    /// <summary>
    /// Base class for custom clip and mixer PlayableBehaviours in Timeline Kit.
    ///
    /// Owner: Set by ClipEx when the playable is created (the GameObject that owns the
    /// PlayableDirector). Use GetDirectorEx() to access preloaded assets at runtime.
    ///
    /// Editor/runtime separation:
    ///   Assign _editorBehaviour in your constructor. While Unity is in edit mode
    ///   (Timeline scrubbing) each callback automatically routes to _editorBehaviour.Editor_*
    ///   and returns early, so runtime logic is never executed in edit mode.
    ///   Use IsEditorPreview to branch if you need finer control inside an override.
    /// </summary>
    public abstract class PlayableBehaviourEx : PlayableBehaviour
    {
        /// <summary>
        /// The GameObject that owns the PlayableDirector. Set by ClipEx before SetupBehaviour.
        /// Use GetDirectorEx() to retrieve the PlayableDirectorEx component for preloaded assets.
        /// </summary>
        public GameObject Owner { get; set; }

        /// <summary>
        /// Returns the PlayableDirectorEx on Owner, or null if not present.
        /// Use this in runtime callbacks to get preloaded assets via GetLoadedAsset().
        /// </summary>
        protected PlayableDirectorEx GetDirectorEx()
            => Owner != null ? Owner.GetComponent<PlayableDirectorEx>() : null;

#if UNITY_EDITOR
        /// <summary>
        /// Assign in the subclass constructor to enable editor preview separation.
        /// </summary>
        protected IEditorBehaviour _editorBehaviour;

        /// <summary>
        /// True when running in edit mode and _editorBehaviour is set.
        /// </summary>
        protected bool IsEditorPreview =>
            !Application.isPlaying && _editorBehaviour != null;
#endif

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
#if UNITY_EDITOR
            if (IsEditorPreview) { _editorBehaviour.Editor_OnBehaviourPlay(playable, info); return; }
#endif
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
#if UNITY_EDITOR
            if (IsEditorPreview) { _editorBehaviour.Editor_OnBehaviourPause(playable, info); return; }
#endif
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
#if UNITY_EDITOR
            if (IsEditorPreview) { _editorBehaviour.Editor_PrepareFrame(playable, info); return; }
#endif
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
#if UNITY_EDITOR
            if (IsEditorPreview) { _editorBehaviour.Editor_ProcessFrame(playable, info, playerData); return; }
#endif
        }
    }
}
