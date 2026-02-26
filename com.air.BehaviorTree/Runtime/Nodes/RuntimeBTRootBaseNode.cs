using GraphProcessor;
using UnityEngine;

namespace Air.BehaviorTree
{
    /// <summary>
    /// Root node of the behavior tree. Entry point; delegates to its single child.
    /// </summary>
    public class RuntimeRootBaseNode : RuntimeBTDecoratorNode
    {
        public RuntimeRootBaseNode(RuntimeGraph graph, NodeExportData exportData) : base(graph, exportData)
        {
        }

        protected override BehaviorTreeStatus OnUpdate()
        {
            var child = GetChild();
            if (child == null)
            {
                Debug.LogError("Root node has no child to execute.");
                return BehaviorTreeStatus.Failure;
            }

            child.OnProcess();
            return child.Status;
        }
    }
}
