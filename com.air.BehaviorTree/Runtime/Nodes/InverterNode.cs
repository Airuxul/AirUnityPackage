using UnityEngine;
using GraphProcessor;

namespace Air.BehaviorTree
{
    /// <summary>
    /// Inverts the child result: Success becomes Failure, Failure becomes Success, Running stays Running.
    /// </summary>
    [NodeMenuItem("Behavior Tree/Decorators/Inverter", typeof(BehaviorTreeGraph))]
    public class InverterNode : DecoratorNode
    {
        public override string name => "Inverter";

        public override Color color => new Color(0.8f, 0.5f, 0.8f);

        protected override BTStatus Decorate(BTStatus childStatus, IBehaviorTreeContext context)
        {
            switch (childStatus)
            {
                case BTStatus.Success: return BTStatus.Failure;
                case BTStatus.Failure: return BTStatus.Success;
                default: return childStatus;
            }
        }
    }
}
