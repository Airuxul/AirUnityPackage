using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;

namespace Air.GameplayTag.Editor
{
    /// <summary>
    /// Gameplay标签管理器窗口 - 支持多配置、手动保存
    /// </summary>
    public class GameplayTagManagerWindow : EditorWindow
    {
        private GameplayTagDatabase _database; // 原始数据库
        private GameplayTagDatabase _workingCopy; // 工作副本
        private bool _hasUnsavedChanges;
        
        private VisualElement _root;
        private ObjectField _databaseField;
        private ScrollView _treeScrollView;
        private VisualElement _treeContainer;
        private TextField _searchField;
        private Label _statusLabel;
        private Button _saveButton;
        private Button _revertButton;
        
        private string _searchQuery = "";
        private TagTreeItem _renamingItem;

        [MenuItem("Tools/Gameplay Tag Manager")]
        public static void ShowWindow()
        {
            var window = GetWindow<GameplayTagManagerWindow>("Gameplay Tag Manager");
            window.minSize = new Vector2(600, 450);
            window.Show();
        }
        
        /// <summary>
        /// 初始化默认数据库，自动查找项目中的GameplayTagDatabase
        /// </summary>
        private void InitializeDefaultDatabase()
        {
            // 如果已经有加载的数据库，跳过
            if (_database != null)
            {
                return;
            }
            
            // 查找项目中所有的GameplayTagDatabase资源
            string[] guids = AssetDatabase.FindAssets("t:GameplayTagDatabase");
            
            if (guids.Length == 0)
            {
                // 没有找到数据库
                _statusLabel.text = "⚠️ No GameplayTagDatabase found in project. Create one from Assets/Create/Gameplay/Gameplay Tag Database";
                return;
            }
            
            if (guids.Length > 1)
            {
                // 找到多个数据库，弹窗提示用户
                string[] paths = new string[guids.Length];
                for (int i = 0; i < guids.Length; i++)
                {
                    paths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
                }
                
                string message = $"Found {guids.Length} GameplayTagDatabase assets in the project:\n\n";
                for (int i = 0; i < paths.Length; i++)
                {
                    message += $"{i + 1}. {paths[i]}\n";
                }
                message += "\nPlease manually select which one to use from the Database field above.";
                
                EditorUtility.DisplayDialog(
                    "Multiple GameplayTagDatabase Found",
                    message,
                    "OK"
                );
                
                _statusLabel.text = $"⚠️ Found {guids.Length} databases. Please select one manually.";
                return;
            }
            
            // 只找到一个数据库，自动加载
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            GameplayTagDatabase database = AssetDatabase.LoadAssetAtPath<GameplayTagDatabase>(assetPath);
            
            if (database != null)
            {
                _database = database;
                _databaseField.value = database;
                LoadDatabase(database);
                Debug.Log($"[GameplayTagManager] Auto-loaded database: {assetPath}");
            }
            else
            {
                _statusLabel.text = "⚠️ Failed to load database";
            }
        }

        private void CreateGUI()
        {
            BuildUI();
        }

        private void BuildUI()
        {
            _root = rootVisualElement;
            _root.style.flexGrow = 1;

            // 加载样式
            LoadStyleSheet();

            // 数据库选择区域
            BuildDatabaseSection();

            // 工具栏
            BuildToolbar();

            // 提示标签
            var tipsLabel = new Label("💡 Drag a Database above | Double-click to rename | Manual Save");
            tipsLabel.AddToClassList("tips-label");
            _root.Add(tipsLabel);

            // 树形视图
            _treeScrollView = new ScrollView(ScrollViewMode.Vertical);
            _treeScrollView.AddToClassList("tree-scroll-view");
            
            _treeContainer = new VisualElement();
            _treeContainer.AddToClassList("tree-container");
            _treeScrollView.Add(_treeContainer);
            
            _root.Add(_treeScrollView);

            // 状态栏
            _statusLabel = new Label();
            _statusLabel.AddToClassList("status-label");
            _root.Add(_statusLabel);

            RefreshTree();
            
            // 初始化并查找默认的GameplayTagDatabase
            InitializeDefaultDatabase();
        }

        private void BuildDatabaseSection()
        {
            var section = new VisualElement();
            section.style.flexDirection = FlexDirection.Row;
            section.style.paddingTop = 10;
            section.style.paddingBottom = 10;
            section.style.paddingLeft = 12;
            section.style.paddingRight = 12;
            section.style.backgroundColor = new Color(0.25f, 0.25f, 0.25f);
            section.style.borderBottomWidth = 1;
            section.style.borderBottomColor = new Color(0.15f, 0.15f, 0.15f);

            var label = new Label("📁 Database:");
            label.style.minWidth = 80;
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            label.style.fontSize = 13;
            section.Add(label);

            _databaseField = new ObjectField();
            _databaseField.objectType = typeof(GameplayTagDatabase);
            _databaseField.style.flexGrow = 1;
            _databaseField.RegisterValueChangedCallback(OnDatabaseChanged);
            section.Add(_databaseField);

            var createButton = new Button(CreateNewDatabase) { text = "Create New" };
            createButton.style.marginLeft = 8;
            createButton.style.width = 100;
            section.Add(createButton);

            _root.Add(section);
        }

