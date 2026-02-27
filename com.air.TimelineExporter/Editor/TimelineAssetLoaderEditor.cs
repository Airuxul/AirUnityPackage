using UnityEditor;
using UnityEngine.Timeline;

namespace TimelineExporter.Editor
{
    [InitializeOnLoad]
    public static class TimelineAssetLoaderEditor
    {
        static TimelineAssetLoaderEditor()
        {
            TimelineAssetLoader.LoadFromAssetPath = path => string.IsNullOrEmpty(path) ? null : AssetDatabase.LoadAssetAtPath<TimelineAsset>(path);
        }
    }
}
