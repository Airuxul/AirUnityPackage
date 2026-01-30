using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace Air.GameplayTag.Editor
{
    /// <summary>
    /// 树形标签项
    /// </summary>
    public class TagTreeItem : VisualElement
    {
        private readonly GameplayTagDatabase.TagNode _node;
        private readonly string _fullPath;
        private int _depth;
        private readonly GameplayTagManagerWindow _window;
        private Label _nameLabel;
        private TextField _nameField;
        private Button _foldoutButton;
        private bool _isExpanded;
        private bool _isRenaming;
        private Label _pathLabel;
        private VisualElement _buttonContainer;

        private const string ExpandedKey = "TagExpanded_";

        public bool IsExpanded => _isExpanded;

        public TagTreeItem(GameplayTagDatabase.TagNode node, string fullPath, int depth, GameplayTagManagerWindow window)
        {
            _node = node;
            _fullPath = fullPath;
            _depth = depth;
            _window = window;
            _isExpanded = EditorPrefs.GetBool(ExpandedKey + fullPath, true);

            AddToClassList("tag-item");
            style.paddingLeft = depth * 20;

            BuildUI();
        }

        private void BuildUI()
        {
            var container = new VisualElement();
            container.AddToClassList("tag-item-container");

            bool hasChildren = _node.children.Count > 0;
            if (hasChildren)
            {
                _foldoutButton = new Button(ToggleFoldout);
                _foldoutButton.AddToClassList("foldout-button");
                _foldoutButton.text = _isExpanded ? "▼" : "▶";
                container.Add(_foldoutButton);
            }
            else
            {
                var spacer = new VisualElement();
                spacer.AddToClassList("foldout-spacer");
                container.Add(spacer);
            }

            var nameContainer = new VisualElement();
            nameContainer.AddToClassList("name-container");

            _nameLabel = new Label(_node.tagName);
            _nameLabel.AddToClassList("name-label");
            if (hasChildren)
            {
                _nameLabel.AddToClassList("has-children");
            }
            nameContainer.Add(_nameLabel);

            _nameField = new TextField();
            _nameField.AddToClassList("name-field");
            _nameField.style.display = DisplayStyle.None;
            _nameField.RegisterCallback<BlurEvent>(_ => SaveRename());
            _nameField.RegisterCallback<KeyDownEvent>(evt =>
            {
                switch (evt.keyCode)
                {
                    case KeyCode.Return or KeyCode.KeypadEnter:
                        SaveRename();
                        evt.StopPropagation();
                        break;
                    case KeyCode.Escape:
                        CancelRename();
                        evt.StopPropagation();
                        break;
                }
            });
            nameContainer.Add(_nameField);

            _pathLabel = new Label(_fullPath);
            _pathLabel.AddToClassList("path-hint");
            nameContainer.Add(_pathLabel);

            container.Add(nameContainer);

            _buttonContainer = new VisualElement();
            _buttonContainer.AddToClassList("button-container");

            var addButton = new Button(() => _window.ShowAddTagDialog(_fullPath));
            addButton.AddToClassList("action-button");
            addButton.AddToClassList("add-button");
            addButton.text = "+";
            addButton.tooltip = "Add child tag";
            _buttonContainer.Add(addButton);

            var deleteButton = new Button(() => _window.DeleteTag(_fullPath, _node.children.Count > 0));
            deleteButton.AddToClassList("action-button");
            deleteButton.AddToClassList("delete-button");
            deleteButton.text = "×";
            deleteButton.tooltip = "Delete tag";
            _buttonContainer.Add(deleteButton);

            container.Add(_buttonContainer);

            Add(container);

            _nameLabel.RegisterCallback<MouseDownEvent>(evt =>
            {
                if (evt.clickCount == 2 && evt.button == 0)
                {
                    StartRename();
                    evt.StopPropagation();
                }
            });

            this.RegisterCallback<ContextualMenuPopulateEvent>(evt =>
            {
                evt.menu.AppendAction("Add Child Tag", _ => _window.ShowAddTagDialog(_fullPath));
                evt.menu.AppendAction("Rename", _ => StartRename());
                evt.menu.AppendSeparator();
                evt.menu.AppendAction("Copy Full Path", _ => 
                {
                    EditorGUIUtility.systemCopyBuffer = _fullPath;
                    Debug.Log($"Copied to clipboard: {_fullPath}");
                });
                evt.menu.AppendSeparator();
                
                string deleteLabel = _node.children.Count > 0 ? "Delete (with children)" : "Delete";
                evt.menu.AppendAction(deleteLabel, _ => _window.DeleteTag(_fullPath, _node.children.Count > 0));
            });
        }

        private void ToggleFoldout()
        {
            _isExpanded = !_isExpanded;
            EditorPrefs.SetBool(ExpandedKey + _fullPath, _isExpanded);
            _foldoutButton.text = _isExpanded ? "▼" : "▶";
            _window.RefreshTree();
        }

        private void StartRename()
        {
            if (_isRenaming)
                return;

            _isRenaming = true;
            _window.StartRenaming(this);

            _nameLabel.style.display = DisplayStyle.None;
            _nameField.style.display = DisplayStyle.Flex;
            _nameField.value = _node.tagName;
            _nameField.Focus();
            _nameField.SelectAll();
        }

        public void CancelRename()
        {
            if (!_isRenaming)
                return;

            _isRenaming = false;
            _nameLabel.style.display = DisplayStyle.Flex;
            _nameField.style.display = DisplayStyle.None;
            _window.FinishRenaming(this, null);
        }

        private void SaveRename()
        {
            if (!_isRenaming)
                return;

            string newName = _nameField.value;
            if (string.IsNullOrEmpty(newName) || newName == _node.tagName)
            {
                CancelRename();
                return;
            }

            var database = _window.GetDatabase();
            if (database.RenameTag(_fullPath, newName))
            {
                _isRenaming = false;
                _window.FinishRenaming(this, newName);
                _window.RefreshTree();
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

