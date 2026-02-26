using System;
using GraphProcessor;

namespace Air.BehaviorTree
{
    [Serializable, NodeMenuItem("Behavior Tree/Decorator/Invert", typeof(BehaviorTreeGraph))]
    public class BTInvertNode : BTDecoratorNode
    {
        public override Type RuntimeNodeType => typeof(RuntimeBTInvertNode);
        public override string name => "Invert";
    }
}