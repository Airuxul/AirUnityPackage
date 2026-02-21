using UnityEngine;
using GraphProcessor;

namespace Air.BehaviorTree
{
    /// <summary>
    /// Root node of the behavior tree. Has exactly one child. Must be unique per graph.
    /// </summary>
    [NodeMenuItem("Behavior Tree/Root", typeof(BehaviorTreeGraph))]
    public class RootNode : BehaviorTreeNode
    {
        [Output(name = "Child", allowMultiple = false)]
        public object child;

        public override string name => "Root";

        public override Color color => new Color(0.2f, 0.6f, 0.2f);

        /// <summary>
        /// Returns true if the root has a child node to execute.
        /// </summary>
        public bool HasChild => GetFirstChild() != null;

        protected override BTStatus ExecuteInternal(IBehaviorTreeContext context)
        {
            var firstChild = GetFirstChild();
            return firstChild != null ? firstChild.Execute(context) : BTStatus.Failure;
        }
    }
}
