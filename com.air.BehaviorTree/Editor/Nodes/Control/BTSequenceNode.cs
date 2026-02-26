using System;
using GraphProcessor;

namespace Air.BehaviorTree
{
    [Serializable, NodeMenuItem("Behavior Tree/Control/Sequence", typeof(BehaviorTreeGraph))]
    public class BTSequenceNode : BTControlNode
    {
        public override Type RuntimeNodeType => typeof(RuntimeBTSequenceNode);

        public override string name => "Sequence";
    }
}
