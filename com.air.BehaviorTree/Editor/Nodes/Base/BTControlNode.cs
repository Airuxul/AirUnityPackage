using GraphProcessor;

namespace Air.BehaviorTree
{
    public abstract class BTControlNode : BaseNode
    {
        [Input(name = "Parent", allowMultiple = false)] public BehaviorTreeStatus input;
        [Output(name = "Children")] public BehaviorTreeStatus output;
    }
}