using UnityEngine;
using UnityEditor;
using System.IO;
using Air.GameplayTag;

namespace Air.GameplayTag.Editor
{
    /// <summary>
    /// GameplayTag数据库备份工具
    /// </summary>
    public static class GameplayTagDatabaseBackup
    {
        [MenuItem("Tools/Gameplay Tag/Backup Database")]
        public static void BackupDatabase()
        {
            var database = GameplayTagDatabase.Instance;
            if (database == null)
            {
                EditorUtility.DisplayDialog("Error", "No database found!", "OK");
                return;
            }

            string sourcePath = AssetDatabase.GetAssetPath(database);
            string backupFolder = "Assets/GameplayTagBackups";
            
            // 创建备份文件夹
            if (!Directory.Exists(backupFolder))
            {
                Directory.CreateDirectory(backupFolder);
            }

            // 生成备份文件名（带时间戳）
            string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string backupFileName = $"GameplayTagDatabase_Backup_{timestamp}.asset";
            string backupPath = Path.Combine(backupFolder, backupFileName);

            // 复制文件
            AssetDatabase.CopyAsset(sourcePath, backupPath);
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Backup Complete", 
                $"Database backed up to:\n{backupPath}\n\nTotal Tags: {database.GetAllTags().Count}", 
                "OK");

            // 高亮备份文件
            var backupAsset = AssetDatabase.LoadAssetAtPath<GameplayTagDatabase>(backupPath);
            EditorGUIUtility.PingObject(backupAsset);
        }

        [MenuItem("Tools/Gameplay Tag/Restore from Backup")]
        public static void RestoreFromBackup()
        {
            string backupFolder = "Assets/GameplayTagBackups";
            
            if (!Directory.Exists(backupFolder))
            {
                EditorUtility.DisplayDialog("Error", "No backup folder found!", "OK");
                return;
            }

            string path = EditorUtility.OpenFilePanel(
                "Select Backup File",
                backupFolder,
                "asset"
            );

            if (string.IsNullOrEmpty(path))
                return;

            // 转换为相对路径
            if (path.StartsWith(Application.dataPath))
            {
                path = "Assets" + path.Substring(Application.dataPath.Length);
            }

            var backupDatabase = AssetDatabase.LoadAssetAtPath<GameplayTagDatabase>(path);
            if (backupDatabase == null)
            {
                EditorUtility.DisplayDialog("Error", "Selected file is not a valid GameplayTagDatabase!", "OK");
                return;
            }

            var currentDatabase = GameplayTagDatabase.Instance;
            if (currentDatabase == null)
            {
                EditorUtility.DisplayDialog("Error", "No current database found!", "OK");
                return;
            }

            if (EditorUtility.DisplayDialog("Confirm Restore",
                $"This will replace the current database with the backup.\n\n" +
                $"Current Tags: {currentDatabase.GetAllTags().Count}\n" +
                $"Backup Tags: {backupDatabase.GetAllTags().Count}\n\n" +
                $"Continue?",
                "Restore", "Cancel"))
            {
                string currentPath = AssetDatabase.GetAssetPath(currentDatabase);
                AssetDatabase.DeleteAsset(currentPath);
                AssetDatabase.CopyAsset(path, currentPath);
                AssetDatabase.Refresh();

                EditorUtility.DisplayDialog("Restore Complete", 
                    "Database has been restored from backup!", 
                    "OK");
            }
        }

        [MenuItem("Tools/Gameplay Tag/Export to JSON")]
        public static void ExportToJSON()
        {
            var database = GameplayTagDatabase.Instance;
            if (database == null)
            {
                EditorUtility.DisplayDialog("Error", "No database found!", "OK");
                return;
            }

            string path = EditorUtility.SaveFilePanel(
                "Export Tags to JSON",
                "",
                "GameplayTags.json",
                "json"
            );

            if (string.IsNullOrEmpty(path))
                return;

            var allTags = database.GetAllTags();
            var json = JsonUtility.ToJson(new TagListWrapper { tags = allTags.ToArray() }, true);
            File.WriteAllText(path, json);

            EditorUtility.DisplayDialog("Export Complete",
                $"Exported {allTags.Count} tags to:\n{path}",
                "OK");
        }

        [MenuItem("Tools/Gameplay Tag/Import from JSON")]
        public static void ImportFromJSON()
        {
            var database = GameplayTagDatabase.Instance;
            if (database == null)
            {
                EditorUtility.DisplayDialog("Error", "No database found!", "OK");
                return;
            }

            string path = EditorUtility.OpenFilePanel(
                "Import Tags from JSON",
                "",
                "json"
            );

            if (string.IsNullOrEmpty(path))
                return;

            try
            {
                string json = File.ReadAllText(path);
                var wrapper = JsonUtility.FromJson<TagListWrapper>(json);

                if (wrapper == null || wrapper.tags == null)
                {
                    EditorUtility.DisplayDialog("Import Failed", "Invalid JSON format", "OK");
                    return;
                }

                int addedCount = 0;
                foreach (var tag in wrapper.tags)
                {
                    if (database.AddTag(tag))
                        addedCount++;
                }

                EditorUtility.SetDirty(database);
                AssetDatabase.SaveAssets();

                EditorUtility.DisplayDialog("Import Complete",
                    $"Imported {addedCount} new tags\n(Skipped {wrapper.tags.Length - addedCount} duplicates)",
                    "OK");
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("Import Failed", $"Error: {e.Message}", "OK");
            }
        }

        [System.Serializable]
        private class TagListWrapper
        {
            public string[] tags;
        }
    }
}

