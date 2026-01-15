using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using Air.GameplayTag;

namespace Air.GameplayTag.Editor
{
    /// <summary>
    /// 树形标签项
    /// </summary>
    public class TagTreeItem : VisualElement
    {
        private GameplayTagDatabase.TagNode node;
        private string fullPath;
        private int depth;
        private GameplayTagManagerWindow window;
        private Label nameLabel;
        private TextField nameField;
        private Button foldoutButton;
        private bool isExpanded;
        private bool isRenaming = false;
        private Label pathLabel;
        private VisualElement buttonContainer;

        private const string ExpandedKey = "TagExpanded_";

        public bool IsExpanded => isExpanded;

        public TagTreeItem(GameplayTagDatabase.TagNode node, string fullPath, int depth, GameplayTagManagerWindow window)
        {
            this.node = node;
            this.fullPath = fullPath;
            this.depth = depth;
            this.window = window;
            this.isExpanded = EditorPrefs.GetBool(ExpandedKey + fullPath, true);

            AddToClassList("tag-item");
            style.paddingLeft = depth * 20;

            BuildUI();
        }

        private void BuildUI()
        {
            var container = new VisualElement();
            container.AddToClassList("tag-item-container");

            bool hasChildren = node.children.Count > 0;
            if (hasChildren)
            {
                foldoutButton = new Button(ToggleFoldout);
                foldoutButton.AddToClassList("foldout-button");
                foldoutButton.text = isExpanded ? "▼" : "▶";
                container.Add(foldoutButton);
            }
            else
            {
                var spacer = new VisualElement();
                spacer.AddToClassList("foldout-spacer");
                container.Add(spacer);
            }

            var nameContainer = new VisualElement();
            nameContainer.AddToClassList("name-container");

            nameLabel = new Label(node.tagName);
            nameLabel.AddToClassList("name-label");
            if (hasChildren)
            {
                nameLabel.AddToClassList("has-children");
            }
            nameContainer.Add(nameLabel);

            nameField = new TextField();
            nameField.AddToClassList("name-field");
            nameField.style.display = DisplayStyle.None;
            nameField.RegisterCallback<BlurEvent>(evt => SaveRename());
            nameField.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                {
                    SaveRename();
                    evt.StopPropagation();
                }
                else if (evt.keyCode == KeyCode.Escape)
                {
                    CancelRename();
                    evt.StopPropagation();
                }
            });
            nameContainer.Add(nameField);

            pathLabel = new Label(fullPath);
            pathLabel.AddToClassList("path-hint");
            nameContainer.Add(pathLabel);

            container.Add(nameContainer);

            buttonContainer = new VisualElement();
            buttonContainer.AddToClassList("button-container");

            var addButton = new Button(() => window.ShowAddTagDialog(fullPath));
            addButton.AddToClassList("action-button");
            addButton.AddToClassList("add-button");
            addButton.text = "+";
            addButton.tooltip = "Add child tag";
            buttonContainer.Add(addButton);

            var deleteButton = new Button(() => window.DeleteTag(fullPath, node.children.Count > 0));
            deleteButton.AddToClassList("action-button");
            deleteButton.AddToClassList("delete-button");
            deleteButton.text = "×";
            deleteButton.tooltip = "Delete tag";
            buttonContainer.Add(deleteButton);

            container.Add(buttonContainer);

            Add(container);

            nameLabel.RegisterCallback<MouseDownEvent>(evt =>
            {
                if (evt.clickCount == 2 && evt.button == 0)
                {
                    StartRename();
                    evt.StopPropagation();
                }
            });

            this.RegisterCallback<ContextualMenuPopulateEvent>(evt =>
            {
                evt.menu.AppendAction("Add Child Tag", a => window.ShowAddTagDialog(fullPath));
                evt.menu.AppendAction("Rename", a => StartRename());
                evt.menu.AppendSeparator();
                evt.menu.AppendAction("Copy Full Path", a => 
                {
                    EditorGUIUtility.systemCopyBuffer = fullPath;
                    Debug.Log($"Copied to clipboard: {fullPath}");
                });
                evt.menu.AppendSeparator();
                
                string deleteLabel = node.children.Count > 0 ? "Delete (with children)" : "Delete";
                evt.menu.AppendAction(deleteLabel, a => window.DeleteTag(fullPath, node.children.Count > 0));
            });
        }

        private void ToggleFoldout()
        {
            isExpanded = !isExpanded;
            EditorPrefs.SetBool(ExpandedKey + fullPath, isExpanded);
            foldoutButton.text = isExpanded ? "▼" : "▶";
            window.RefreshTree();
        }

        private void StartRename()
        {
            if (isRenaming)
                return;

            isRenaming = true;
            window.StartRenaming(this);

            nameLabel.style.display = DisplayStyle.None;
            nameField.style.display = DisplayStyle.Flex;
            nameField.value = node.tagName;
            nameField.Focus();
            nameField.SelectAll();
        }

        public void CancelRename()
        {
            if (!isRenaming)
                return;

            isRenaming = false;
            nameLabel.style.display = DisplayStyle.Flex;
            nameField.style.display = DisplayStyle.None;
            window.FinishRenaming(this, null);
        }

        private void SaveRename()
        {
            if (!isRenaming)
                return;

            string newName = nameField.value;
            if (string.IsNullOrEmpty(newName) || newName == node.tagName)
            {
                CancelRename();
                return;
            }

            var database = window.GetDatabase();
            if (database.RenameTag(fullPath, newName))
            {
                isRenaming = false;
                window.FinishRenaming(this, newName);
                window.RefreshTree();
            }
            else
            {
                EditorUtility.DisplayDialog("Rename Failed", 
                    "Failed to rename tag. A tag with this name may already exist at the same level.", "OK");
                CancelRename();
            }
        }
    }
}

