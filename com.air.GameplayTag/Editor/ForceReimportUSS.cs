using UnityEditor;
using UnityEngine;

namespace Air.GameplayTag.Editor
{
    /// <summary>
    /// 强制重新导入USS文件的工具
    /// </summary>
    public static class ForceReimportUSS
    {
        [MenuItem("Tools/Gameplay Tag/Reimport USS Style")]
        public static void ReimportStyleSheet()
        {
            var guids = AssetDatabase.FindAssets("GameplayTagManagerWindow t:StyleSheet");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                Debug.Log($"Reimported style sheet: {path}");
            }
            else
            {
                Debug.LogWarning("Could not find GameplayTagManagerWindow.uss");
            }
        }
    }
}

