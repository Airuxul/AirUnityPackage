namespace TimelineExporter.Editor
{
    /// <summary>
    /// Editor-only PlayableBehaviour for CustomClip. Runtime uses CustomRuntimeBehaviour via TimelinePlayer.
    /// </summary>
    public class CustomPlayableBehaviour : PlayableBehaviourEx
    {
        public CustomPlayableBehaviour()
        {
            _editorBehaviour = new CustomEditorBehaviour();
            _playableBehaviour = new CustomRuntimeBehaviour();
        }
    }
}
