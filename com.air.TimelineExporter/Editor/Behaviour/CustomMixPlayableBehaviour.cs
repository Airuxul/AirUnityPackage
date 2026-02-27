namespace TimelineExporter.Editor
{
    /// <summary>
    /// Editor-only PlayableBehaviour for CustomTrack mixer. Runtime uses CustomMixRuntimeBehaviour via TimelinePlayer.
    /// </summary>
    public class CustomMixPlayableBehaviour : PlayableBehaviourEx
    {
        public CustomMixPlayableBehaviour()
        {
            _editorBehaviour = new CustomMixEditorBehaviour();
            _playableBehaviour = new CustomMixRuntimeBehaviour();
        }
    }
}
