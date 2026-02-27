using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

namespace TimelineExporter.Editor
{
    /// <summary>
    /// Exports Unity TimelineAsset to serialized TimelineData (JSON).
    /// </summary>
    public static class TimelineExporter
    {
        private const string MenuPath = "Assets/Timeline Exporter/Export Timeline";

        [MenuItem(MenuPath, true)]
        private static bool ValidateExport()
        {
            return Selection.activeObject is TimelineAsset;
        }

        [MenuItem(MenuPath)]
        private static void Export()
        {
            var asset = Selection.activeObject as TimelineAsset;
            if (asset == null) return;

            var path = EditorUtility.SaveFilePanel("Export Timeline", "Assets", asset.name, "json");
            if (string.IsNullOrEmpty(path)) return;

            var data = ExportTimeline(asset);
            var json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);
            AssetDatabase.Refresh();
            Debug.Log($"Timeline exported: {path}");
        }

        public static TimelineData ExportTimeline(TimelineAsset timeline)
        {
            var assetPath = AssetDatabase.GetAssetPath(timeline);
            var resourcePath = GetResourcePathFromAssetPath(assetPath);
            var data = new TimelineData
            {
                Id = timeline.name,
                Name = timeline.name,
                Duration = timeline.duration,
                FrameRate = timeline.editorSettings?.frameRate ?? 60,
                TimelineAssetPath = assetPath,
                TimelineResourcePath = resourcePath,
                Version = 1
            };

            var resourceMap = new Dictionary<string, TimelineResourceData>();

            int trackIndex = 0;
            foreach (var track in timeline.GetOutputTracks())
            {
                if (track == null) continue;
                data.Tracks.Add(ExportTrack(track, trackIndex, resourceMap));
                trackIndex++;
            }

            data.Resources.AddRange(resourceMap.Values);
            return data;
        }

        private static TimelineTrackData ExportTrack(TrackAsset track, int trackIndex,
            Dictionary<string, TimelineResourceData> resourceMap)
        {
            string bindingType = null;
            foreach (var output in track.outputs)
            {
                bindingType = output.outputTargetType?.FullName ?? "";
                break;
            }

            var trackData = new TimelineTrackData
            {
                Id = track.name,
                Name = track.name,
                TrackType = track.GetType().Name,
                Muted = track.muted,
                BindingType = bindingType ?? ""
            };

            int clipIndex = 0;
            foreach (var clip in track.GetClips())
            {
                if (clip?.asset == null) continue;
                trackData.Clips.Add(TimelineExporterClip.Export(clip, track, trackIndex, clipIndex, resourceMap));
                clipIndex++;
            }

            return trackData;
        }

        private static string GetResourcePathFromAssetPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return "";
            const string resourcesToken = "/Resources/";
            var idx = path.IndexOf(resourcesToken, System.StringComparison.OrdinalIgnoreCase);
            if (idx < 0) return "";
            var subPath = path.Substring(idx + resourcesToken.Length);
            return string.IsNullOrEmpty(subPath) ? "" : Path.ChangeExtension(subPath, null);
        }
    }
}
