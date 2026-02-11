using System.Collections.Generic;
using System.Linq;
// ReSharper disable UnusedMember.Global

namespace GraphProcessor
{
    [System.Serializable, NodeMenuItem("Flow/Start")]
    public class StartNode : BaseNode, IConditionalNode
    {
        [Output(name = "Executes"), ExecutionOrder]
        public ConditionalLink executes;
        
        public IEnumerable<ConditionalNode> GetExecutedNodes()
        {
            return ApplyExecutionOrder(this, nameof(executes), GetOutputNodes().OfType<ConditionalNode>());
        }
    }
}