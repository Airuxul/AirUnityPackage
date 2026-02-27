namespace TimelineExporter
{
    /// <summary>
    /// Runtime behaviour for CustomClip. Mirrors CustomMixRuntimeBehaviour pattern.
    /// Override ProcessFrame for per-frame logic. Reused in both Timeline and simulation.
    /// </summary>
    public class CustomRuntimeBehaviour : BaseRuntimeBehaviour
    {
        protected override void ProcessFrame(IPlayableContext context, IClipContext clipContext)
        {
        }
    }
}