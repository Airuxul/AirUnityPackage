using UnityEngine;
using GraphProcessor;

namespace Air.BehaviorTree
{
    /// <summary>
    /// Sequence node. Executes children in order until one fails.
    /// Returns Success if all succeed, Failure on first Failure, Running if one is Running.
    /// </summary>
    [NodeMenuItem("Behavior Tree/Composites/Sequence", typeof(BehaviorTreeGraph))]
    public class SequenceNode : BehaviorTreeNode
    {
        [Input(name = "Parent")]
        public object parent;

        [Output(name = "Children", allowMultiple = true)]
        public object children;

        public override string name => "Sequence";

        public override Color color => new Color(0.9f, 0.6f, 0.4f);

        public override BTStatus Execute(IBehaviorTreeContext context)
        {
            foreach (var child in GetOrderedChildren())
            {
                var status = child.Execute(context);
                if (status == BTStatus.Failure)
                    return BTStatus.Failure;
                if (status == BTStatus.Running)
                    return BTStatus.Running;
            }
            return BTStatus.Success;
        }
    }
}
