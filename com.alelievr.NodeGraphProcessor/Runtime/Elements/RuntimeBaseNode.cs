using System.Collections.Generic;

namespace GraphProcessor
{
    /// <summary>
    /// Base class for runtime nodes. Lightweight version for graph execution,
    /// deserialized from JSON/binary export of BaseGraph.
    /// </summary>
    public abstract class RuntimeBaseNode
    {
        /// <summary>
        /// Unique identifier of the node.
        /// </summary>
        public string GUID { get; set; }

        /// <summary>
        /// Compute order for execution sequence. Higher values execute later.
        /// </summary>
        public int ComputeOrder { get; set; }

        /// <summary>
        /// Whether the node can be processed.
        /// </summary>
        public virtual bool CanProcess => true;

        /// <summary>
        /// Get nodes connected to this node's input ports.
        /// </summary>
        public abstract IEnumerable<RuntimeBaseNode> GetInputNodes();

        /// <summary>
        /// Get nodes connected to this node's output ports.
        /// </summary>
        public abstract IEnumerable<RuntimeBaseNode> GetOutputNodes();

        /// <summary>
        /// Execute node processing. Called by ProcessGraphProcessor in compute order.
        /// </summary>
        public abstract void OnProcess();
    }
}
