using System;
using GraphProcessor;

namespace Air.BehaviorTree
{
    [Serializable, NodeMenuItem("Behavior Tree/Control/Selector", typeof(BehaviorTreeGraph))]
    public class BTSelectorNode : BTControlNode
    {
        public override Type RuntimeNodeType => typeof(RuntimeBTSelectorNode);

        public override string name => "Selector";
    }
}