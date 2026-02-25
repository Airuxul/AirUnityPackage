using System;
using GraphProcessor;

namespace BehaviorTree
{
    [Serializable, NodeMenuItem("Behavior Tree/Selector", typeof(BehaviorTreeGraph))]
    public class SelectorNode : ControlNode
    {
        public override Type RuntimeNodeType => typeof(RuntimeBTSelectorNode);

        public override string name => "Selector";

        public override string GetExportJsonData() => "{}";
    }
}
