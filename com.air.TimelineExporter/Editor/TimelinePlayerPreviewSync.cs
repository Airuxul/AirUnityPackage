using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;

namespace TimelineExporter.Editor
{
    /// <summary>
    /// Syncs Timeline window with TimelinePlayerComponent during Play mode.
    /// When the component's GameObject is selected, the Timeline window shows the preview timeline
    /// and playhead follows TimelinePlayer (read-only: dragging is overwritten next frame).
    /// </summary>
    [InitializeOnLoad]
    public static class TimelinePlayerPreviewSync
    {
        static TimelinePlayerPreviewSync()
        {
            EditorApplication.update += OnEditorUpdate;
        }

        private static void OnEditorUpdate()
        {
            if (!Application.isPlaying) return;

            var go = Selection.activeGameObject;
            if (go == null) return;

            var comp = go.GetComponent<TimelinePlayerComponent>();
            if (comp == null || comp.Player == null || comp.PreviewDirector == null) return;

            var window = TimelineEditor.GetWindow();
            if (window == null) return;

            var state = typeof(TimelineEditor).GetProperty("state",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)?.GetValue(null);
            if (state == null) return;

            var editSequence = state.GetType().GetProperty("editSequence")?.GetValue(state);
            if (editSequence == null) return;

            var editAsset = editSequence.GetType().GetProperty("asset")?.GetValue(editSequence) as UnityEngine.Timeline.TimelineAsset;
            var previewAsset = comp.ResolvedPreviewAsset;
            if (editAsset != previewAsset)
            {
                window.SetTimeline(comp.PreviewDirector);
            }

            var editSequenceTime = editSequence.GetType().GetProperty("time");
            if (editSequenceTime != null)
            {
                var currentTime = comp.Player.CurrentTime;
                editSequenceTime.SetValue(editSequence, currentTime);
            }

            window.Repaint();
        }
    }
}
