namespace TimelineExporter
{
    /// <summary>
    /// Abstraction of FrameData. Mirrors Playables.FrameData properties.
    /// </summary>
    public interface IFrameData
    {
        float DeltaTime { get; }
        float EffectiveWeight { get; }
        float EffectiveSpeed { get; }
        float Weight { get; }
    }
}
