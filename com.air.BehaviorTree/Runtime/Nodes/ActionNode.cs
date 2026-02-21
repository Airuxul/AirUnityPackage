using UnityEngine;
using GraphProcessor;

namespace Air.BehaviorTree
{
    /// <summary>
    /// Base class for leaf (action) nodes. Actions have no children and implement the actual behavior.
    /// </summary>
    public abstract class ActionNode : BehaviorTreeNode
    {
        [Input(name = "Parent")]
        public object parent;

        public override string name => "Action";

        public override Color color => new Color(0.7f, 0.7f, 0.5f);

        /// <summary>
        /// Override to implement the action logic.
        /// </summary>
        protected abstract BTStatus OnExecute(IBehaviorTreeContext context);

        public override BTStatus Execute(IBehaviorTreeContext context)
        {
            return OnExecute(context);
        }
    }
}
