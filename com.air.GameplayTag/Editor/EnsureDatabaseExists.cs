using UnityEngine;
using UnityEditor;
using Air.GameplayTag;
using System.IO;

namespace Air.GameplayTag.Editor
{
    /// <summary>
    /// 确保数据库文件存在的工具
    /// </summary>
    [InitializeOnLoad]
    public static class EnsureDatabaseExists
    {
        static EnsureDatabaseExists()
        {
            EditorApplication.delayCall += CheckDatabase;
        }

        private static void CheckDatabase()
        {
            string path = "Assets/Resources/GameplayTagDatabase.asset";
            
            if (!File.Exists(path))
            {
                Debug.LogWarning("⚠️ GameplayTagDatabase.asset not found! Creating default database...");
                CreateDefaultDatabase();
            }
        }

        [MenuItem("Tools/Gameplay Tag/Create Database (Force)")]
        public static void CreateDefaultDatabase()
        {
            // 确保 Resources 文件夹存在
            string resourcesPath = "Assets/Resources";
            if (!AssetDatabase.IsValidFolder(resourcesPath))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
                Debug.Log("✓ Created Resources folder");
            }

            string assetPath = "Assets/Resources/GameplayTagDatabase.asset";
            
            // 检查是否已存在
            var existing = AssetDatabase.LoadAssetAtPath<GameplayTagDatabase>(assetPath);
            if (existing != null)
            {
                if (EditorUtility.DisplayDialog("Database Already Exists",
                    $"A database already exists at:\n{assetPath}\n\n" +
                    $"Total Tags: {existing.GetAllTags().Count}\n\n" +
                    "Do you want to replace it with a new empty database?",
                    "Replace", "Cancel"))
                {
                    AssetDatabase.DeleteAsset(assetPath);
                }
                else
                {
                    EditorGUIUtility.PingObject(existing);
                    return;
                }
            }

            // 创建新数据库
            var database = ScriptableObject.CreateInstance<GameplayTagDatabase>();
            
            // 保存为资源文件
            AssetDatabase.CreateAsset(database, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"✓ Database created and saved at: {assetPath}");
            
            // 高亮显示文件
            EditorGUIUtility.PingObject(database);
            Selection.activeObject = database;

            // 显示成功对话框
            EditorUtility.DisplayDialog("Success!",
                $"✅ Database created successfully!\n\n" +
                $"Location: {assetPath}\n\n" +
                "You can now add tags using:\n" +
                "Window > Gameplay Tag Manager",
                "OK");
        }
    }
}

