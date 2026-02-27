using UnityEngine.Playables;

namespace TimelineExporter
{
    /// <summary>
    /// Adapts Unity Playable to IPlayableHandle.
    /// </summary>
    public readonly struct PlayableHandleAdapter : IPlayableHandle
    {
        private readonly Playable _playable;

        public PlayableHandleAdapter(Playable playable) => _playable = playable;

        public double GetTime() => _playable.GetTime();
        public double GetDuration() => _playable.GetDuration();
    }
}
