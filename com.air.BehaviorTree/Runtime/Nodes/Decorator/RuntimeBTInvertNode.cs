
using GraphProcessor;

namespace Air.BehaviorTree
{
    public class RuntimeBTInvertNode : RuntimeBTDecoratorNode
    {
        public RuntimeBTInvertNode(RuntimeGraph graph, NodeExportData exportData) : base(graph, exportData)
        {
        }

        protected override BehaviorTreeStatus OnUpdate()
        {
            var child = GetChild();
            child.OnProcess();
            switch (child.Status)
            {
                case BehaviorTreeStatus.Running:
                    return child.Status;
                case BehaviorTreeStatus.Success:
                    return BehaviorTreeStatus.Failure;
                case BehaviorTreeStatus.Failure:
                    return BehaviorTreeStatus.Success;
            }

            return BehaviorTreeStatus.Running;
        }
    }
}