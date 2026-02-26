using GraphProcessor;

namespace Air.BehaviorTree
{
    public abstract class BTDecoratorNode : BaseNode
    {
        [Input(name = "Parent")] public BehaviorTreeStatus input;
        [Output(name = "Child", allowMultiple = false)] public BehaviorTreeStatus output;
    }
}