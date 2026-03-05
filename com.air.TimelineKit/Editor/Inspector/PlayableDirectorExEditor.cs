using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

namespace TimelineKit.Editor
{
    [CustomEditor(typeof(PlayableDirectorEx))]
    public class PlayableDirectorExEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var player = (PlayableDirectorEx)target;
            var director = player.GetComponent<UnityEngine.Playables.PlayableDirector>();
            var timeline = director != null ? director.playableAsset as TimelineAsset : null;

            EditorGUILayout.Space(4);

            using (new EditorGUI.DisabledScope(timeline == null))
            {
                var label = player.Manifest != null
                    ? "Update Reference Manifest"
                    : "Export Reference Manifest";

                if (GUILayout.Button(label))
                    ExportOrUpdate(player, timeline);
            }

            if (timeline == null)
                EditorGUILayout.HelpBox("Assign a Timeline asset to PlayableDirector to enable manifest export.", MessageType.Info);
        }

        private void ExportOrUpdate(PlayableDirectorEx player, TimelineAsset timeline)
        {
            var newManifest = TimelineReferenceExporter.Export(timeline, player.gameObject);

            if (player.Manifest != null)
            {
                // Overwrite the existing manifest asset in place.
                var existing = player.Manifest;
                existing.timelineGuid = newManifest.timelineGuid;
                existing.timelinePath = newManifest.timelinePath;
                existing.assetReferences = newManifest.assetReferences;
                EditorUtility.SetDirty(existing);
                AssetDatabase.SaveAssets();

                var path = AssetDatabase.GetAssetPath(existing);
                Debug.Log($"[Timeline Kit] Manifest updated: {path} ({existing.assetReferences.Count} references)");
                EditorGUIUtility.PingObject(existing);
            }
            else
            {
                // Create a new manifest asset next to the timeline.
                var timelinePath = AssetDatabase.GetAssetPath(timeline);
                var dir = Path.GetDirectoryName(timelinePath) ?? "Assets";
                var name = Path.GetFileNameWithoutExtension(timelinePath) + "_References.asset";
                var savePath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(dir, name).Replace('\\', '/'));

                AssetDatabase.CreateAsset(newManifest, savePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                // Assign the new manifest back to the component.
                var so = new SerializedObject(player);
                so.FindProperty("_manifest").objectReferenceValue = newManifest;
                so.ApplyModifiedProperties();

                Debug.Log($"[Timeline Kit] Manifest created: {savePath} ({newManifest.assetReferences.Count} references)");
                EditorGUIUtility.PingObject(newManifest);
            }
        }
    }
}
