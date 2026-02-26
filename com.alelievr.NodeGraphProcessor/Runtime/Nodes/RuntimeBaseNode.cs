using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GraphProcessor
{
    /// <summary>
    /// Base class for runtime nodes. Lightweight version for graph execution,
    /// deserialized from JSON/binary export of BaseGraph.
    /// </summary>
    public abstract class RuntimeBaseNode
    {
        Dictionary<(string, string), Action<object>> _portSetters;

        protected readonly RuntimeGraph graph;

        protected RuntimeBaseNode(RuntimeGraph graph, NodeExportData exportData)
        {
            this.graph = graph;
            GUID = exportData.guid;
            Order = exportData.computeOrder;
        }

        /// <summary>
        /// Register an input port. When upstream output changes, setter is invoked with the converted value.
        /// Call from derived constructor for each input port that receives from connected nodes.
        /// </summary>
        protected void RegisterInputPort<T>(string fieldName, Action<T> setter, string portId = null)
        {
            _portSetters ??= new Dictionary<(string, string), Action<object>>();
            var key = (fieldName, portId ?? "");
            if (_portSetters.Count == 0)
                graph.PortValueChanged += OnPortValueChanged;

            _portSetters[key] = obj =>
            {
                var converted = ConvertValue(obj, typeof(T));
                if (converted != null)
                    setter((T)converted);
            };
        }

        void OnPortValueChanged(string nodeGUID, string fieldName, string portId, object value)
        {
            if (nodeGUID != GUID) return;
            if (_portSetters == null || !_portSetters.TryGetValue((fieldName, portId ?? ""), out var setter)) return;
            setter(value);
        }

        static object ConvertValue(object value, Type targetType)
        {
            if (value == null) return null;
            var sourceType = value.GetType();
            if (targetType.IsAssignableFrom(sourceType))
                return value;
            try
            {
                if (targetType.IsEnum)
                    return Enum.ToObject(targetType, Convert.ChangeType(value, Enum.GetUnderlyingType(targetType)));
                return Convert.ChangeType(value, targetType);
            }
            catch { return null; }
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
        public IEnumerable<T> GetInputNodes<T>() where T : RuntimeBaseNode
        {
            return graph.GetInputNodes(this).Select(node => node as T).Where(node => node != null);
        }

        /// <summary>
        /// Get nodes connected to this node's output ports.
        /// </summary>
        public IEnumerable<T> GetOutputNodes<T>() where T : RuntimeBaseNode
        {
            return graph.GetOutputNodes(this).Select(node => node as T).Where(node => node != null);
        }

        /// <summary>
        /// Execute node processing. Called by ProcessGraphProcessor in compute order.
        /// </summary>
        public abstract void OnProcess();

        /// <summary>
        /// Gets the value from the connected input port.
        /// </summary>
        public object GetInputValue(string fieldName, string portId = null)
        {
            return graph.GetPortValue(GUID, fieldName, portId ?? "");
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

        public T GetNodeParamDataFromJson<T>(string jsonStr) where T : NodeParamData
        {
            return JsonUtility.FromJson<T>(jsonStr);
        }
    }
}
