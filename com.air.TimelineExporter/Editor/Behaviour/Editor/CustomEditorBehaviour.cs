using UnityEngine.Playables;

namespace TimelineExporter.Editor
{
    /// <summary>
    /// Editor preview behaviour for CustomClip. Mirrors CustomMixEditorBehaviour pattern.
    /// </summary>
    public class CustomEditorBehaviour : IEditorBehaviour
    {
        public void Editor_OnGraphStart(Playable playable)
        {
        }

        public void Editor_OnGraphStop(Playable playable)
        {
        }

        public void Editor_OnPlayableCreate(Playable playable)
        {
        }

        public void Editor_OnPlayableDestroy(Playable playable)
        {
        }

        public void Editor_OnBehaviourPlay(Playable playable, FrameData info)
        {
        }

        public void Editor_OnBehaviourPause(Playable playable, FrameData info)
        {
        }

        public void Editor_PrepareFrame(Playable playable, FrameData info)
        {
        }

        public void Editor_ProcessFrame(Playable playable, FrameData info, object playerData)
        {
        }
    }
}