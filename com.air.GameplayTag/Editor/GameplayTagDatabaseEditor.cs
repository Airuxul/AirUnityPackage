using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Air.GameplayTag;

namespace Air.GameplayTag.Editor
{
    /// <summary>
    /// GameplayTagDatabase的自定义编辑器 - UIToolkit实现
    /// </summary>
    [CustomEditor(typeof(GameplayTagDatabase))]
    public class GameplayTagDatabaseEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var database = target as GameplayTagDatabase;
            if (database == null)
                return new VisualElement();

            var root = new VisualElement();
            root.style.paddingTop = 10;
            root.style.paddingBottom = 10;
            root.style.paddingLeft = 5;
            root.style.paddingRight = 5;

            // 标题
            var titleLabel = new Label("Gameplay Tag Database");
            titleLabel.style.fontSize = 14;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            titleLabel.style.marginBottom = 8;
            root.Add(titleLabel);

            // 提示框
            var helpBox = new HelpBox("Use the Gameplay Tag Manager window to edit tags.", HelpBoxMessageType.Info);
            helpBox.style.marginBottom = 10;
            root.Add(helpBox);

            // 标签列表区域
            var allTags = database.GetAllTags();
            if (allTags.Count > 0)
            {
                var tagsFoldout = new Foldout();
                tagsFoldout.text = $"Tags: ({allTags.Count})";
                tagsFoldout.value = true;
                tagsFoldout.style.marginBottom = 10;
                tagsFoldout.style.marginTop = 5;

                // 标签容器
                var tagsContainer = new VisualElement();
                tagsContainer.style.backgroundColor = new Color(0, 0, 0, 0.1f);
                tagsContainer.style.borderTopLeftRadius = 4;
                tagsContainer.style.borderTopRightRadius = 4;
                tagsContainer.style.borderBottomLeftRadius = 4;
                tagsContainer.style.borderBottomRightRadius = 4;
                tagsContainer.style.paddingTop = 8;
                tagsContainer.style.paddingBottom = 8;
                tagsContainer.style.paddingLeft = 10;
                tagsContainer.style.paddingRight = 10;
                tagsContainer.style.marginTop = 5;
                tagsContainer.style.marginLeft = 15;
                tagsContainer.style.maxHeight = 300;

                // 滚动视图
                var scrollView = new ScrollView(ScrollViewMode.Vertical);
                scrollView.style.maxHeight = 300;

                foreach (var tag in allTags)
                {
                    var tagLabel = new Label("• " + tag);
                    tagLabel.style.fontSize = 11;
                    tagLabel.style.marginBottom = 2;
                    tagLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
                    scrollView.Add(tagLabel);
                }

                tagsContainer.Add(scrollView);
                tagsFoldout.Add(tagsContainer);
                root.Add(tagsFoldout);
            }
            else
            {
                // 无标签提示
                var emptyLabel = new Label("Total Tags: 0");
                emptyLabel.style.backgroundColor = new Color(0, 0, 0, 0.1f);
                emptyLabel.style.paddingTop = 8;
                emptyLabel.style.paddingBottom = 8;
                emptyLabel.style.paddingLeft = 10;
                emptyLabel.style.paddingRight = 10;
                emptyLabel.style.borderTopLeftRadius = 4;
                emptyLabel.style.borderTopRightRadius = 4;
                emptyLabel.style.borderBottomLeftRadius = 4;
                emptyLabel.style.borderBottomRightRadius = 4;
                emptyLabel.style.marginBottom = 10;
                emptyLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
                root.Add(emptyLabel);
            }

            // 打开管理器按钮
            var openButton = new Button(() => GameplayTagManagerWindow.ShowWindow());
            openButton.text = "Open Tag Manager Window";
            openButton.style.height = 32;
            openButton.style.fontSize = 12;
            openButton.style.unityFontStyleAndWeight = FontStyle.Bold;
            openButton.style.backgroundColor = new Color(0.2f, 0.5f, 0.8f, 1f);
            openButton.style.color = Color.white;
            openButton.style.borderTopLeftRadius = 4;
            openButton.style.borderTopRightRadius = 4;
            openButton.style.borderBottomLeftRadius = 4;
            openButton.style.borderBottomRightRadius = 4;
            openButton.style.marginTop = 5;
            
            // 悬停效果
            openButton.RegisterCallback<MouseEnterEvent>(evt =>
            {
                openButton.style.backgroundColor = new Color(0.25f, 0.55f, 0.85f, 1f);
            });
            openButton.RegisterCallback<MouseLeaveEvent>(evt =>
            {
                openButton.style.backgroundColor = new Color(0.2f, 0.5f, 0.8f, 1f);
            });
            
            root.Add(openButton);

            return root;
        }
    }
}


