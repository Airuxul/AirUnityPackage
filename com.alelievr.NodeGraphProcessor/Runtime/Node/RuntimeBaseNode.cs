using System.Collections.Generic;
using System.Linq;

namespace GraphProcessor
{
    /// <summary>
    /// Base class for runtime nodes. Lightweight version for graph execution,
    /// deserialized from JSON/binary export of BaseGraph.
    /// </summary>
    public abstract class RuntimeBaseNode
    {
        protected readonly RuntimeGraph graph;

        protected RuntimeBaseNode(RuntimeGraph graph)
        {
            this.graph = graph;
        }

        /// <summary>
        /// Unique identifier of the node.
        /// </summary>
        public string GUID { get; set; }

        /// <summary>
        /// Compute order for execution sequence. Higher values execute later.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Get nodes connected to this node's input ports.
        /// </summary>
        public virtual IEnumerable<T> GetInputNodes<T>() where T : RuntimeBaseNode
        {
            return graph.GetInputNodes(this).Select(node => node as T).Where(node => node != null);
        }

        /// <summary>
        /// Get nodes connected to this node's output ports.
        /// </summary>
        public virtual IEnumerable<T> GetOutputNodes<T>() where T : RuntimeBaseNode
        {
            return graph.GetOutputNodes(this).Select(node => node as T).Where(node => node != null);
        }

        /// <summary>
        /// Execute node processing. Called by ProcessGraphProcessor in compute order.
        /// </summary>
        public abstract void OnProcess();

        public object GetInputValue(string fieldName, string portId = null)
        {
            foreach (var outputNode in GetInputNodes<RuntimeBaseNode>())
            {
                var edge = graph.GetEdgeForInput(this, fieldName, portId, outputNode);
                if (edge != null)
                    return graph.GetPortValue(GUID, edge.InputFieldName, edge.InputPortIdentifier);
            }

            return null;
        }

        public void SetOutputValue(string fieldName, string portId, object value)
        {
            foreach (var inputNode in GetOutputNodes<RuntimeBaseNode>())
            {
                var edge = graph.GetEdgeForOutput(this, fieldName, portId, inputNode);
                if (edge != null)
                    graph.SetPortValue(inputNode.GUID, edge.InputFieldName, edge.InputPortIdentifier, value);
            }
        }
    }
}