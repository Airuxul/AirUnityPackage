using System.Collections.Generic;

namespace GraphProcessor
{
    /// <summary>
    /// Interface for runtime graph that can be processed by BaseRuntimeGraphProcessor.
    /// Implemented by RuntimeGraph (built from JSON/binary export of BaseGraph).
    /// </summary>
    public interface IRuntimeGraph
    {
        /// <summary>
        /// All nodes in the graph.
        /// </summary>
        IReadOnlyList<RuntimeBaseNode> Nodes { get; }
    }
}
