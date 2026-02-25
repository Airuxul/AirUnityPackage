using System.Collections.Generic;
using System.Linq;
using GraphProcessor;

namespace BehaviorTree.Action
{
    public abstract class RuntimeBTControlNode : RuntimeBTBaseNode
    {
        protected RuntimeBTControlNode(RuntimeGraph graph) : base(graph)
        {
        }
                
        private List<RuntimeBTBaseNode> childrenCache;
        
        protected List<RuntimeBTBaseNode> GetChildren()
        {
            childrenCache ??= GetOutputNodes<RuntimeBaseNode>()
                .Select(n => n as RuntimeBTBaseNode)
                .Where(n => n != null).OrderBy(node => node.Order).ToList();

            return childrenCache;
        }
        
        /// <summary>
        /// Get the single child (for decorators or root).
        /// </summary>
        protected RuntimeBTBaseNode GetChild()
        {
            return GetChildren().FirstOrDefault();
        }
    }
}