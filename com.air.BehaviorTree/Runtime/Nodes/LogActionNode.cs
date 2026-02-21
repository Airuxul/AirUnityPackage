using UnityEngine;
using GraphProcessor;

namespace Air.BehaviorTree
{
    /// <summary>
    /// Example action that logs a message and returns Success. Useful for testing.
    /// </summary>
    [NodeMenuItem("Behavior Tree/Actions/Log", typeof(BehaviorTreeGraph))]
    public class LogActionNode : ActionNode
    {
        [SerializeField, ShowInInspector]
        string message = "Action executed";

        public override string name => "Log";

        protected override BTStatus OnExecute(IBehaviorTreeContext context)
        {
            Debug.Log($"[BehaviorTree] {message}");
            return BTStatus.Success;
        }
    }
}
