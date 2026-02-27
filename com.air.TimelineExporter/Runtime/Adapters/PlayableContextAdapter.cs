using UnityEngine.Playables;

namespace TimelineExporter
{
    /// <summary>
    /// Adapts Playable + FrameData to IPlayableContext for reusing logic in Timeline path.
    /// </summary>
    public readonly struct PlayableContextAdapter : IPlayableContext
    {
        private readonly Playable _playable;
        private readonly FrameData _frameData;

        public PlayableContextAdapter(Playable playable, FrameData frameData)
        {
            _playable = playable;
            _frameData = frameData;
        }

        public double LocalTime => _playable.GetTime();
        public double Duration => _playable.GetDuration();
        public double GetTime() => _playable.GetTime();
        public double GetDuration() => _playable.GetDuration();
        public float DeltaTime => _frameData.deltaTime;
        public float EffectiveWeight => _frameData.effectiveWeight;
        public float EffectiveSpeed => _frameData.effectiveSpeed;
        public float Weight => _frameData.weight;
    }
}
