using System.Collections.Generic;

namespace TimelineExporter
{
    /// <summary>
    /// Base for track blend handlers. Common flow: 1 clip -> TryProcessFrame; 2+ clips -> ProcessMix.
    /// Subclass and override ProcessMix for clip-type-specific blend logic.
    /// </summary>
    public abstract class BaseTrackBlendHandler : ITrackBlendHandler
    {
        public abstract string ClipType { get; }

        public virtual bool CanHandle(TimelineTrackData track)
        {
            if (track?.Clips == null) return false;
            foreach (var clip in track.Clips)
            {
                if (clip?.ClipType == ClipType) return true;
            }
            return false;
        }

        public void Process(TimelinePlayer player, TimelineTrackData track, List<TimelineClipData> toExit)
        {
            if (player == null || track == null) return;

            var active = CollectActiveClips(player, track, toExit);
            if (active.Count == 0)
            {
                OnTrackIdle(player, track);
                return;
            }

            ProcessMix(player, track, active);
        }

        /// <summary>
        /// Called when track has no active clips. Override to reset animator or other per-track state.
        /// </summary>
        protected virtual void OnTrackIdle(TimelinePlayer player, TimelineTrackData track) { }

        /// <summary>
        /// Override for clip-type-specific blend logic when 2+ clips overlap.
        /// </summary>
        protected abstract void ProcessMix(TimelinePlayer player, TimelineTrackData track,
            List<(TimelineClipData clip, float weight)> active);

        protected static List<(TimelineClipData clip, float weight)> CollectActiveClips(
            TimelinePlayer player, TimelineTrackData track, List<TimelineClipData> toExit)
        {
            var active = new List<(TimelineClipData clip, float weight)>();
            var currentTime = player.CurrentTime;

            for (int i = 0; i < track.Clips.Count; i++)
            {
                var clip = track.Clips[i];
                var inRange = currentTime >= clip.StartTime && currentTime < clip.EndTime;
                var wasActive = player.IsClipActive(clip.Id);

                if (inRange && !wasActive) player.TryEnterClip(clip, track);
                else if (!inRange && wasActive) toExit.Add(clip);

                if (inRange)
                {
                    float weight = BlendWeightUtility.EvaluateWeight(clip, currentTime);
                    active.Add((clip, weight));
                }
            }
            return active;
        }

        protected static IClipContext CreateClipContext(TimelinePlayer player, TimelineClipData clip, TimelineTrackData track)
        {
            return new ClipPlayableContext
            {
                Context = player.GetContext(),
                Clip = clip,
                Track = track,
                TimelineData = player.Data,
                Bindings = player.Bindings,
                SubTimelinePlayers = player.SubTimelinePlayers,
                BindingRoot = player.BindingRoot,
                CachedTimelineAsset = player.GetCachedTimelineAsset()
            };
        }
    }
}
