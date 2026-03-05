using UnityEngine.Playables;

namespace TimelineKit
{
    /// <summary>
    /// Implement to provide editor-only preview logic for a custom clip behaviour.
    /// Assign an instance to PlayableBehaviourEx._editorBehaviour in the subclass constructor.
    /// When Unity is in edit mode (Timeline scrubbing), all PlayableBehaviour callbacks are
    /// routed here instead of executing runtime logic.
    /// </summary>
    public interface IEditorBehaviour
    {
        void Editor_OnBehaviourPlay(Playable playable, FrameData info);
        void Editor_OnBehaviourPause(Playable playable, FrameData info);
        void Editor_PrepareFrame(Playable playable, FrameData info);
        void Editor_ProcessFrame(Playable playable, FrameData info, object playerData);
    }
}
