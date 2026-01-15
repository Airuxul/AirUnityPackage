using System;
using System.Collections.Generic;
using UnityEngine;

namespace Air.GameplayTag
{
    /// <summary>
    /// 单个Gameplay标签，支持层级结构（使用'.'分隔符）
    /// 例如：Character.State.Stunned
    /// </summary>
    [Serializable]
    public struct GameplayTag : IEquatable<GameplayTag>, IComparable<GameplayTag>
    {
        [SerializeField] private string tagName;

        public string TagName => tagName;

        public bool IsValid => !string.IsNullOrEmpty(tagName);

        public GameplayTag(string name)
        {
            tagName = name;
        }

        /// <summary>
        /// 检查此标签是否与目标标签完全匹配
        /// </summary>
        public bool MatchesExact(GameplayTag other)
        {
            return tagName == other.tagName;
        }

        /// <summary>
        /// 检查此标签是否与目标标签或其任何父标签匹配
        /// 例如：Character.State.Stunned 匹配 Character.State 和 Character
        /// </summary>
        public bool MatchesTag(GameplayTag other)
        {
            if (string.IsNullOrEmpty(other.tagName))
                return false;

            return tagName == other.tagName || tagName.StartsWith(other.tagName + ".");
        }

        /// <summary>
        /// 获取父标签
        /// 例如：Character.State.Stunned -> Character.State
        /// </summary>
        public GameplayTag GetParentTag()
        {
            if (string.IsNullOrEmpty(tagName))
                return new GameplayTag();

            int lastDotIndex = tagName.LastIndexOf('.');
            if (lastDotIndex <= 0)
                return new GameplayTag();

            return new GameplayTag(tagName.Substring(0, lastDotIndex));
        }

        /// <summary>
        /// 获取所有父标签（从根到直接父标签）
        /// </summary>
        public List<GameplayTag> GetParentTags()
        {
            var parents = new List<GameplayTag>();
            if (string.IsNullOrEmpty(tagName))
                return parents;

            string[] parts = tagName.Split('.');
            string currentPath = "";

            for (int i = 0; i < parts.Length - 1; i++)
            {
                if (i > 0)
                    currentPath += ".";
                currentPath += parts[i];
                parents.Add(new GameplayTag(currentPath));
            }

            return parents;
        }

        /// <summary>
        /// 获取标签的层级深度
        /// </summary>
        public int GetDepth()
        {
            if (string.IsNullOrEmpty(tagName))
                return 0;

            return tagName.Split('.').Length;
        }

        public bool Equals(GameplayTag other)
        {
            return tagName == other.tagName;
        }

        public override bool Equals(object obj)
        {
            return obj is GameplayTag other && Equals(other);
        }

        public override int GetHashCode()
        {
            return tagName?.GetHashCode() ?? 0;
        }

        public int CompareTo(GameplayTag other)
        {
            return string.Compare(tagName, other.tagName, StringComparison.Ordinal);
        }

        public override string ToString()
        {
            return tagName ?? "None";
        }

        public static bool operator ==(GameplayTag lhs, GameplayTag rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(GameplayTag lhs, GameplayTag rhs)
        {
            return !lhs.Equals(rhs);
        }

        public static readonly GameplayTag Empty = new GameplayTag(string.Empty);
    }
}


