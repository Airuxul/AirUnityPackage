using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Air.GameplayTag;
using System.Collections.Generic;
using System.Linq;

namespace Air.GameplayTag.Editor
{
    /// <summary>
    /// Gameplay标签管理器窗口 - 支持多配置、手动保存
    /// </summary>
    public class GameplayTagManagerWindow : EditorWindow
    {
        private GameplayTagDatabase database; // 原始数据库
        private GameplayTagDatabase workingCopy; // 工作副本
        private bool hasUnsavedChanges = false;
        
        private VisualElement root;
        private ObjectField databaseField;
        private ScrollView treeScrollView;
        private VisualElement treeContainer;
        private TextField searchField;
        private Label statusLabel;
        private Button saveButton;
        private Button revertButton;
        
        private string searchQuery = "";
        private TagTreeItem renamingItem = null;

        [MenuItem("Window/Gameplay Tag Manager")]
        public static void ShowWindow()
        {
            var window = GetWindow<GameplayTagManagerWindow>("Gameplay Tag Manager");
            window.minSize = new Vector2(600, 450);
            window.Show();
        }

        private void CreateGUI()
        {
            BuildUI();
        }

        private void BuildUI()
        {
            root = rootVisualElement;
            root.style.flexGrow = 1;

            // 加载样式
            LoadStyleSheet();

            // 数据库选择区域
            BuildDatabaseSection();

            // 工具栏
            BuildToolbar();

            // 提示标签
            var tipsLabel = new Label("💡 Drag a Database above | Double-click to rename | Manual Save");
            tipsLabel.AddToClassList("tips-label");
            root.Add(tipsLabel);

            // 树形视图
            treeScrollView = new ScrollView(ScrollViewMode.Vertical);
            treeScrollView.AddToClassList("tree-scroll-view");
            
            treeContainer = new VisualElement();
            treeContainer.AddToClassList("tree-container");
            treeScrollView.Add(treeContainer);
            
            root.Add(treeScrollView);

            // 状态栏
            statusLabel = new Label();
            statusLabel.AddToClassList("status-label");
            root.Add(statusLabel);

            RefreshTree();
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

            databaseField = new ObjectField();
            databaseField.objectType = typeof(GameplayTagDatabase);
            databaseField.style.flexGrow = 1;
            databaseField.RegisterValueChangedCallback(OnDatabaseChanged);
            section.Add(databaseField);

            var createButton = new Button(CreateNewDatabase) { text = "Create New" };
            createButton.style.marginLeft = 8;
            createButton.style.width = 100;
            section.Add(createButton);

            root.Add(section);
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

            saveButton = new Button(SaveChanges) { text = "💾 Save" };
            saveButton.AddToClassList("toolbar-button");
            saveButton.AddToClassList("save-button");
            saveButton.SetEnabled(false);
            toolbar.Add(saveButton);

            revertButton = new Button(RevertChanges) { text = "↺ Revert" };
            revertButton.AddToClassList("toolbar-button");
            revertButton.SetEnabled(false);
            toolbar.Add(revertButton);

            // 搜索栏
            var searchContainer = new VisualElement();
            searchContainer.AddToClassList("search-container");
            
            var searchLabel = new Label("Search:");
            searchLabel.AddToClassList("search-label");
            searchContainer.Add(searchLabel);

            searchField = new TextField();
            searchField.AddToClassList("search-field");
            searchField.RegisterValueChangedCallback(evt =>
            {
                searchQuery = evt.newValue;
                RefreshTree();
            });
            searchContainer.Add(searchField);

            var clearButton = new Button(() => 
            {
                searchField.value = "";
                searchQuery = "";
                RefreshTree();
            }) { text = "Clear" };
            clearButton.AddToClassList("clear-button");
            searchContainer.Add(clearButton);

            root.Add(toolbar);
            root.Add(searchContainer);
        }

        private void OnDatabaseChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            if (hasUnsavedChanges)
            {
                if (EditorUtility.DisplayDialog("Unsaved Changes",
                    "⚠️ You have unsaved changes!\n\nDo you want to discard them and switch database?",
                    "Discard", "Cancel"))
                {
                    LoadDatabase(evt.newValue as GameplayTagDatabase);
                }
                else
                {
                    databaseField.SetValueWithoutNotify(database);
                }
            }
            else
            {
                LoadDatabase(evt.newValue as GameplayTagDatabase);
            }
        }

        private void LoadDatabase(GameplayTagDatabase db)
        {
            database = db;
            
            if (database != null)
            {
                CreateWorkingCopy();
                hasUnsavedChanges = false;
                UpdateButtonStates();
                RefreshTree();
            }
            else
            {
                workingCopy = null;
                RefreshTree();
            }
        }

