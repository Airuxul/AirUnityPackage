using UnityEngine.Playables;

namespace TimelineExporter.Editor
{
    /// <summary>
    /// Editor preview behaviour interface. Used only when Timeline plays in Editor.
    /// </summary>
    public interface IEditorBehaviour
    {
        void Editor_OnGraphStart(Playable playable);
        void Editor_OnGraphStop(Playable playable);
        void Editor_OnPlayableCreate(Playable playable);
        void Editor_OnPlayableDestroy(Playable playable);
        void Editor_OnBehaviourPlay(Playable playable, FrameData info);
        void Editor_OnBehaviourPause(Playable playable, FrameData info);
        void Editor_PrepareFrame(Playable playable, FrameData info);
        void Editor_ProcessFrame(Playable playable, FrameData info, object playerData);
    }
}
