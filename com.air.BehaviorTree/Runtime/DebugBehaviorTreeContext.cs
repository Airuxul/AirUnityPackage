namespace Air.BehaviorTree
{
    /// <summary>
    /// Wraps an existing context and adds debug state for execution recording.
    /// </summary>
    public class DebugBehaviorTreeContext : IBehaviorTreeDebugContext
    {
        public IBehaviorTreeContext InnerContext { get; }
        public BehaviorTreeDebugState DebugState { get; }

        public DebugBehaviorTreeContext(IBehaviorTreeContext innerContext, BehaviorTreeDebugState debugState)
        {
            InnerContext = innerContext;
            DebugState = debugState;
        }
    }
}
