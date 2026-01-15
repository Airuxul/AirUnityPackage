using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Air.GameplayTag
{
    /// <summary>
    /// Gameplay标签数据库，存储项目中所有可用的标签
    /// </summary>
    [CreateAssetMenu(fileName = "GameplayTagDatabase", menuName = "Gameplay/Gameplay Tag Database")]
    public class GameplayTagDatabase : ScriptableObject
    {
        [Serializable]
        public class TagNode
        {
            public string tagName;
            public string description;
            public List<TagNode> children = new();

            public TagNode(string name)
            {
                tagName = name;
            }
        }

        [SerializeField] private List<TagNode> rootNodes = new();

        private static GameplayTagDatabase _instance;
        private HashSet<string> _allTagsCache;

        /// <summary>
        /// 获取数据库单例实例
        /// </summary>
        public static GameplayTagDatabase Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<GameplayTagDatabase>("GameplayTagDatabase");
                    
                    if (_instance == null)
                    {
                        Debug.LogWarning("GameplayTagDatabase not found in Resources folder. Creating default instance.");
                        _instance = CreateInstance<GameplayTagDatabase>();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 获取所有根节点
        /// </summary>
        public List<TagNode> GetRootNodes()
        {
            return rootNodes;
        }

        /// <summary>
        /// 添加新标签
        /// </summary>
        public bool AddTag(string fullTagName, string description = "")
        {
            if (string.IsNullOrEmpty(fullTagName))
                return false;

            if (TagExists(fullTagName))
                return false;

            string[] parts = fullTagName.Split('.');
            List<TagNode> currentLevel = rootNodes;

            for (int i = 0; i < parts.Length; i++)
            {
                if (i > 0)
                {
                }

                TagNode node = currentLevel.Find(n => n.tagName == parts[i]);
                
                if (node == null)
                {
                    node = new TagNode(parts[i]);
                    if (i == parts.Length - 1)
                        node.description = description;
                    
                    currentLevel.Add(node);
                    currentLevel.Sort((a, b) => string.Compare(a.tagName, b.tagName, StringComparison.Ordinal));
                }

                currentLevel = node.children;
            }

            InvalidateCache();
            return true;
        }

        /// <summary>
        /// 添加子节点到指定父节点
        /// </summary>
        public bool AddChildTag(string parentPath, string childName, string description = "")
        {
            if (string.IsNullOrEmpty(childName))
                return false;

            // 检查子节点名称是否有效（不能包含'.'）
            if (childName.Contains("."))
            {
                Debug.LogError("Child tag name cannot contain '.' separator");
                return false;
            }

            List<TagNode> targetLevel;
            string fullPath;

            if (string.IsNullOrEmpty(parentPath))
            {
                // 添加到根节点
                targetLevel = rootNodes;
                fullPath = childName;
            }
            else
            {
                // 找到父节点
                TagNode parentNode = FindNode(parentPath);
                if (parentNode == null)
                {
                    Debug.LogError($"Parent node not found: {parentPath}");
                    return false;
                }

                targetLevel = parentNode.children;
                fullPath = parentPath + "." + childName;
            }

            // 检查是否已存在
            if (targetLevel.Exists(n => n.tagName == childName))
            {
                Debug.LogWarning($"Tag already exists: {fullPath}");
                return false;
            }

            // 创建新节点
            TagNode newNode = new TagNode(childName)
            {
                description = description
            };

            targetLevel.Add(newNode);
            targetLevel.Sort((a, b) => string.Compare(a.tagName, b.tagName, StringComparison.Ordinal));

            InvalidateCache();
            return true;
        }

        /// <summary>
        /// 重命名标签节点
        /// </summary>
        public bool RenameTag(string fullTagName, string newName)
        {
            if (string.IsNullOrEmpty(fullTagName) || string.IsNullOrEmpty(newName))
                return false;

            // 新名称不能包含'.'
            if (newName.Contains("."))
            {
                Debug.LogError("New name cannot contain '.' separator");
                return false;
            }

            TagNode node = FindNode(fullTagName);
            if (node == null)
                return false;

            // 检查同级是否已存在相同名称
            string[] parts = fullTagName.Split('.');
            List<TagNode> parentLevel;

            if (parts.Length == 1)
            {
                parentLevel = rootNodes;
            }
            else
            {
                string parentPath = string.Join(".", parts, 0, parts.Length - 1);
                TagNode parentNode = FindNode(parentPath);
                parentLevel = parentNode?.children;
            }

            if (parentLevel != null && parentLevel.Exists(n => n.tagName == newName && n != node))
            {
                Debug.LogWarning("A tag with this name already exists at the same level");
                return false;
            }

            node.tagName = newName;
            
            // 重新排序
            if (parentLevel != null)
            {
                parentLevel.Sort((a, b) => string.Compare(a.tagName, b.tagName, StringComparison.Ordinal));
            }

            InvalidateCache();
            return true;
        }

        /// <summary>
        /// 删除标签（包括所有子标签）
        /// </summary>
        public bool RemoveTag(string fullTagName)
        {
            if (string.IsNullOrEmpty(fullTagName))
                return false;

            string[] parts = fullTagName.Split('.');
            List<TagNode> currentLevel = rootNodes;
            List<TagNode> parentLevel = null;
            TagNode nodeToRemove = null;

            for (int i = 0; i < parts.Length; i++)
            {
                TagNode node = currentLevel.Find(n => n.tagName == parts[i]);
                
                if (node == null)
                    return false;

                if (i == parts.Length - 1)
                {
                    nodeToRemove = node;
                    break;
                }

                parentLevel = currentLevel;
                currentLevel = node.children;
            }

            if (nodeToRemove != null)
            {
                (parentLevel ?? rootNodes).Remove(nodeToRemove);
                InvalidateCache();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 检查标签是否存在
        /// </summary>
        public bool TagExists(string fullTagName)
        {
            if (_allTagsCache == null)
                RebuildCache();

            return _allTagsCache != null && _allTagsCache.Contains(fullTagName);
        }

        /// <summary>
        /// 获取所有标签（完整路径）
        /// </summary>
        public List<string> GetAllTags()
        {
            var allTags = new List<string>();
            CollectAllTags(rootNodes, "", allTags);
            return allTags;
        }

        /// <summary>
        /// 获取与查询字符串匹配的标签
        /// </summary>
        public List<string> SearchTags(string query)
        {
            if (string.IsNullOrEmpty(query))
                return GetAllTags();

            var allTags = GetAllTags();
            return allTags.Where(t => t.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
        }

        /// <summary>
        /// 获取指定标签的描述
        /// </summary>
        public string GetTagDescription(string fullTagName)
        {
            TagNode node = FindNode(fullTagName);
            return node?.description ?? "";
        }

        /// <summary>
        /// 设置标签描述
        /// </summary>
        public void SetTagDescription(string fullTagName, string description)
        {
            TagNode node = FindNode(fullTagName);
            if (node != null)
            {
                node.description = description;
            }
        }

        private TagNode FindNode(string fullTagName)
        {
            if (string.IsNullOrEmpty(fullTagName))
                return null;

            string[] parts = fullTagName.Split('.');
            List<TagNode> currentLevel = rootNodes;

            for (int i = 0; i < parts.Length; i++)
            {
                TagNode node = currentLevel.Find(n => n.tagName == parts[i]);
                
                if (node == null)
                    return null;

                if (i == parts.Length - 1)
                    return node;

                currentLevel = node.children;
            }

            return null;
        }

        private void CollectAllTags(List<TagNode> nodes, string parentPath, List<string> output)
        {
            foreach (var node in nodes)
            {
                string fullPath = string.IsNullOrEmpty(parentPath) 
                    ? node.tagName 
                    : parentPath + "." + node.tagName;
                
                output.Add(fullPath);

                if (node.children.Count > 0)
                {
                    CollectAllTags(node.children, fullPath, output);
                }
            }
        }

        private void RebuildCache()
        {
            _allTagsCache = new HashSet<string>(GetAllTags());
        }

        private void InvalidateCache()
        {
            _allTagsCache = null;
        }

        private void OnValidate()
        {
            InvalidateCache();
        }
    }
}

