namespace Air.BehaviorTree
{
    /// <summary>
    /// Execution status of a behavior tree node.
    /// </summary>
    public enum BTStatus
    {
        /// <summary>Node completed successfully.</summary>
        Success,

        /// <summary>Node failed to complete.</summary>
        Failure,

        /// <summary>Node is still running (e.g. waiting for async operation).</summary>
        Running
    }
}
