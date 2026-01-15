using UnityEngine;
using UnityEditor;
using Air.GameplayTag;

namespace Air.GameplayTag.Editor
{
    /// <summary>
    /// 显示数据库信息的工具
    /// </summary>
    public static class GameplayTagDatabaseInfo
    {
        [MenuItem("Tools/Gameplay Tag/Show Database Info")]
        public static void ShowDatabaseInfo()
        {
            var database = GameplayTagDatabase.Instance;
            
            if (database == null)
            {
                EditorUtility.DisplayDialog("Database Info", 
                    "❌ No database found!\n\n" +
                    "Please create a database first:\n" +
                    "1. Open Window > Gameplay Tag Manager\n" +
                    "2. Click 'Create Database'\n" +
                    "3. Save to Assets/Resources/GameplayTagDatabase.asset", 
                    "OK");
                return;
            }

            string path = AssetDatabase.GetAssetPath(database);
            int tagCount = database.GetAllTags().Count;
            long fileSize = 0;
            
            if (!string.IsNullOrEmpty(path))
            {
                string fullPath = Application.dataPath + path.Substring("Assets".Length);
                if (System.IO.File.Exists(fullPath))
                {
                    fileSize = new System.IO.FileInfo(fullPath).Length;
                }
            }

            string message = "📊 Gameplay Tag Database Info\n\n" +
                           $"📁 File Path:\n{path}\n\n" +
                           $"📝 Total Tags: {tagCount}\n\n" +
                           $"💾 File Size: {FormatBytes(fileSize)}\n\n" +
                           $"✅ Status: Active and loaded";

            if (EditorUtility.DisplayDialog("Database Info", message, "Open in Project", "Close"))
            {
                EditorGUIUtility.PingObject(database);
                Selection.activeObject = database;
            }
        }

        [MenuItem("Tools/Gameplay Tag/Locate Database File")]
        public static void LocateDatabaseFile()
        {
            var database = GameplayTagDatabase.Instance;
            
            if (database == null)
            {
                EditorUtility.DisplayDialog("Error", "No database found!", "OK");
                return;
            }

            string path = AssetDatabase.GetAssetPath(database);
            string fullPath = System.IO.Path.GetFullPath(path);

            EditorUtility.DisplayDialog("Database File Location", 
                $"Project Path:\n{path}\n\n" +
                $"Full Path:\n{fullPath}\n\n" +
                "The file will be highlighted in the Project window.", 
                "OK");

            // 高亮显示文件
            EditorGUIUtility.PingObject(database);
            Selection.activeObject = database;
        }

        [MenuItem("Tools/Gameplay Tag/Check Database Status")]
        public static void CheckDatabaseStatus()
        {
            Debug.Log("=== Gameplay Tag Database Status ===");
            
            var database = GameplayTagDatabase.Instance;
            
            if (database == null)
            {
                Debug.LogError("❌ Database not found!");
                Debug.LogError("Expected location: Assets/Resources/GameplayTagDatabase.asset");
                return;
            }

            string path = AssetDatabase.GetAssetPath(database);
            Debug.Log($"✓ Database found at: {path}");
            Debug.Log($"✓ Total tags: {database.GetAllTags().Count}");
            
            var allTags = database.GetAllTags();
            Debug.Log("\n=== All Tags ===");
            foreach (var tag in allTags)
            {
                Debug.Log($"  • {tag}");
            }
        }

        private static string FormatBytes(long bytes)
        {
            if (bytes < 1024)
                return $"{bytes} B";
            else if (bytes < 1024 * 1024)
                return $"{bytes / 1024.0:F2} KB";
            else
                return $"{bytes / (1024.0 * 1024.0):F2} MB";
        }
    }
}

