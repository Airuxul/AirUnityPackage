using UnityEngine;

namespace Air.GameplayTag
{
    /// <summary>
    /// GameObject上的GameplayTag组件，用于为对象添加标签
    /// </summary>
    [DisallowMultipleComponent]
    public class GameplayTagComponent : MonoBehaviour
    {
        [SerializeField] private GameplayTagContainer tags = new();

        /// <summary>
        /// 添加标签
        /// </summary>
        public void AddTag(GameplayTag gameplayTag)
        {
            tags.AddTag(gameplayTag);
        }

        /// <summary>
        /// 添加多个标签
        /// </summary>
        public void AddTags(GameplayTagContainer other)
        {
            tags.AddTags(other);
        }

        /// <summary>
        /// 移除标签
        /// </summary>
        public bool RemoveTag(GameplayTag gameplayTag)
        {
            return tags.RemoveTag(gameplayTag);
        }

        /// <summary>
        /// 移除多个标签
        /// </summary>
        public void RemoveTags(GameplayTagContainer other)
        {
            tags.RemoveTags(other);
        }

        /// <summary>
        /// 清空所有标签
        /// </summary>
        public void ClearTags()
        {
            tags.Clear();
        }

        /// <summary>
        /// 检查是否包含指定标签
        /// </summary>
        public bool HasTag(GameplayTag gameplayTag)
        {
            return tags.HasTag(gameplayTag);
        }

        /// <summary>
        /// 检查是否包含所有指定的标签
        /// </summary>
        public bool HasAllTags(GameplayTagContainer other)
        {
            return tags.HasAllTags(other);
        }

        /// <summary>
        /// 检查是否包含任意一个指定的标签
        /// </summary>
        public bool HasAnyTags(GameplayTagContainer other)
        {
            return tags.HasAnyTags(other);
        }

        /// <summary>
        /// 检查查询是否匹配
        /// </summary>
        public bool MatchesQuery(GameplayTagQuery query)
        {
            return query.Matches(tags);
        }

        /// <summary>
        /// 获取所有标签
        /// </summary>
        public GameplayTagContainer GetTags()
        {
            return tags;
        }
    }
}


