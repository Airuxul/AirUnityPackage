using UnityEngine;
using UnityEngine.Playables;

namespace TimelineExporter.Editor
{
    /// <summary>
    /// Bridges PlayableBehaviour to IPlayableBehaviour / IEditorBehaviour for Timeline preview in Editor.
    /// </summary>
    public class PlayableBehaviourEx : PlayableBehaviour
    {
        protected IPlayableBehaviour _playableBehaviour;
        protected IEditorBehaviour _editorBehaviour;

        public override void OnGraphStart(Playable playable)
        {
            if (!Application.isPlaying && _editorBehaviour != null)
            {
                _editorBehaviour.Editor_OnGraphStart(playable);
                return;
            }
            _playableBehaviour?.OnGraphStart(playable);
        }

        public override void OnGraphStop(Playable playable)
        {
            if (!Application.isPlaying && _editorBehaviour != null)
            {
                _editorBehaviour.Editor_OnGraphStop(playable);
                return;
            }
            _playableBehaviour?.OnGraphStop(playable);
        }

        public override void OnPlayableCreate(Playable playable)
        {
            if (!Application.isPlaying && _editorBehaviour != null)
            {
                _editorBehaviour.Editor_OnPlayableCreate(playable);
                return;
            }
            _playableBehaviour?.OnPlayableCreate(playable);
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            if (!Application.isPlaying && _editorBehaviour != null)
            {
                _editorBehaviour.Editor_OnPlayableDestroy(playable);
                return;
            }
            _playableBehaviour?.OnPlayableDestroy(playable);
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (!Application.isPlaying && _editorBehaviour != null)
            {
                _editorBehaviour.Editor_OnBehaviourPlay(playable, info);
                return;
            }
            _playableBehaviour?.OnBehaviourPlay(playable, info);
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (!Application.isPlaying && _editorBehaviour != null)
            {
                _editorBehaviour.Editor_OnBehaviourPause(playable, info);
                return;
            }
            _playableBehaviour?.OnBehaviourPause(playable, info);
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            if (!Application.isPlaying && _editorBehaviour != null)
            {
                _editorBehaviour.Editor_PrepareFrame(playable, info);
                return;
            }
            _playableBehaviour?.PrepareFrame(playable, info);
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (!Application.isPlaying && _editorBehaviour != null)
            {
                _editorBehaviour.Editor_ProcessFrame(playable, info, playerData);
                return;
            }
            _playableBehaviour?.ProcessFrame(playable, info, playerData);
        }
    }
}
