using UnityEngine;
using GraphProcessor;

namespace Air.BehaviorTree
{
    /// <summary>
    /// Action that always returns Success. Useful for testing and placeholders.
    /// </summary>
    [NodeMenuItem("Behavior Tree/Actions/Success", typeof(BehaviorTreeGraph))]
    public class SuccessActionNode : ActionNode
    {
        public override string name => "Success";

        protected override BTStatus OnExecute(IBehaviorTreeContext context)
        {
            return BTStatus.Success;
        }
    }
}
