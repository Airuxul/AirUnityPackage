using System;
using GraphProcessor;

namespace BehaviorTree
{
    [Serializable, NodeMenuItem("Behavior Tree/Sequence", typeof(BehaviorTreeGraph))]
    public class SequenceNode : ControlNode
    {
        public override Type RuntimeNodeType => typeof(RuntimeBTSequenceNode);

        public override string name => "Sequence";

        public override string GetExportJsonData() => "{}";
    }
}
