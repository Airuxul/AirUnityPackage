using System.Collections.Generic;
using UnityEngine;

namespace TimelineExporter
{
    /// <summary>
    /// Track blend handler for AnimationPlayableAsset. Extends BaseTrackBlendHandler.
    /// </summary>
    public class AnimationTrackBlendHandler : BaseTrackBlendHandler
    {
        public override string ClipType => "AnimationPlayableAsset";

        protected override void OnTrackIdle(TimelinePlayer player, TimelineTrackData track)
        {
            if (player == null || track == null || track.Clips == null || track.Clips.Count == 0) return;

            var clip = track.Clips[0];
            var animData = ClipContextResolver.ParseCustomData<AnimationClipData>(clip);
            var clipContext = CreateClipContext(player, clip, track);
            var target = ClipContextResolver.ResolveGameObject(clipContext,
                animData?.trackName ?? track.Name,
                track.Name,
                clip.DisplayName);
            if (target == null) return;

            var animator = target.GetComponent<Animator>();
            if (animator == null) return;

            animator.enabled = false;
            var mixerKey = track.Id ?? track.Name;
            if (!player.HasTrackMixer(mixerKey))
                player.RegisterAnimatorToRestoreOnStop(animator);
        }

        protected override void ProcessMix(TimelinePlayer player, TimelineTrackData track,
            List<(TimelineClipData clip, float weight)> active)
        {
            var data = player.Data;
            var cachedTimelineAsset = player.GetCachedTimelineAsset();
            if (data == null) return;

            var animData0 = ClipContextResolver.ParseCustomData<AnimationClipData>(active[0].clip);
            var clipContext = CreateClipContext(player, active[0].clip, track);
            var target = ClipContextResolver.ResolveGameObject(clipContext,
                animData0?.trackName ?? track.Name,
                track.Name,
                active[0].clip.DisplayName);
            if (target == null) return;

            var animator = target.GetComponent<Animator>();
            if (animator == null)
            {
                var best = active[0];
                foreach (var x in active)
                    if (x.weight > best.weight) best = x;
                player.TryProcessFrame(best.clip, track, 1f);
                return;
            }

            var mixClips = BuildMixClips(data, active, track, player.CurrentTime, cachedTimelineAsset);
            var mixContext = new AnimationTrackMixContext
            {
                Context = player.GetContext(),
                Clip = active[0].clip,
                Track = track,
                TimelineData = data,
                Bindings = player.Bindings,
                SubTimelinePlayers = player.SubTimelinePlayers,
                BindingRoot = player.BindingRoot,
                CachedTimelineAsset = cachedTimelineAsset,
                TargetAnimator = animator,
                MixClips = mixClips,
                TrackClipCount = track.Clips.Count
            };

            var mixer = player.GetOrCreateTrackMixer(track.Id ?? track.Name, () => new AnimationMixBehaviour());
            mixer.ProcessFrame(player.GetContext(), mixContext);
        }

        private static List<AnimationMixClipInfo> BuildMixClips(
            TimelineData data, List<(TimelineClipData clip, float weight)> active,
            TimelineTrackData track, double currentTime, UnityEngine.Timeline.TimelineAsset cachedTimelineAsset)
        {
            var mixClips = new List<AnimationMixClipInfo>();
            foreach (var (clip, weight) in active)
            {
                var animData = ClipContextResolver.ParseCustomData<AnimationClipData>(clip);
                if (animData == null) continue;

                var animClip = ClipContextResolver.ResolveAnimationClip(data, animData, cachedTimelineAsset);
                if (animClip == null || animClip.legacy) continue;

                mixClips.Add(new AnimationMixClipInfo
                {
                    Clip = clip,
                    Weight = weight,
                    LocalTime = currentTime - clip.StartTime,
                    ClipIn = clip.ClipIn,
                    TimeScale = clip.TimeScale,
                    Index = track.Clips.IndexOf(clip),
                    AnimData = animData,
                    ResolvedClip = animClip
                });
            }
            return mixClips;
        }
    }
}
