using System.Collections.Generic;

namespace GraphProcessor
{
    /// <summary>
    /// Runtime graph implementation. Built from GraphExportData (JSON deserialization).
    /// </summary>
    public class RuntimeGraph : IRuntimeGraph
    {
        readonly List<RuntimeBaseNode> nodes = new();
        readonly List<RuntimeEdge> edges = new();
        readonly Dictionary<string, RuntimeBaseNode> nodesByGUID = new();
        readonly Dictionary<string, RuntimeEdge> edgesByGUID = new();
        readonly Dictionary<string, object> exposedParameters = new();
        readonly Dictionary<(string nodeGUID, string fieldName, string portId), object> portValues = new();

        public IReadOnlyList<RuntimeBaseNode> Nodes => nodes;

        public void AddNode(RuntimeBaseNode node)
        {
            nodes.Add(node);
            nodesByGUID[node.GUID] = node;
        }

        public void AddEdge(RuntimeEdge edge)
        {
            edges.Add(edge);
            edgesByGUID[edge.GUID] = edge;
        }

        public void SetExposedParameter(string guid, object value)
        {
            exposedParameters[guid] = value;
        }

        public object GetExposedParameter(string guid)
        {
            return exposedParameters.GetValueOrDefault(guid);
        }
        
        public bool TryGetNode<T>(string nodeGUID, out T node) where T : RuntimeBaseNode
        {
            var hasValue = nodesByGUID.TryGetValue(nodeGUID, out var baseNode);
            if (hasValue)
                node = (T)baseNode;
            else
                node = null;
            return hasValue;
        }

        public void SetPortValue(string nodeGUID, string fieldName, string portId, object value)
        {
            var key = (nodeGUID, fieldName, portId ?? "");
            if (value == null)
                portValues.Remove(key);
            else
                portValues[key] = value;
        }

        public object GetPortValue(string nodeGUID, string fieldName, string portId)
        {
            return portValues.GetValueOrDefault((nodeGUID, fieldName, portId ?? ""));
        }

        public IEnumerable<RuntimeBaseNode> GetInputNodes(RuntimeBaseNode node)
        {
            foreach (var edge in edges)
            {
                if (edge.InputNodeGUID == node.GUID && nodesByGUID.TryGetValue(edge.OutputNodeGUID, out var outputNode))
                    yield return outputNode;
            }
        }

        public IEnumerable<RuntimeBaseNode> GetOutputNodes(RuntimeBaseNode node)
        {
            foreach (var edge in edges)
            {
                if (edge.OutputNodeGUID == node.GUID && nodesByGUID.TryGetValue(edge.InputNodeGUID, out var inputNode))
                    yield return inputNode;
            }
        }

        public RuntimeEdge GetEdgeForInput(RuntimeBaseNode inputNode, string fieldName, string portId, RuntimeBaseNode outputNode)
        {
            foreach (var edge in edges)
            {
                if (edge.InputNodeGUID == inputNode.GUID && edge.OutputNodeGUID == outputNode.GUID
                    && edge.InputFieldName == fieldName && (portId == null || edge.InputPortIdentifier == portId))
                    return edge;
            }
            return null;
        }

        public RuntimeEdge GetEdgeForOutput(RuntimeBaseNode outputNode, string fieldName, string portId, RuntimeBaseNode inputNode)
        {
            foreach (var edge in edges)
            {
                if (edge.OutputNodeGUID == outputNode.GUID && edge.InputNodeGUID == inputNode.GUID
                    && edge.OutputFieldName == fieldName && (portId == null || edge.OutputPortIdentifier == portId))
                    return edge;
            }
            return null;
        }

    }
}
