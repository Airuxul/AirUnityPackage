using System.Collections.Generic;

namespace TimelineExporter
{
    /// <summary>
    /// Handles track-level blend for a clip type. Builds context and invokes ITrackMixer.ProcessFrame.
    /// </summary>
    public interface ITrackBlendHandler
    {
        /// <summary>
        /// Clip type this handler supports (e.g. "AnimationPlayableAsset").
        /// </summary>
        string ClipType { get; }

        /// <summary>
        /// Returns true if track has clips of this type.
        /// </summary>
        bool CanHandle(TimelineTrackData track);

        /// <summary>
        /// Process track blend. Updates toExit, gets/creates mixer, builds context, calls ProcessFrame.
        /// </summary>
        void Process(TimelinePlayer player, TimelineTrackData track, List<TimelineClipData> toExit);
    }
}
