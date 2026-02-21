using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GraphProcessor;

namespace Air.BehaviorTree
{
    /// <summary>
    /// Base class for all behavior tree nodes. Extends BaseNode with BT execution semantics.
    /// </summary>
    public abstract class BehaviorTreeNode : BaseNode
    {
        /// <summary>
        /// Execute this node. Override in derived classes to implement node logic.
        /// </summary>
        /// <param name="context">Execution context (blackboard, agent, etc.).</param>
        /// <returns>Execution status.</returns>
        public abstract BTStatus Execute(IBehaviorTreeContext context);

        /// <summary>
        /// Get child nodes in execution order (sorted by vertical position in graph).
        /// </summary>
        protected IEnumerable<BehaviorTreeNode> GetOrderedChildren()
        {
            var children = GetOutputNodes()
                .OfType<BehaviorTreeNode>()
                .ToList();

            return children.OrderBy(c => c.position.y).ThenBy(c => c.position.x);
        }

        /// <summary>
        /// Get the first (and typically only) child node. Used by decorators and root.
        /// </summary>
        protected BehaviorTreeNode GetFirstChild()
        {
            return GetOrderedChildren().FirstOrDefault();
        }
    }
}
