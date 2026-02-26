using System.Collections.Generic;
using System.Linq;
using GraphProcessor;

namespace Air.BehaviorTree
{
    public abstract class RuntimeBTControlNode : RuntimeBTBaseNode
    {
        private List<RuntimeBTBaseNode> childrenCache;

        protected RuntimeBTControlNode(RuntimeGraph graph, NodeExportData exportData) : base(graph, exportData)
        {
        }

        protected List<RuntimeBTBaseNode> GetChildren()
        {
            childrenCache ??= GetOutputNodes<RuntimeBaseNode>()
                .Select(n => n as RuntimeBTBaseNode)
                .Where(n => n != null).OrderBy(node => node.Order).ToList();
            return childrenCache;
        }
    }
}