using BehaviorTree.Action;
using GraphProcessor;

namespace BehaviorTree
{
    /// <summary>
    /// Selector node: executes children in order. Succeeds on first success; fails when all fail.
    /// </summary>
    public class RuntimeBTSelectorNode : RuntimeBTControlNode
    {
        private int current;

        public RuntimeBTSelectorNode(RuntimeGraph graph, NodeExportData exportData) : base(graph, exportData)
        {
        }

        protected override void OnInit()
        {
            current = 0;
        }

        protected override BehaviorTreeStatus OnUpdate()
        {
            var children = GetChildren();
            while (current < children.Count)
            {
                var child = children[current];
                child.OnProcess();
                if (child.Status != BehaviorTreeStatus.Failure)
                {
                    return child.Status;
                }
                current++;
            }
            return BehaviorTreeStatus.Failure;
        }
    }
}
