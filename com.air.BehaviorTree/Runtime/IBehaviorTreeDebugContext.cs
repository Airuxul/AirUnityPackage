namespace Air.BehaviorTree
{
    /// <summary>
    /// Context that provides debug state for editor visualization.
    /// Used when enableDebug is true on BehaviorTreeRunner.
    /// </summary>
    public interface IBehaviorTreeDebugContext : IBehaviorTreeContext
    {
        BehaviorTreeDebugState DebugState { get; }
        IBehaviorTreeContext InnerContext { get; }
    }
}
