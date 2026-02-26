using System.Linq;
using GraphProcessor;

namespace Air.BehaviorTree
{
    public abstract class RuntimeBTDecoratorNode : RuntimeBTBaseNode
    {
        private RuntimeBTBaseNode child;

        protected RuntimeBTDecoratorNode(RuntimeGraph graph, NodeExportData exportData) : base(graph, exportData) { }

        protected RuntimeBTBaseNode GetChild()
        {
            if (child != null) return child;
            
            var children = GetOutputNodes<RuntimeBaseNode>()
                .Select(n => n as RuntimeBTBaseNode)
                .Where(n => n != null).OrderBy(node => node.Order).ToList();
            if (children.Count > 1)
                throw new System.Exception("Decorator node can only have one child");
            child = children.FirstOrDefault();

            return child;
        }
    }
}