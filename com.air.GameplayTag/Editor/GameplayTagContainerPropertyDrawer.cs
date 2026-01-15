using UnityEngine;
using UnityEditor;
using Air.GameplayTag;
using System.Collections.Generic;

namespace Air.GameplayTag.Editor
{
    /// <summary>
    /// GameplayTagContainer的自定义属性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(GameplayTagContainer))]
    public class GameplayTagContainerPropertyDrawer : PropertyDrawer
    {
        private bool isExpanded = false;
        private const float LineHeight = 20f;
        private const float Spacing = 2f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var tagsProp = property.FindPropertyRelative("tags");
            if (tagsProp == null)
            {
                EditorGUI.LabelField(position, label.text, "Error: tags field not found");
                EditorGUI.EndProperty();
                return;
            }

            // 绘制折叠箭头和标签
            Rect foldoutRect = new Rect(position.x, position.y, position.width - 60f, LineHeight);
            isExpanded = EditorGUI.Foldout(foldoutRect, isExpanded, $"{label.text} ({tagsProp.arraySize} tags)", true);

            // 绘制添加按钮
            Rect addButtonRect = new Rect(position.x + position.width - 55f, position.y, 25f, LineHeight - 2f);
            if (GUI.Button(addButtonRect, "+", EditorStyles.miniButton))
            {
                ShowAddTagMenu(tagsProp);
            }

            // 绘制清空按钮
            Rect clearButtonRect = new Rect(position.x + position.width - 28f, position.y, 28f, LineHeight - 2f);
            if (GUI.Button(clearButtonRect, "×", EditorStyles.miniButton))
            {
                if (EditorUtility.DisplayDialog("Clear Tags", "Remove all tags from this container?", "Yes", "No"))
                {
                    tagsProp.ClearArray();
                    tagsProp.serializedObject.ApplyModifiedProperties();
                }
            }

            if (isExpanded)
            {
                EditorGUI.indentLevel++;
                float yOffset = position.y + LineHeight + Spacing;

                // 绘制所有标签
                for (int i = 0; i < tagsProp.arraySize; i++)
                {
                    Rect tagRect = new Rect(position.x, yOffset, position.width, LineHeight);
                    DrawTagElement(tagRect, tagsProp, i);
                    yOffset += LineHeight + Spacing;
                }

                // 如果没有标签，显示提示
                if (tagsProp.arraySize == 0)
                {
                    Rect emptyRect = new Rect(position.x, yOffset, position.width, LineHeight);
                    EditorGUI.LabelField(emptyRect, "No tags added", EditorStyles.miniLabel);
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        private void DrawTagElement(Rect position, SerializedProperty tagsProp, int index)
        {
            var elementProp = tagsProp.GetArrayElementAtIndex(index);
            var tagNameProp = elementProp.FindPropertyRelative("tagName");

            // 绘制标签名称
            Rect labelRect = new Rect(position.x, position.y, position.width - 30f, position.height);
            EditorGUI.LabelField(labelRect, tagNameProp.stringValue);

            // 绘制删除按钮
            Rect removeButtonRect = new Rect(position.x + position.width - 25f, position.y, 25f, position.height - 2f);
            if (GUI.Button(removeButtonRect, "-", EditorStyles.miniButton))
            {
                tagsProp.DeleteArrayElementAtIndex(index);
                tagsProp.serializedObject.ApplyModifiedProperties();
            }
        }

        private void ShowAddTagMenu(SerializedProperty tagsProp)
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

            // 获取当前已有的标签
            HashSet<string> existingTags = new HashSet<string>();
            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                var element = tagsProp.GetArrayElementAtIndex(i);
                var tagName = element.FindPropertyRelative("tagName");
                if (tagName != null && !string.IsNullOrEmpty(tagName.stringValue))
                {
                    existingTags.Add(tagName.stringValue);
                }
            }

            GenericMenu menu = new GenericMenu();

            // 添加所有标签
            foreach (var tag in allTags)
            {
                bool isAdded = existingTags.Contains(tag);
                if (!isAdded)
                {
                    menu.AddItem(new GUIContent(tag), false, () =>
                    {
                        tagsProp.arraySize++;
                        var newElement = tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1);
                        var newTagName = newElement.FindPropertyRelative("tagName");
                        newTagName.stringValue = tag;
                        tagsProp.serializedObject.ApplyModifiedProperties();
                    });
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent(tag + " (already added)"));
                }
            }

            menu.ShowAsContext();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!isExpanded)
                return LineHeight;

            var tagsProp = property.FindPropertyRelative("tags");
            if (tagsProp == null)
                return LineHeight;

            int lines = 1 + Mathf.Max(1, tagsProp.arraySize);
            return lines * (LineHeight + Spacing);
        }
    }
}


