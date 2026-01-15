using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Air.GameplayTag
{
    /// <summary>
    /// Gameplay标签查询表达式，用于复杂的标签匹配逻辑
    /// </summary>
    [Serializable]
    public class GameplayTagQuery
    {
        public enum QueryType
        {
            None,           // 无查询
            HasAllTags,     // 必须包含所有指定的标签
            HasAnyTags,     // 必须包含至少一个指定的标签
            HasNoTags,      // 不能包含任何指定的标签
            And,            // 多个查询的与操作
            Or,             // 多个查询的或操作
            Not             // 对查询结果取反
        }

        [SerializeField] private QueryType queryType = QueryType.None;
        [SerializeField] private GameplayTagContainer tags = new();
        [SerializeField] private List<GameplayTagQuery> subQueries = new();

        public QueryType Type => queryType;

        /// <summary>
        /// 创建一个空查询（总是返回true）
        /// </summary>
        public static GameplayTagQuery MakeQuery_None()
        {
            return new GameplayTagQuery { queryType = QueryType.None };
        }

        /// <summary>
        /// 创建"包含所有标签"查询
        /// </summary>
        public static GameplayTagQuery MakeQuery_MatchAllTags(GameplayTagContainer tagsToMatch)
        {
            return new GameplayTagQuery
            {
                queryType = QueryType.HasAllTags,
                tags = new GameplayTagContainer(tagsToMatch.GetTags())
            };
        }

        /// <summary>
        /// 创建"包含任意标签"查询
        /// </summary>
        public static GameplayTagQuery MakeQuery_MatchAnyTags(GameplayTagContainer tagsToMatch)
        {
            return new GameplayTagQuery
            {
                queryType = QueryType.HasAnyTags,
                tags = new GameplayTagContainer(tagsToMatch.GetTags())
            };
        }

        /// <summary>
        /// 创建"不包含任何标签"查询
        /// </summary>
        public static GameplayTagQuery MakeQuery_MatchNoTags(GameplayTagContainer tagsToMatch)
        {
            return new GameplayTagQuery
            {
                queryType = QueryType.HasNoTags,
                tags = new GameplayTagContainer(tagsToMatch.GetTags())
            };
        }

        /// <summary>
        /// 创建"与"查询（所有子查询必须为true）
        /// </summary>
        public static GameplayTagQuery MakeQuery_And(params GameplayTagQuery[] queries)
        {
            return new GameplayTagQuery
            {
                queryType = QueryType.And,
                subQueries = new List<GameplayTagQuery>(queries)
            };
        }

        /// <summary>
        /// 创建"或"查询（至少一个子查询为true）
        /// </summary>
        public static GameplayTagQuery MakeQuery_Or(params GameplayTagQuery[] queries)
        {
            return new GameplayTagQuery
            {
                queryType = QueryType.Or,
                subQueries = new List<GameplayTagQuery>(queries)
            };
        }

        /// <summary>
        /// 创建"非"查询（对结果取反）
        /// </summary>
        public static GameplayTagQuery MakeQuery_Not(GameplayTagQuery query)
        {
            return new GameplayTagQuery
            {
                queryType = QueryType.Not,
                subQueries = new List<GameplayTagQuery> { query }
            };
        }

        /// <summary>
        /// 执行查询，检查标签容器是否满足条件
        /// </summary>
        public bool Matches(GameplayTagContainer container)
        {
            if (container == null)
                return false;

            switch (queryType)
            {
                case QueryType.None:
                    return true;

                case QueryType.HasAllTags:
                    return container.HasAllTags(tags);

                case QueryType.HasAnyTags:
                    return container.HasAnyTags(tags);

                case QueryType.HasNoTags:
                    return !container.HasAnyTags(tags);

                case QueryType.And:
                    return subQueries.All(q => q.Matches(container));

                case QueryType.Or:
                    return subQueries.Any(q => q.Matches(container));

                case QueryType.Not:
                    return subQueries.Count > 0 && !subQueries[0].Matches(container);

                default:
                    return false;
            }
        }

        /// <summary>
        /// 获取查询描述
        /// </summary>
        public string GetDescription()
        {
            switch (queryType)
            {
                case QueryType.None:
                    return "Any";

                case QueryType.HasAllTags:
                    return $"Has All: {tags}";

                case QueryType.HasAnyTags:
                    return $"Has Any: {tags}";

                case QueryType.HasNoTags:
                    return $"Has None: {tags}";

                case QueryType.And:
                    return $"AND ({string.Join(", ", subQueries.Select(q => q.GetDescription()))})";

                case QueryType.Or:
                    return $"OR ({string.Join(", ", subQueries.Select(q => q.GetDescription()))})";

                case QueryType.Not:
                    return subQueries.Count > 0 ? $"NOT ({subQueries[0].GetDescription()})" : "NOT (Empty)";

                default:
                    return "Unknown";
            }
        }

        public override string ToString()
        {
            return GetDescription();
        }
    }
}


