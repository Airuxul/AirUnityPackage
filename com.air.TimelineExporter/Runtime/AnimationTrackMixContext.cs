using System.Collections.Generic;
using UnityEngine;

namespace TimelineExporter
{
    /// <summary>
    /// IAnimationTrackMixContext implementation for animation track blend.
    /// </summary>
    public class AnimationTrackMixContext : ClipPlayableContext, IAnimationTrackMixContext
    {
        public Animator TargetAnimator { get; set; }
        public IReadOnlyList<AnimationMixClipInfo> MixClips { get; set; }
        public int TrackClipCount { get; set; }
    }
}