        private void BuildToolbar()
        {
            var toolbar = new VisualElement();
            toolbar.AddToClassList("toolbar");

            var addRootButton = new Button(() => ShowAddTagDialog("")) { text = "+ Add Root Tag" };
            addRootButton.AddToClassList("toolbar-button");
            toolbar.Add(addRootButton);

            var spacer = new VisualElement();
            spacer.style.flexGrow = 1;
            toolbar.Add(spacer);

            _saveButton = new Button(SaveChanges) { text = "💾 Save" };
            _saveButton.AddToClassList("toolbar-button");
            _saveButton.AddToClassList("save-button");
            _saveButton.SetEnabled(false);
            toolbar.Add(_saveButton);

            _revertButton = new Button(RevertChanges) { text = "↺ Revert" };
            _revertButton.AddToClassList("toolbar-button");
            _revertButton.SetEnabled(false);
            toolbar.Add(_revertButton);

            // 搜索栏
            var searchContainer = new VisualElement();
            searchContainer.AddToClassList("search-container");
            
            var searchLabel = new Label("Search:");
            searchLabel.AddToClassList("search-label");
            searchContainer.Add(searchLabel);

            _searchField = new TextField();
            _searchField.AddToClassList("search-field");
            _searchField.RegisterValueChangedCallback(evt =>
            {
                _searchQuery = evt.newValue;
                RefreshTree();
            });
            searchContainer.Add(_searchField);

            var clearButton = new Button(() => 
            {
                _searchField.value = "";
                _searchQuery = "";
                RefreshTree();
            }) { text = "Clear" };
            clearButton.AddToClassList("clear-button");
            searchContainer.Add(clearButton);

            _root.Add(toolbar);
            _root.Add(searchContainer);
        }

        private void OnDatabaseChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            if (_hasUnsavedChanges)
            {
                if (EditorUtility.DisplayDialog("Unsaved Changes",
                    "⚠️ You have unsaved changes!\n\nDo you want to discard them and switch database?",
                    "Discard", "Cancel"))
                {
                    LoadDatabase(evt.newValue as GameplayTagDatabase);
                }
                else
                {
                    _databaseField.SetValueWithoutNotify(_database);
                }
            }
            else
            {
                LoadDatabase(evt.newValue as GameplayTagDatabase);
            }
        }

        private void LoadDatabase(GameplayTagDatabase db)
        {
            _database = db;
            
            if (_database != null)
            {
                CreateWorkingCopy();
                _hasUnsavedChanges = false;
                UpdateButtonStates();
                RefreshTree();
            }
            else
            {
                _workingCopy = null;
                RefreshTree();
            }
        }

        private void CreateWorkingCopy()
        {
            if (_database == null)
            {
                _workingCopy = null;
                return;
            }

            string json = EditorJsonUtility.ToJson(_database);
            _workingCopy = ScriptableObject.CreateInstance<GameplayTagDatabase>();
            EditorJsonUtility.FromJsonOverwrite(json, _workingCopy);
        }

        private void MarkAsModified()
        {
            _hasUnsavedChanges = true;
            UpdateButtonStates();
            UpdateStatus();
        }

        private void UpdateButtonStates()
        {
            if (_saveButton != null)
                _saveButton.SetEnabled(_hasUnsavedChanges && _database != null);
            
            if (_revertButton != null)
                _revertButton.SetEnabled(_hasUnsavedChanges && _database != null);
        }

        private void SaveChanges()
        {
            if (_database == null || _workingCopy == null)
                return;

            string json = EditorJsonUtility.ToJson(_workingCopy);
            EditorJsonUtility.FromJsonOverwrite(json, _database);
            
            EditorUtility.SetDirty(_database);
            AssetDatabase.SaveAssets();
            
            _hasUnsavedChanges = false;
            UpdateButtonStates();
            UpdateStatus();
            
            Debug.Log($"✓ Changes saved to: {AssetDatabase.GetAssetPath(_database)}");
            EditorUtility.DisplayDialog("Saved", 
                $"✅ All changes saved successfully!\n\nTotal Tags: {_database.GetAllTags().Count}", 
                "OK");
        }

        private void RevertChanges()
        {
            if (!_hasUnsavedChanges)
                return;

            if (EditorUtility.DisplayDialog("Revert Changes",
                "⚠️ Discard all unsaved changes?",
                "Revert", "Cancel"))
            {
                CreateWorkingCopy();
                _hasUnsavedChanges = false;
                UpdateButtonStates();
                RefreshTree();
                Debug.Log("✓ Changes reverted");
            }
        }

