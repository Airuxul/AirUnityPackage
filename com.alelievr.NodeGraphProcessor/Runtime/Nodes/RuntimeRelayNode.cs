using System.Collections.Generic;

namespace GraphProcessor
{
    /// <summary>
    /// Runtime node for RelayNode. Pass-through relay logic. PackInput/UnpackOutput exported for future use.
    /// </summary>
    public class RuntimeRelayNode : RuntimeBaseNode
    {
        readonly RuntimeGraph graph;

        public bool PackInput { get; set; }
        public bool UnpackOutput { get; set; }

        public RuntimeRelayNode(RuntimeGraph graph)
        {
            this.graph = graph;
        }

        public override IEnumerable<RuntimeBaseNode> GetInputNodes() => graph.GetInputNodes(this);
        public override IEnumerable<RuntimeBaseNode> GetOutputNodes() => graph.GetOutputNodes(this);

        object GetInputValue(string fieldName, string portId = null)
        {
            foreach (var outputNode in GetInputNodes())
            {
                var edge = graph.GetEdgeForInput(this, fieldName, portId, outputNode);
                if (edge != null)
                    return graph.GetPortValue(GUID, edge.InputFieldName, edge.InputPortIdentifier);
            }
            return null;
        }

        void SetOutputValue(string fieldName, string portId, object value)
        {
            foreach (var inputNode in GetOutputNodes())
            {
                var edge = graph.GetEdgeForOutput(this, fieldName, portId, inputNode);
                if (edge != null)
                    graph.SetPortValue(inputNode.GUID, edge.InputFieldName, edge.InputPortIdentifier, value);
            }
        }

        public override void OnProcess()
        {
            var input = GetInputValue("input", null);
            SetOutputValue("output", null, input);
        }
    }
}
