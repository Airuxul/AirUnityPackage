namespace TimelineExporter
{
    /// <summary>
    /// Mixer for track-level blend (e.g. AnimationTrack, future AudioTrack). ProcessFrame receives context per frame.
    /// </summary>
    public interface ITrackMixer
    {
        void ProcessFrame(IPlayableContext context, IClipContext clipContext);
        void Destroy();
    }
}
