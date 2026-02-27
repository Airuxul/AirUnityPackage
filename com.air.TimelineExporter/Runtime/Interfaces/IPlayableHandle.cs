namespace TimelineExporter
{
    /// <summary>
    /// Abstraction of Playable for timing. Mirrors Playable.GetTime/GetDuration.
    /// </summary>
    public interface IPlayableHandle
    {
        double GetTime();
        double GetDuration();
    }
}
