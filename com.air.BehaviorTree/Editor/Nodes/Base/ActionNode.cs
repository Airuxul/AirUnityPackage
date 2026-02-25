using GraphProcessor;

namespace BehaviorTree
{
    public abstract class ActionNode : BaseNode
    {
        [Input(name = "Parent")] public BehaviorTreeStatus input;
    }
}