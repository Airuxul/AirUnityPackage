using System.Collections.Generic;

namespace GraphProcessor
{
    /// <summary>
    /// Runtime graph implementation. Built from GraphExportData (JSON deserialization).
    /// </summary>
    public class RuntimeGraph
    {
        /// <summary>
        /// Asset path of the source graph SO. Set from GraphExportData when loading from JSON.
        /// </summary>
        public string SourceGraphPath { get; set; }

        public Dictionary<string, RuntimeBaseNode> Guid2Nodes { get; } = new();
        public Dictionary<string, RuntimeEdge> Guid2Edges { get; } = new();
        public Dictionary<string, object> ExposedParameters { get; } = new();
        
        public Dictionary<(string nodeGUID, string fieldName, string portId), object> PortValues { get; } = new();

        public void AddNode(RuntimeBaseNode node)
        {
            Guid2Nodes.Add(node.GUID, node);
        }

        public void AddEdge(RuntimeEdge edge)
        {
            Guid2Edges.Add(edge.GUID, edge);
        }

        public void SetExposedParameter(string guid, object value)
        {
            ExposedParameters.Add(guid, value);
        }

        public object GetExposedParameter(string guid)
        {
            return ExposedParameters.GetValueOrDefault(guid);
        }
        
        public bool TryGetNode<T>(string nodeGUID, out T node) where T : RuntimeBaseNode
        {
            var hasValue = Guid2Nodes.TryGetValue(nodeGUID, out var baseNode);
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
                PortValues.Remove(key);
            else
                PortValues[key] = value;
        }

        public object GetPortValue(string nodeGUID, string fieldName, string portId)
        {
            return PortValues.GetValueOrDefault((nodeGUID, fieldName, portId ?? ""));
        }
        
        // todo 优化，缓存字典不遍历
        public IEnumerable<RuntimeBaseNode> GetInputNodes(RuntimeBaseNode node)
        {
            foreach (var edge in Guid2Edges.Values)
            {
                if (edge.InputNodeGUID == node.GUID && Guid2Nodes.TryGetValue(edge.OutputNodeGUID, out var outputNode))
                    yield return outputNode;
            }
        }

        public IEnumerable<RuntimeBaseNode> GetOutputNodes(RuntimeBaseNode node)
        {
            foreach (var edge in Guid2Edges.Values)
            {
                if (edge.OutputNodeGUID == node.GUID && Guid2Nodes.TryGetValue(edge.InputNodeGUID, out var inputNode))
                    yield return inputNode;
            }
        }
        
        public RuntimeEdge GetEdgeForInput(RuntimeBaseNode inputNode, string fieldName, string portId, RuntimeBaseNode outputNode)
        {
            foreach (var edge in Guid2Edges.Values)
            {
                if (edge.InputNodeGUID == inputNode.GUID && edge.OutputNodeGUID == outputNode.GUID
                                                         && edge.InputFieldName == fieldName && (portId == null || edge.InputPortIdentifier == portId))
                    return edge;
            }
            return null;
        }

        public RuntimeEdge GetEdgeForOutput(RuntimeBaseNode outputNode, string fieldName, string portId, RuntimeBaseNode inputNode)
        {
            foreach (var edge in Guid2Edges.Values)
            {
                if (edge.OutputNodeGUID == outputNode.GUID && edge.InputNodeGUID == inputNode.GUID
                                                           && edge.OutputFieldName == fieldName && (portId == null || edge.OutputPortIdentifier == portId))
                    return edge;
            }
            return null;
        }
    }
}
