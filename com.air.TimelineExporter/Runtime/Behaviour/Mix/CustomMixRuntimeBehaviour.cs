namespace TimelineExporter
{
    /// <summary>
    /// Runtime behaviour for CustomTrack mixer. Blends CustomClip inputs by weight.
    /// Override ProcessFrame for custom blend logic. Mirrors CustomRuntimeBehaviour pattern.
    /// </summary>
    public class CustomMixRuntimeBehaviour : BaseRuntimeBehaviour
    {
        protected override void ProcessFrame(IPlayableContext context, IClipContext clipContext)
        {
            
        }
    }
}
