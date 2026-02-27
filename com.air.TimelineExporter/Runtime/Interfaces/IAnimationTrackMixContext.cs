using System.Collections.Generic;

namespace TimelineExporter
{
    /// <summary>
    /// Extended IClipContext for animation track mixer. Provides Animator and per-clip mix data.
    /// </summary>
    public interface IAnimationTrackMixContext : IClipContext
    {
        /// <summary>
        /// Resolved Animator from track binding. Null when binding has no Animator.
        /// </summary>
        UnityEngine.Animator TargetAnimator { get; }

        /// <summary>
        /// Active clips with blend weights and timing. Empty when no clips in range.
        /// </summary>
        IReadOnlyList<AnimationMixClipInfo> MixClips { get; }

        /// <summary>
        /// Total clip count in track for mixer setup.
        /// </summary>
        int TrackClipCount { get; }
    }
}
