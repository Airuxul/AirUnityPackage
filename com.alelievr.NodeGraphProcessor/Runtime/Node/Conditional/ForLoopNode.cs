using System.Collections.Generic;
using System.Linq;
// ReSharper disable UnusedMember.Global

namespace GraphProcessor
{
    [System.Serializable, NodeMenuItem("Conditional/ForLoop")]
    public class ForLoopNode : ConditionalNode
    {
        [Output(name = "Loop Body"), ExecutionOrder]
        public ConditionalLink loopBody;

        [Output(name = "Loop Completed"), ExecutionOrder]
        public ConditionalLink loopCompleted;

        public int start = 0;
        public int end = 10;

        [Output]
        public int index;

        public override string name => "ForLoop";

        protected override void Process() => index++;

        public override IEnumerable<ConditionalNode> GetExecutedNodes() =>
            throw new System.Exception("Do not use GetExecutedNodes in for loop to get its dependencies");

        public IEnumerable<ConditionalNode> GetExecutedNodesLoopBody()
        {
            var port = outputPorts.FirstOrDefault(n => n.fieldName == nameof(loopBody));
            var nodes = port?.GetEdges().Select(e => e.inputNode as ConditionalNode) ?? Enumerable.Empty<ConditionalNode>();
            return ApplyExecutionOrder(this, nameof(loopBody), nodes);
        }

        public IEnumerable<ConditionalNode> GetExecutedNodesLoopCompleted()
        {
            var port = outputPorts.FirstOrDefault(n => n.fieldName == nameof(loopCompleted));
            var nodes = port?.GetEdges().Select(e => e.inputNode as ConditionalNode) ?? Enumerable.Empty<ConditionalNode>();
            return ApplyExecutionOrder(this, nameof(loopCompleted), nodes);
        }
    }
}
