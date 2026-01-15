using UnityEngine;

namespace Air.GameplayTag
{
    /// <summary>
    /// GameplayTag扩展方法
    /// </summary>
    public static class GameplayTagExtensions
    {
        /// <summary>
        /// 从字符串创建GameplayTag
        /// </summary>
        public static GameplayTag ToGameplayTag(this string tagName)
        {
            return new GameplayTag(tagName);
        }

        /// <summary>
        /// 检查GameObject是否拥有指定的标签组件
        /// </summary>
        public static bool HasGameplayTag(this GameObject obj, GameplayTag tag)
        {
            return obj.TryGetComponent<GameplayTagComponent>(out var component) && component.HasTag(tag);
        }

        /// <summary>
        /// 检查GameObject是否拥有任意指定的标签
        /// </summary>
        public static bool HasAnyGameplayTags(this GameObject obj, GameplayTagContainer tags)
        {
            return obj.TryGetComponent<GameplayTagComponent>(out var component) && component.HasAnyTags(tags);
        }

        /// <summary>
        /// 检查GameObject是否拥有所有指定的标签
        /// </summary>
        public static bool HasAllGameplayTags(this GameObject obj, GameplayTagContainer tags)
        {
            return obj.TryGetComponent<GameplayTagComponent>(out var component) && component.HasAllTags(tags);
        }

        /// <summary>
        /// 为GameObject添加标签
        /// </summary>
        public static void AddGameplayTag(this GameObject obj, GameplayTag tag)
        {
            if (!obj.TryGetComponent<GameplayTagComponent>(out var tagComponent))
            {
                tagComponent = obj.AddComponent<GameplayTagComponent>();
            }
            tagComponent.AddTag(tag);
        }

        /// <summary>
        /// 从GameObject移除标签
        /// </summary>
        public static void RemoveGameplayTag(this GameObject obj, GameplayTag tag)
        {
            var tagComponent = obj.GetComponent<GameplayTagComponent>();
            if (tagComponent)
                tagComponent.RemoveTag(tag);
        }

        /// <summary>
        /// 获取GameObject的所有标签
        /// </summary>
        public static GameplayTagContainer GetGameplayTags(this GameObject obj)
        {
            var tagComponent = obj.GetComponent<GameplayTagComponent>();
            return tagComponent?.GetTags() ?? new GameplayTagContainer();
        }
    }
}


