using UnityEngine;
using UnityEditor;

namespace Air.GameplayTag.Editor
{
    /// <summary>
    /// 保存状态指示器 - 在编辑器中显示保存状态
    /// </summary>
    [InitializeOnLoad]
    public static class SaveStatusIndicator
    {
        private static double lastSaveTime;
        private static string lastSavedTag = "";

        static SaveStatusIndicator()
        {
            EditorApplication.update += OnEditorUpdate;
        }

        public static void NotifySaved(string tagInfo)
        {
            lastSaveTime = EditorApplication.timeSinceStartup;
            lastSavedTag = tagInfo;
        }

        private static void OnEditorUpdate()
        {
            // 在保存后的3秒内显示提示
            if (EditorApplication.timeSinceStartup - lastSaveTime < 3.0 && !string.IsNullOrEmpty(lastSavedTag))
            {
                // 这个会在 Scene 视图显示
                SceneView.RepaintAll();
            }
        }
    }
}

