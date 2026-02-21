using UnityEngine;
using GraphProcessor;

namespace Air.BehaviorTree
{
    /// <summary>
    /// Base class for decorator nodes. Decorators have exactly one child and modify its behavior.
    /// </summary>
    public abstract class DecoratorNode : BehaviorTreeNode
    {
        [Input(name = "Parent")]
        public object parent;

        [Output(name = "Child")]
        public object child;

        protected override BTStatus ExecuteInternal(IBehaviorTreeContext context)
        {
            var firstChild = GetFirstChild();
            return firstChild != null ? Decorate(firstChild.Execute(context), context) : BTStatus.Failure;
        }

        /// <summary>
        /// Override to modify the child's status.
        /// </summary>
        protected abstract BTStatus Decorate(BTStatus childStatus, IBehaviorTreeContext context);
    }
}
