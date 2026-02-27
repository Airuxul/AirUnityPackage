namespace TimelineExporter
{
    /// <summary>
    /// IPlayableContext implementation for simulation playback.
    /// </summary>
    public class SimulatedPlayableContext : IPlayableContext
    {
        public double LocalTime { get; set; }
        public double Duration { get; set; }
        public float DeltaTime { get; set; }
        public float EffectiveWeight { get; set; } = 1f;
        public float EffectiveSpeed { get; set; } = 1f;
        public float Weight { get; set; } = 1f;

        public double GetTime() => LocalTime;
        public double GetDuration() => Duration;
    }
}
