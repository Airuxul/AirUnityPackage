using System;
using System.Collections.Generic;

namespace GraphProcessor
{
    /// <summary>
    /// Concrete RuntimeBaseNode with dictionary-based port storage.
    /// Supports ParameterNode (Get/Set) and relay/passthrough.
    /// </summary>
    public class RuntimeDataNode : RuntimeBaseNode
    {
        RuntimeGraph graph;
        public string NodeType { get; set; }
        public string ParameterGUID { get; set; }
        public int ParameterAccessor { get; set; }

        public RuntimeDataNode(RuntimeGraph graph)
        {
            this.graph = graph;
        }

        public override IEnumerable<RuntimeBaseNode> GetInputNodes() => graph.GetInputNodes(this);
        public override IEnumerable<RuntimeBaseNode> GetOutputNodes() => graph.GetOutputNodes(this);

        public object GetInputValue(string fieldName, string portId = null)
        {
            foreach (var outputNode in GetInputNodes())
            {
                var edge = graph.GetEdgeForInput(this, fieldName, portId, outputNode);
                if (edge != null)
                    return graph.GetPortValue(GUID, edge.InputFieldName, edge.InputPortIdentifier);
            }
            return null;
        }

        public void SetOutputValue(string fieldName, string portId, object value)
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
            if (NodeType == "Parameter")
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
            else
            {
                var input = GetInputValue("input", null);
                SetOutputValue("output", null, input);
            }
        }
    }
}
