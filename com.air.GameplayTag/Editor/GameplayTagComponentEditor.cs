using UnityEngine;
using UnityEditor;
using Air.GameplayTag;

namespace Air.GameplayTag.Editor
{
    /// <summary>
    /// GameplayTagComponent的自定义编辑器
    /// </summary>
    [CustomEditor(typeof(GameplayTagComponent))]
    public class GameplayTagComponentEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space(5);
            
            EditorGUILayout.LabelField("Gameplay Tags", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Tags assigned to this GameObject", MessageType.Info);
            
            EditorGUILayout.Space(5);

            // 绘制标签容器
            var tagsProp = serializedObject.FindProperty("tags");
            if (tagsProp != null)
            {
                EditorGUILayout.PropertyField(tagsProp, new GUIContent("Tags"), true);
            }

            EditorGUILayout.Space(10);

            // 快速操作按钮
            EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Clear All Tags"))
            {
                var component = target as GameplayTagComponent;
                if (component != null && EditorUtility.DisplayDialog("Clear Tags", "Remove all tags?", "Yes", "No"))
                {
                    component.ClearTags();
                    EditorUtility.SetDirty(target);
                }
            }
            
            if (GUILayout.Button("Open Tag Manager"))
            {
                GameplayTagManagerWindow.ShowWindow();
            }
            
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }
}


