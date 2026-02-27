using UnityEngine.Playables;

namespace TimelineExporter
{
    /// <summary>
    /// Base runtime behaviour. Mirrors PlayableBehaviour; override any method for shared logic (Timeline + simulation).
    /// </summary>
    public class BaseRuntimeBehaviour : IPlayableBehaviour, ISimulatedPlayableBehaviour
    {
        public virtual void OnGraphStart(Playable playable) => OnGraphStart(new PlayableContextAdapter(playable, default), null);
        public virtual void OnGraphStop(Playable playable) => OnGraphStop(new PlayableContextAdapter(playable, default), null);
        public virtual void OnPlayableCreate(Playable playable) { }
        public virtual void OnPlayableDestroy(Playable playable) { }

        public virtual void OnBehaviourPlay(Playable playable, FrameData info) => OnBehaviourPlay(new PlayableContextAdapter(playable, info), null);
        public virtual void OnBehaviourPause(Playable playable, FrameData info) => OnBehaviourPause(new PlayableContextAdapter(playable, info), null);
        public virtual void PrepareFrame(Playable playable, FrameData info) => PrepareFrame(new PlayableContextAdapter(playable, info), null);
        public virtual void ProcessFrame(Playable playable, FrameData info, object playerData) => ProcessFrame(new PlayableContextAdapter(playable, info), null);

        protected virtual void OnGraphStart(IPlayableContext context, IClipContext clipContext) { }
        protected virtual void OnGraphStop(IPlayableContext context, IClipContext clipContext) { }
        protected virtual void OnBehaviourPlay(IPlayableContext context, IClipContext clipContext) { }
        protected virtual void OnBehaviourPause(IPlayableContext context, IClipContext clipContext) { }
        protected virtual void PrepareFrame(IPlayableContext context, IClipContext clipContext) { }
        protected virtual void ProcessFrame(IPlayableContext context, IClipContext clipContext) { }

        void ISimulatedPlayableBehaviour.OnGraphStart(IPlayableContext c, IClipContext p) => OnGraphStart(c, p);
        void ISimulatedPlayableBehaviour.OnGraphStop(IPlayableContext c, IClipContext p) => OnGraphStop(c, p);
        void ISimulatedPlayableBehaviour.OnBehaviourPlay(IPlayableContext c, IClipContext p) => OnBehaviourPlay(c, p);
        void ISimulatedPlayableBehaviour.OnBehaviourPause(IPlayableContext c, IClipContext p) => OnBehaviourPause(c, p);
        void ISimulatedPlayableBehaviour.PrepareFrame(IPlayableContext c, IClipContext p) => PrepareFrame(c, p);
        void ISimulatedPlayableBehaviour.ProcessFrame(IPlayableContext c, IClipContext p) => ProcessFrame(c, p);
    }
}