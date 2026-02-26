namespace Air.BehaviorTree
{
    /// <summary>
    /// Execution status of a behavior tree node.
    /// </summary>
    public enum BehaviorTreeStatus
    {
        /// <summary>Node succeeded.</summary>
        Success,

        /// <summary>Node failed.</summary>
        Failure,

        /// <summary>Node is still running (e.g. delay, async action).</summary>
        Running
    }
}
