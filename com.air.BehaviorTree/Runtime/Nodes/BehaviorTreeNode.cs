using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        /// Execute this node. Records execution start for debug visualization, then calls ExecuteInternal.
        /// </summary>
        public BTStatus Execute(IBehaviorTreeContext context)
        {
            if (context is IBehaviorTreeDebugContext debugContext)
                debugContext.DebugState.RecordExecutionStart(GUID);
            var status = ExecuteInternal(context);
            if (context is IBehaviorTreeDebugContext dc)
                dc.DebugState.RecordExecutionEnd(GUID, status);
            return status;
        }

        /// <summary>
        /// Override in derived classes to implement node logic.
        /// </summary>
        protected abstract BTStatus ExecuteInternal(IBehaviorTreeContext context);

        /// <summary>
        /// Get child nodes in execution order.
        /// When output has [OrderByPosition], children are sorted by Y (smaller Y = earlier).
        /// Otherwise connection order is used.
        /// </summary>
        protected IEnumerable<BehaviorTreeNode> GetOrderedChildren()
        {
            var children = GetOutputNodes()
                .OfType<BehaviorTreeNode>()
                .ToList();

            if (HasOrderByPositionOnOutput())
                return children.OrderBy(c => c.position.y).ThenBy(c => c.position.x);

            return children;
        }

        /// <summary>
        /// Check if any output field has [OrderByPosition] attribute.
        /// </summary>
        bool HasOrderByPositionOnOutput()
        {
            foreach (var field in GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (field.GetCustomAttribute<GraphProcessor.OutputAttribute>() != null &&
                    field.GetCustomAttribute<OrderByPositionAttribute>() != null)
                    return true;
            }
            return false;
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
