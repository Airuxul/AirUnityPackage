namespace TimelineExporter
{
    /// <summary>
    /// Combined IPlayableHandle + IFrameData for simulation.
    /// </summary>
    public interface IPlayableContext : IPlayableHandle, IFrameData
    {
        double LocalTime { get; }
        double Duration { get; }
    }
}
