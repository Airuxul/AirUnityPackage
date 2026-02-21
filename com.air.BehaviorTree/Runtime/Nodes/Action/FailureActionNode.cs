using UnityEngine;
using GraphProcessor;

namespace Air.BehaviorTree
{
    /// <summary>
    /// Action that always returns Failure. Useful for testing and placeholders.
    /// </summary>
    [NodeMenuItem("Behavior Tree/Actions/Failure", typeof(BehaviorTreeGraph))]
    public class FailureActionNode : ActionNode
    {
        public override string name => "Failure";

        protected override BTStatus OnExecute(IBehaviorTreeContext context)
        {
            return BTStatus.Failure;
        }
    }
}
