namespace TimelineExporter
{
    /// <summary>
    /// Simulation equivalent of PlayableBehaviour. Full method set mirroring PlayableBehaviour.
    /// clipContext: null for graph-level; IClipContext for clip-level.
    /// </summary>
    public interface ISimulatedPlayableBehaviour
    {
        void OnGraphStart(IPlayableContext context, IClipContext clipContext);
        void OnGraphStop(IPlayableContext context, IClipContext clipContext);
        void OnBehaviourPlay(IPlayableContext context, IClipContext clipContext);
        void OnBehaviourPause(IPlayableContext context, IClipContext clipContext);
        void PrepareFrame(IPlayableContext context, IClipContext clipContext);
        void ProcessFrame(IPlayableContext context, IClipContext clipContext);
    }
}
