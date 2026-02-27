using UnityEngine;

namespace TimelineExporter
{
    /// <summary>
    /// Computes blend weight for Timeline clips. Mirrors Unity Timeline's EvaluateMixIn * EvaluateMixOut.
    /// Uses default EaseInOut curves when blend durations are set.
    /// </summary>
    public static class BlendWeightUtility
    {
        private static readonly AnimationCurve DefaultMixIn = AnimationCurve.EaseInOut(0, 0, 1, 1);
        private static readonly AnimationCurve DefaultMixOut = AnimationCurve.EaseInOut(0, 1, 1, 0);

        /// <summary>
        /// Evaluates blend weight at timeline time. Weight = mixIn * mixOut (0..1).
        /// </summary>
        /// <param name="clip">Clip data with StartTime, Duration, MixInDuration, MixOutDuration.</param>
        /// <param name="timelineTime">Current time on the timeline.</param>
        public static float EvaluateWeight(TimelineClipData clip, double timelineTime)
        {
            float mixIn = EvaluateMixIn(clip, timelineTime);
            float mixOut = EvaluateMixOut(clip, timelineTime);
            return mixIn * mixOut;
        }

        private static float EvaluateMixIn(TimelineClipData clip, double timelineTime)
        {
            if (clip.MixInDuration <= 1e-6) return 1f;
            double mixInTime = clip.StartTime;
            double mixInEnd = clip.StartTime + clip.MixInDuration;
            if (timelineTime >= mixInEnd) return 1f;
            float perc = (float)((timelineTime - mixInTime) / clip.MixInDuration);
            return Mathf.Clamp01(DefaultMixIn.Evaluate(perc));
        }

        private static float EvaluateMixOut(TimelineClipData clip, double timelineTime)
        {
            if (clip.MixOutDuration <= 1e-6) return 1f;
            double mixOutStart = clip.StartTime + clip.Duration - clip.MixOutDuration;
            if (timelineTime <= mixOutStart) return 1f;
            float perc = (float)((timelineTime - mixOutStart) / clip.MixOutDuration);
            return Mathf.Clamp01(DefaultMixOut.Evaluate(perc));
        }
    }
}
