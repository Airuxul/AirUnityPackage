using System.Collections.Generic;
using UnityEngine;

namespace TimelineExporter
{
    /// <summary>
    /// Mixer for animation track blend. Uses AnimationBlendUtility (no Playables) for true multi-clip blend.
    /// </summary>
    public class AnimationMixBehaviour : BaseRuntimeBehaviour, ITrackMixer
    {
        private Animator controlledAnimator;

        public bool IsValid => true;

        void ITrackMixer.ProcessFrame(IPlayableContext context, IClipContext clipContext) => ProcessFrame(context, clipContext);

        protected override void ProcessFrame(IPlayableContext context, IClipContext clipContext)
        {
            if (clipContext is not IAnimationTrackMixContext mixContext ||
                mixContext.TargetAnimator == null)
                return;

            var mixClips = mixContext.MixClips;
            if (mixClips == null || mixClips.Count == 0)
                return;

            var clips = new List<(AnimationClip clip, float weight, float sampleTime, AnimationClipData animData)>();
            foreach (var info in mixClips)
            {
                if (info?.ResolvedClip == null || info.AnimData == null || info.ResolvedClip.legacy || info.Weight <= 0)
                    continue;

                var sampleTime = (float)(info.LocalTime * info.TimeScale + info.ClipIn);
                var clipLength = info.ResolvedClip.length;
                if (clipLength > 0)
                {
                    var loop = info.AnimData.loop == 1 || (info.AnimData.loop == 0 && info.ResolvedClip.isLooping);
                    sampleTime = loop ? Mathf.Repeat(sampleTime, clipLength) : Mathf.Clamp(sampleTime, 0, clipLength);
                }

                clips.Add((info.ResolvedClip, info.Weight, sampleTime, info.AnimData));
            }

            if (clips.Count == 0)
                return;

            controlledAnimator = mixContext.TargetAnimator;
            AnimationBlendUtility.SampleAndBlend(mixContext.TargetAnimator.gameObject, clips);
        }

        public void Destroy()
        {
            // Do not re-enable animator. Keep last pose to avoid default state (e.g. MoveY) playing.
            controlledAnimator = null;
        }
    }
}
