using BehaviorTree.Action;
using GraphProcessor;

namespace BehaviorTree
{
    /// <summary>
    /// Root node of the behavior tree. Entry point; delegates to its single child.
    /// </summary>
    public class RuntimeRootBaseNode : RuntimeBTControlNode
    {
        public RuntimeRootBaseNode(RuntimeGraph graph) : base(graph)
        {
        }

        protected override BehaviorTreeStatus OnUpdate()
        {
            var child = GetChild();
            if (child == null)
                return BehaviorTreeStatus.Failure;

            child.OnProcess();
            return child.Status;
        }
    }
}
