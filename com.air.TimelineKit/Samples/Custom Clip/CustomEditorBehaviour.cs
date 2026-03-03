#if UNITY_EDITOR
using TimelineKit;
using UnityEngine.Playables;

/// <summary>
/// Editor preview behaviour for CustomClip.
/// Override Editor_ProcessFrame to show a preview state while scrubbing the Timeline.
/// </summary>
public class CustomEditorBehaviour : IEditorBehaviour
{
    public void Editor_OnBehaviourPlay(Playable playable, FrameData info) { }
    public void Editor_OnBehaviourPause(Playable playable, FrameData info) { }
    public void Editor_PrepareFrame(Playable playable, FrameData info) { }
    public void Editor_ProcessFrame(Playable playable, FrameData info, object playerData) { }
}
#endif
