using UnityEngine;

namespace TimelineExporter
{
    /// <summary>
    /// Simulates AnimationTrack for TimelinePlayer. Samples AnimationClip at local time on bound Animator/GameObject.
    /// </summary>
    public class AnimationBehaviour : ISimulatedPlayableBehaviour
    {
        public void OnGraphStart(IPlayableContext context, IClipContext clipContext) { }
        public void OnGraphStop(IPlayableContext context, IClipContext clipContext) { }
        public void PrepareFrame(IPlayableContext context, IClipContext clipContext) { }

        public void OnBehaviourPlay(IPlayableContext context, IClipContext clipCtx) { }

        public void OnBehaviourPause(IPlayableContext context, IClipContext clipCtx) { }

        public void ProcessFrame(IPlayableContext context, IClipContext clipCtx)
        {
            if (clipCtx == null) return;

            var animData = ClipContextResolver.ParseCustomData<AnimationClipData>(clipCtx.Clip);
            if (animData == null) return;

            var target = ClipContextResolver.ResolveGameObject(clipCtx, animData.trackName, clipCtx.Clip?.DisplayName);
            if (target == null) return;

            var clip = ClipContextResolver.ResolveAnimationClip(clipCtx.TimelineData, animData, clipCtx.CachedTimelineAsset);
            if (clip == null || clip.legacy) return;

            float localTime = (float)context.LocalTime;
            if (clipCtx.Clip == null) return;
            float timeScale = (float)clipCtx.Clip.TimeScale;
            float clipIn = (float)clipCtx.Clip.ClipIn;
            float sampleTime = localTime * timeScale + clipIn;
            float clipLength = clip.length;
            if (clipLength <= 0) return;

            bool loop = animData.loop == 1 || (animData.loop == 0 && clip.isLooping);
            sampleTime = loop ? Mathf.Repeat(sampleTime, clipLength) : Mathf.Clamp(sampleTime, 0, clipLength);

            clip.SampleAnimation(target, sampleTime);
        }
    }
}
