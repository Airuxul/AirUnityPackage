using UnityEngine;
using GraphProcessor;

namespace Air.BehaviorTree
{
    /// <summary>
    /// Selector (Priority) node. Executes children in order until one succeeds.
    /// Returns Success on first Success, Failure if all fail, Running if one is Running.
    /// </summary>
    [NodeMenuItem("Behavior Tree/Composites/Selector", typeof(BehaviorTreeGraph))]
    public class SelectorNode : BehaviorTreeNode
    {
        [Input(name = "Parent")]
        public object parent;

        [Output(name = "Children", allowMultiple = true)]
        public object children;

        public override string name => "Selector";

        public override Color color => new Color(0.4f, 0.6f, 0.9f);

        public override BTStatus Execute(IBehaviorTreeContext context)
        {
            foreach (var child in GetOrderedChildren())
            {
                var status = child.Execute(context);
                if (status == BTStatus.Success)
                    return BTStatus.Success;
                if (status == BTStatus.Running)
                    return BTStatus.Running;
            }
            return BTStatus.Failure;
        }
    }
}
