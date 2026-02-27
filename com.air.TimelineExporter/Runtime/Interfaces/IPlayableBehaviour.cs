using UnityEngine.Playables;

namespace TimelineExporter
{
    /// <summary>
    /// Abstraction of PlayableBehaviour callbacks for both Timeline and simulation playback.
    /// Reuses same method signatures as PlayableBehaviour for minimal code.
    /// </summary>
    public interface IPlayableBehaviour
    {
        void OnGraphStart(Playable playable);
        void OnGraphStop(Playable playable);
        void OnPlayableCreate(Playable playable);
        void OnPlayableDestroy(Playable playable);
        void OnBehaviourPlay(Playable playable, FrameData info);
        void OnBehaviourPause(Playable playable, FrameData info);
        void PrepareFrame(Playable playable, FrameData info);
        void ProcessFrame(Playable playable, FrameData info, object playerData);
    }
}