        private void CreateWorkingCopy()
        {
            if (database == null)
            {
                workingCopy = null;
                return;
            }

            string json = EditorJsonUtility.ToJson(database);
            workingCopy = ScriptableObject.CreateInstance<GameplayTagDatabase>();
            EditorJsonUtility.FromJsonOverwrite(json, workingCopy);
        }

        private void MarkAsModified()
        {
            hasUnsavedChanges = true;
            UpdateButtonStates();
            UpdateStatus();
        }

        private void UpdateButtonStates()
        {
            if (saveButton != null)
                saveButton.SetEnabled(hasUnsavedChanges && database != null);
            
            if (revertButton != null)
                revertButton.SetEnabled(hasUnsavedChanges && database != null);
        }

        private void SaveChanges()
        {
            if (database == null || workingCopy == null)
                return;

            string json = EditorJsonUtility.ToJson(workingCopy);
            EditorJsonUtility.FromJsonOverwrite(json, database);
            
            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();
            
            hasUnsavedChanges = false;
            UpdateButtonStates();
            UpdateStatus();
            
            Debug.Log($"✓ Changes saved to: {AssetDatabase.GetAssetPath(database)}");
            EditorUtility.DisplayDialog("Saved", 
                $"✅ All changes saved successfully!\n\nTotal Tags: {database.GetAllTags().Count}", 
                "OK");
        }

        private void RevertChanges()
        {
            if (!hasUnsavedChanges)
                return;

            if (EditorUtility.DisplayDialog("Revert Changes",
                "⚠️ Discard all unsaved changes?",
                "Revert", "Cancel"))
            {
                CreateWorkingCopy();
                hasUnsavedChanges = false;
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
            
            databaseField.value = newDatabase;
            LoadDatabase(newDatabase);
            
            EditorGUIUtility.PingObject(newDatabase);
            Debug.Log($"✓ Database created: {path}");
        }

        public void RefreshTree()
        {
            treeContainer.Clear();
            
            if (workingCopy == null)
            {
                var emptyLabel = new Label("📝 No database selected.\n\nDrag a GameplayTagDatabase above or click 'Create New'.");
                emptyLabel.AddToClassList("empty-label");
                treeContainer.Add(emptyLabel);
                UpdateStatus();
                return;
            }

            var rootNodes = workingCopy.GetRootNodes();
            
            if (rootNodes.Count == 0)
            {
                var emptyLabel = new Label("📝 No tags defined.\n\nClick '+ Add Root Tag' to create your first tag.");
                emptyLabel.AddToClassList("empty-label");
                treeContainer.Add(emptyLabel);
            }
            else
            {
                BuildTreeItems(rootNodes, "", 0, treeContainer);
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

                bool matchesFilter = string.IsNullOrEmpty(searchQuery) || 
                                   fullPath.IndexOf(searchQuery, System.StringComparison.OrdinalIgnoreCase) >= 0;

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
            if (string.IsNullOrEmpty(searchQuery))
                return false;

            foreach (var child in node.children)
            {
                string fullPath = parentPath + "." + child.tagName;
                if (fullPath.IndexOf(searchQuery, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;

                if (HasMatchingChildren(child, fullPath))
                    return true;
            }

            return false;
        }

        private void UpdateStatus()
        {
            if (workingCopy == null)
            {
                statusLabel.text = "❌ No database loaded";
                return;
            }

            var allTags = workingCopy.GetAllTags();
            string dbName = database != null ? database.name : "Working Copy";
            string status = hasUnsavedChanges ? " | ⚠️ Unsaved Changes" : " | ✅ Saved";
            statusLabel.text = $"📊 Total Tags: {allTags.Count}  |  📁 {dbName}{status}";
        }

        public void StartRenaming(TagTreeItem item)
        {
            if (renamingItem != null && renamingItem != item)
            {
                renamingItem.CancelRename();
            }
            renamingItem = item;
        }

        public void FinishRenaming(TagTreeItem item, string newName)
        {
            if (renamingItem == item)
            {
                renamingItem = null;
            }
        }

        public void ShowAddTagDialog(string parentPath)
        {
            if (workingCopy == null)
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
                    ? workingCopy.AddChildTag("", tagName)
                    : workingCopy.AddChildTag(parentPath, tagName);

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
            if (workingCopy == null)
                return;

            string message = hasChildren
                ? $"Delete tag '{fullPath}' and all its children?"
                : $"Delete tag '{fullPath}'?";
            
            if (EditorUtility.DisplayDialog("Delete Tag", message, "Delete", "Cancel"))
            {
                if (workingCopy.RemoveTag(fullPath))
                {
                    MarkAsModified();
                    RefreshTree();
                }
            }
        }

        public GameplayTagDatabase GetDatabase()
        {
            return workingCopy;
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
                    root.styleSheets.Add(styleSheet);
                }
            }
        }
    }

}

