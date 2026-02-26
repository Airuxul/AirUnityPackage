using System;
using GraphProcessor;

namespace Air.BehaviorTree
{
    [Serializable, NodeMenuItem("Behavior Tree/Root", typeof(BehaviorTreeGraph))]
    public class RootNode : BaseNode
    {
        public override Type RuntimeNodeType => typeof(RuntimeRootBaseNode);

        public override string name => "Root";

        [Output(name = "Child")]
        public BehaviorTreeStatus output;

        public override bool deletable => false;

        public override string GetExportJsonData() => "{}";
    }
}
