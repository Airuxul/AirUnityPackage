using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Air.GameplayTag
{
    /// <summary>
    /// Gameplay标签容器，用于存储和管理多个GameplayTag
    /// </summary>
    [Serializable]
    public class GameplayTagContainer : IEnumerable<GameplayTag>
    {
        [SerializeField] private List<GameplayTag> tags;

        public int Count => tags.Count;

        public GameplayTagContainer()
        {
            tags = new List<GameplayTag>();
        }

        public GameplayTagContainer(params GameplayTag[] initialTags)
        {
            tags = new List<GameplayTag>(initialTags);
        }

        public GameplayTagContainer(IEnumerable<GameplayTag> initialTags)
        {
            tags = new List<GameplayTag>(initialTags);
        }

        /// <summary>
        /// 添加标签
        /// </summary>
        public void AddTag(GameplayTag tag)
        {
            if (!tag.IsValid || tags.Contains(tag))
                return;

            tags.Add(tag);
        }

        /// <summary>
        /// 添加多个标签
        /// </summary>
        public void AddTags(GameplayTagContainer other)
        {
            if (other == null)
                return;

            foreach (var tag in other.tags)
            {
                AddTag(tag);
            }
        }

        /// <summary>
        /// 移除标签
        /// </summary>
        public bool RemoveTag(GameplayTag tag)
        {
            return tags.Remove(tag);
        }

        /// <summary>
        /// 移除多个标签
        /// </summary>
        public void RemoveTags(GameplayTagContainer other)
        {
            if (other == null)
                return;

            foreach (var tag in other.tags)
            {
                RemoveTag(tag);
            }
        }

        /// <summary>
        /// 清空所有标签
        /// </summary>
        public void Clear()
        {
            tags.Clear();
        }

        /// <summary>
        /// 检查是否包含指定标签（精确匹配）
        /// </summary>
        public bool HasTagExact(GameplayTag tag)
        {
            return tags.Contains(tag);
        }

        /// <summary>
        /// 检查是否包含指定标签或其子标签
        /// </summary>
        public bool HasTag(GameplayTag tag)
        {
            return tags.Any(t => t.MatchesTag(tag));
        }

        /// <summary>
        /// 检查是否包含所有指定的标签
        /// </summary>
        public bool HasAllTags(GameplayTagContainer other)
        {
            if (other == null || other.Count == 0)
                return true;

            return other.tags.All(HasTag);
        }

        /// <summary>
        /// 检查是否包含任意一个指定的标签
        /// </summary>
        public bool HasAnyTags(GameplayTagContainer other)
        {
            if (other == null || other.Count == 0)
                return false;

            return other.tags.Any(HasTag);
        }

        /// <summary>
        /// 获取所有标签
        /// </summary>
        public List<GameplayTag> GetTags()
        {
            return new List<GameplayTag>(tags);
        }

        /// <summary>
        /// 过滤标签
        /// </summary>
        public GameplayTagContainer Filter(Func<GameplayTag, bool> predicate)
        {
            return new GameplayTagContainer(tags.Where(predicate));
        }

        /// <summary>
        /// 检查容器是否为空
        /// </summary>
        public bool IsEmpty()
        {
            return tags.Count == 0;
        }

        public IEnumerator<GameplayTag> GetEnumerator()
        {
            return tags.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            if (tags.Count == 0)
                return "Empty";

            return string.Join(", ", tags.Select(t => t.ToString()));
        }
    }
}


