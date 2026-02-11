using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering;
// ReSharper disable UnusedMember.Global

namespace GraphProcessor
{
    [System.Serializable, NodeMenuItem("Conditional/If"), NodeMenuItem("Conditional/Branch")]
    public class IfNode : ConditionalNode
    {
        [Input(name = "Condition")]
        public bool condition;

        [Output(name = "True"), ExecutionOrder]
        public ConditionalLink	@true;
        [Output(name = "False"), ExecutionOrder]
        public ConditionalLink	@false;

        [Setting("Compare Function")]
        public CompareFunction compareOperator;

        public override string		name => "If";

        public override IEnumerable< ConditionalNode >	GetExecutedNodes()
        {
            string fieldName = condition ? nameof(@true) : nameof(@false);
            var nodes = outputPorts.FirstOrDefault(n => n.fieldName == fieldName)
                ?.GetEdges().Select(e => e.inputNode as ConditionalNode);
            return ApplyExecutionOrder(this, fieldName, nodes ?? Enumerable.Empty<ConditionalNode>());
        }
    }
}
