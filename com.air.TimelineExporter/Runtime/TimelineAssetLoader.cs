using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;

namespace TimelineExporter
{
    /// <summary>
    /// Loads TimelineAsset from path. Editor registers AssetDatabase loader; runtime uses Resources when in Resources folder.
    /// </summary>
    public static class TimelineAssetLoader
    {
        /// <summary>
        /// Editor-only: loads from full asset path. Set by TimelineExporter.Editor.
        /// </summary>
        public static System.Func<string, TimelineAsset> LoadFromAssetPath { get; set; }

        /// <summary>
        /// Loads TimelineAsset from TimelineData paths. Tries Resources first, then Editor loader.
        /// </summary>
        public static TimelineAsset Load(TimelineData data)
        {
            if (data == null) return null;

            if (!string.IsNullOrEmpty(data.TimelineResourcePath))
            {
                var asset = Resources.Load<TimelineAsset>(data.TimelineResourcePath);
                if (asset != null) return asset;
            }

            if (!string.IsNullOrEmpty(data.TimelineAssetPath) && LoadFromAssetPath != null)
                return LoadFromAssetPath(data.TimelineAssetPath);

            return null;
        }

        /// <summary>
        /// Extracts AnimationClip from loaded TimelineAsset by track and clip index.
        /// </summary>
        public static AnimationClip GetAnimationClipFromTimeline(TimelineAsset timeline, int trackBindingIndex, int clipIndex)
        {
            if (timeline == null || trackBindingIndex < 0 || clipIndex < 0) return null;

            var tracks = timeline.GetOutputTracks().ToArray();
            if (trackBindingIndex >= tracks.Length) return null;

            var track = tracks[trackBindingIndex];
            var clips = track.GetClips().ToArray();
            if (clipIndex >= clips.Length) return null;

            var timelineClip = clips[clipIndex];
            var animAsset = timelineClip?.asset as AnimationPlayableAsset;
            return animAsset?.clip;
        }
    }
}
