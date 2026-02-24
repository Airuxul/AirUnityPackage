using System.Collections.Generic;

namespace GraphProcessor
{
    /// <summary>
    /// Runtime node for ParameterNode. Handles Get/Set exposed parameters.
    /// </summary>
    public class RuntimeParameterNode : RuntimeBaseNode
    {
        readonly RuntimeGraph graph;
        public string ParameterGUID { get; set; }
        public int ParameterAccessor { get; set; }

        public RuntimeParameterNode(RuntimeGraph graph)
        {
            this.graph = graph;
        }

        public override IEnumerable<RuntimeBaseNode> GetInputNodes() => graph.GetInputNodes(this);
        public override IEnumerable<RuntimeBaseNode> GetOutputNodes() => graph.GetOutputNodes(this);

        object GetInputValue(string fieldName, string portId)
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
            if (ParameterAccessor == 0)
            {
                var val = graph.GetExposedParameter(ParameterGUID);
                SetOutputValue("output", "output", val);
            }
            else
            {
                var val = GetInputValue("input", "input");
                graph.SetExposedParameter(ParameterGUID, val);
            }
        }
    }
}
