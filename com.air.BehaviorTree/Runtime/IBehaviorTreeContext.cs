using System.Collections.Generic;

namespace Air.BehaviorTree
{
    /// <summary>
    /// Context passed to behavior tree nodes during execution.
    /// Implement this interface to provide game-specific data (blackboard, agent reference, etc.).
    /// </summary>
    public interface IBehaviorTreeContext
    {
    }

    /// <summary>
    /// Context that provides per-node state storage (e.g. for delay timers).
    /// Used by stateful nodes like DelayActionNode.
    /// </summary>
    public interface IBehaviorTreeNodeStateContext : IBehaviorTreeContext
    {
        Dictionary<string, object> NodeState { get; }
    }
}
