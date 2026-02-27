using UnityEngine;

namespace TimelineExporter
{
    /// <summary>
    /// Per-clip info for animation track mixer. Provides data required by ProcessFrame.
    /// </summary>
    public class AnimationMixClipInfo
    {
        public TimelineClipData Clip { get; set; }
        public float Weight { get; set; }
        public double LocalTime { get; set; }
        public double ClipIn { get; set; }
        public double TimeScale { get; set; }
        public int Index { get; set; }
        public AnimationClipData AnimData { get; set; }
        public AnimationClip ResolvedClip { get; set; }
    }
}
