using UnityEngine;
using GraphProcessor;

namespace Air.BehaviorTree
{
    /// <summary>
    /// Decorator that waits for a specified duration before executing its child.
    /// Returns Running while waiting, then executes the child and returns its result.
    /// </summary>
    [NodeMenuItem("Behavior Tree/Decorators/Delay", typeof(BehaviorTreeGraph))]
    public class DelayDecoratorNode : DecoratorNode
    {
        [SerializeField, ShowInInspector]
        [Tooltip("Wait duration in seconds before executing child.")]
        float duration = 1f;

        const string StateKeyPrefix = "Wait_";

        public override string name => "Wait";

        public override Color color => new Color(0.7f, 0.6f, 0.9f);

        protected override BTStatus ExecuteInternal(IBehaviorTreeContext context)
        {
            var stateCtx = GetNodeStateContext(context);
            if (stateCtx == null)
                return BTStatus.Failure;

            var firstChild = GetFirstChild();
            if (firstChild == null)
                return BTStatus.Failure;

            var key = StateKeyPrefix + GUID;
            if (!stateCtx.NodeState.TryGetValue(key, out var stateObj))
            {
                stateCtx.NodeState[key] = Time.time;
                return BTStatus.Running;
            }

            var startTime = (float)stateObj;
            var elapsed = Time.time - startTime;
            if (elapsed < duration)
                return BTStatus.Running;

            stateCtx.NodeState.Remove(key);
            return firstChild.Execute(context);
        }

        protected override BTStatus Decorate(BTStatus childStatus, IBehaviorTreeContext context)
        {
            return childStatus;
        }

        static IBehaviorTreeNodeStateContext GetNodeStateContext(IBehaviorTreeContext context)
        {
            if (context is IBehaviorTreeNodeStateContext ctx)
                return ctx;
            if (context is IBehaviorTreeDebugContext { InnerContext: IBehaviorTreeNodeStateContext inner })
                return inner;
            return null;
        }
    }
}
