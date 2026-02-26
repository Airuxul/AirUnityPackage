using GraphProcessor;

namespace Air.BehaviorTree
{
    public abstract class BTActionNode : BaseNode
    {
        [Input(name = "Parent", allowMultiple = false)] public BehaviorTreeStatus input;
    }
}