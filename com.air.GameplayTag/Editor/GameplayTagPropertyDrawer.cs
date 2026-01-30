using UnityEngine;
using UnityEditor;

namespace Air.GameplayTag.Editor
{
    /// <summary>
    /// GameplayTag的自定义属性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(GameplayTag))]
    public class GameplayTagPropertyDrawer : PropertyDrawer
    {
        private const float ButtonWidth = 20f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var tagNameProp = property.FindPropertyRelative("tagName");
            
            // 绘制标签
            Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
            EditorGUI.LabelField(labelRect, label);

            // 绘制标签名称字段
            Rect fieldRect = new Rect(
                position.x + EditorGUIUtility.labelWidth,
                position.y,
                position.width - EditorGUIUtility.labelWidth - ButtonWidth - 5f,
                position.height
            );

            EditorGUI.BeginChangeCheck();
            string newValue = EditorGUI.TextField(fieldRect, tagNameProp.stringValue);
            if (EditorGUI.EndChangeCheck())
            {
                tagNameProp.stringValue = newValue;
            }

            // 绘制选择按钮
            Rect buttonRect = new Rect(
                position.x + position.width - ButtonWidth,
                position.y,
                ButtonWidth,
                position.height
            );

            if (GUI.Button(buttonRect, "▼", EditorStyles.miniButton))
            {
                ShowTagSelectionMenu(tagNameProp);
            }

            EditorGUI.EndProperty();
        }

        private void ShowTagSelectionMenu(SerializedProperty tagNameProp)
        {
            var database = GameplayTagDatabase.Instance;
            if (database == null)
            {
                EditorUtility.DisplayDialog("Error", "GameplayTagDatabase not found!", "OK");
                return;
            }

            var allTags = database.GetAllTags();
            if (allTags.Count == 0)
            {
                EditorUtility.DisplayDialog("Info", "No tags defined in database. Please create tags first.", "OK");
                return;
            }

            GenericMenu menu = new GenericMenu();
            
            // 添加清空选项
            menu.AddItem(new GUIContent("None"), string.IsNullOrEmpty(tagNameProp.stringValue), () =>
            {
                tagNameProp.stringValue = string.Empty;
                tagNameProp.serializedObject.ApplyModifiedProperties();
            });

            menu.AddSeparator("");

            // 添加所有标签
            foreach (var tag in allTags)
            {
                bool isSelected = tagNameProp.stringValue == tag;
                menu.AddItem(new GUIContent(tag), isSelected, () =>
                {
                    tagNameProp.stringValue = tag;
                    tagNameProp.serializedObject.ApplyModifiedProperties();
                });
            }

            menu.ShowAsContext();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}