        private void CreateNewDatabase()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Create Gameplay Tag Database",
                "GameplayTagDatabase",
                "asset",
                "Create a new Gameplay Tag Database",
                "Assets/Resources"
            );

            if (string.IsNullOrEmpty(path))
                return;

            var newDatabase = ScriptableObject.CreateInstance<GameplayTagDatabase>();
            AssetDatabase.CreateAsset(newDatabase, path);
            AssetDatabase.SaveAssets();
            
            _databaseField.value = newDatabase;
            LoadDatabase(newDatabase);
            
            EditorGUIUtility.PingObject(newDatabase);
            Debug.Log($"✓ Database created: {path}");
        }

        public void RefreshTree()
        {
            _treeContainer.Clear();
            
            if (_workingCopy == null)
            {
                var emptyLabel = new Label("📝 No database selected.\n\nDrag a GameplayTagDatabase above or click 'Create New'.");
                emptyLabel.AddToClassList("empty-label");
                _treeContainer.Add(emptyLabel);
                UpdateStatus();
                return;
            }

            var rootNodes = _workingCopy.GetRootNodes();
            
            if (rootNodes.Count == 0)
            {
                var emptyLabel = new Label("📝 No tags defined.\n\nClick '+ Add Root Tag' to create your first tag.");
                emptyLabel.AddToClassList("empty-label");
                _treeContainer.Add(emptyLabel);
            }
            else
            {
                BuildTreeItems(rootNodes, "", 0, _treeContainer);
            }

            UpdateStatus();
        }

        private void BuildTreeItems(List<GameplayTagDatabase.TagNode> nodes, string parentPath, int depth, VisualElement container)
        {
            if (nodes == null)
                return;

            foreach (var node in nodes)
            {
                string fullPath = string.IsNullOrEmpty(parentPath) 
                    ? node.tagName 
                    : parentPath + "." + node.tagName;

                bool matchesFilter = string.IsNullOrEmpty(_searchQuery) || 
                                   fullPath.IndexOf(_searchQuery, System.StringComparison.OrdinalIgnoreCase) >= 0;

                if (!matchesFilter && !HasMatchingChildren(node, fullPath))
                    continue;

                var treeItem = new TagTreeItem(node, fullPath, depth, this);
                container.Add(treeItem);

                if (node.children.Count > 0 && treeItem.IsExpanded)
                {
                    BuildTreeItems(node.children, fullPath, depth + 1, container);
                }
            }
        }

        private bool HasMatchingChildren(GameplayTagDatabase.TagNode node, string parentPath)
        {
            if (string.IsNullOrEmpty(_searchQuery))
                return false;

            foreach (var child in node.children)
            {
                string fullPath = parentPath + "." + child.tagName;
                if (fullPath.IndexOf(_searchQuery, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;

                if (HasMatchingChildren(child, fullPath))
                    return true;
            }

            return false;
        }

        private void UpdateStatus()
        {
            if (_workingCopy == null)
            {
                _statusLabel.text = "❌ No database loaded";
                return;
            }

            var allTags = _workingCopy.GetAllTags();
            string dbName = _database != null ? _database.name : "Working Copy";
            string status = _hasUnsavedChanges ? " | ⚠️ Unsaved Changes" : " | ✅ Saved";
            _statusLabel.text = $"📊 Total Tags: {allTags.Count}  |  📁 {dbName}{status}";
        }

        public void StartRenaming(TagTreeItem item)
        {
            if (_renamingItem != null && _renamingItem != item)
            {
                _renamingItem.CancelRename();
            }
            _renamingItem = item;
        }

        public void FinishRenaming(TagTreeItem item, string newName)
        {
            if (_renamingItem == item)
            {
                _renamingItem = null;
            }
        }

        public void ShowAddTagDialog(string parentPath)
        {
            if (_workingCopy == null)
            {
                EditorUtility.DisplayDialog("Error", "No database loaded!", "OK");
                return;
            }

            string title = string.IsNullOrEmpty(parentPath) ? "Add Root Tag" : $"Add Child Tag";
            string message = string.IsNullOrEmpty(parentPath) 
                ? "Enter the name for the new root tag:" 
                : $"Enter the name for the new child tag under '{parentPath}':";
            
            AddTagDialog.Show(title, message, (tagName) =>
            {
                if (string.IsNullOrEmpty(tagName))
                    return;

                bool success = string.IsNullOrEmpty(parentPath)
                    ? _workingCopy.AddChildTag("", tagName)
                    : _workingCopy.AddChildTag(parentPath, tagName);

                if (success)
                {
                    MarkAsModified();
                    RefreshTree();
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "Failed to add tag. Tag may already exist.", "OK");
                }
            });
        }

        public void DeleteTag(string fullPath, bool hasChildren)
        {
            if (_workingCopy == null)
                return;

            string message = hasChildren
                ? $"Delete tag '{fullPath}' and all its children?"
                : $"Delete tag '{fullPath}'?";
            
            if (EditorUtility.DisplayDialog("Delete Tag", message, "Delete", "Cancel"))
            {
                if (_workingCopy.RemoveTag(fullPath))
                {
                    MarkAsModified();
                    RefreshTree();
                }
            }
        }

        public GameplayTagDatabase GetDatabase()
        {
            return _workingCopy;
        }

        private void LoadStyleSheet()
        {
            var guids = AssetDatabase.FindAssets("GameplayTagManagerWindow t:StyleSheet");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
                if (styleSheet != null)
                {
                    _root.styleSheets.Add(styleSheet);
                }
            }
        }
    }

}

