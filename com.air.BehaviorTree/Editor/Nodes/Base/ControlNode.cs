using GraphProcessor;

namespace BehaviorTree
{
    public abstract class ControlNode : BaseNode
    {
        [Input(name = "Parent")] public BehaviorTreeStatus input;
        [Output(name = "Children")] public BehaviorTreeStatus output;
    }
}