using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace GraphProcessor
{
    /// <summary>
    /// Builds RuntimeGraph from GraphExportData (deserialized from JSON or binary).
    /// </summary>
    public static class RuntimeGraphBuilder
    {
        const uint BinaryMagic = 0x47504752; // "GPGR"

        /// <summary>
        /// Register a custom node creator. Call from [RuntimeInitializeOnLoadMethod] in dependent packages.
        /// </summary>
        public static void RegisterNodeCreator(Type runtimeNodeType, Func<RuntimeGraph, NodeExportData, RuntimeBaseNode> creator)
        {
            if (runtimeNodeType == null || creator == null) return;
            _customCreators[runtimeNodeType] = creator;
        }

        private static readonly Dictionary<Type, Func<RuntimeGraph, NodeExportData, RuntimeBaseNode>> _customCreators = new();

        /// <summary>
        /// Load RuntimeGraph from JSON string.
        /// </summary>
        public static RuntimeGraph FromJson(string json)
        {
            var data = JsonUtility.FromJson<GraphExportData>(json);
            return Build(data);
        }

        /// <summary>
        /// Load RuntimeGraph from binary stream (magic + length + JSON UTF-8).
        /// </summary>
        public static RuntimeGraph FromBinary(Stream stream)
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                var magic = reader.ReadUInt32();
                if (magic != BinaryMagic)
                    throw new InvalidDataException($"Invalid graph binary magic: 0x{magic:X8}");
                var length = reader.ReadInt32();
                var bytes = reader.ReadBytes(length);
                var json = Encoding.UTF8.GetString(bytes);
                return FromJson(json);
            }
        }

        /// <summary>
        /// Load RuntimeGraph from binary bytes.
        /// </summary>
        public static RuntimeGraph FromBinary(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
                return FromBinary(ms);
        }

        /// <summary>
        /// Build RuntimeGraph from GraphExportData.
        /// </summary>
        public static RuntimeGraph Build(GraphExportData data)
        {
            return BuildInto(data, new RuntimeGraph());
        }

        /// <summary>
        /// Build into an existing RuntimeGraph. Use for custom graph subclasses.
        /// </summary>
        public static T BuildInto<T>(GraphExportData data, T graph) where T : RuntimeGraph
        {
            foreach (var param in data.exposedParameters)
            {
                var value = DeserializeParameterValue(param.type, param.jsonValue);
                graph.SetExposedParameter(param.guid, value);
            }

            foreach (var nodeData in data.nodes)
            {
                var node = CreateNode(graph, nodeData);
                graph.AddNode(node);
            }

            foreach (var edgeData in data.edges)
            {
                graph.AddEdge(new RuntimeEdge
                {
                    GUID = edgeData.guid,
                    InputNodeGUID = edgeData.inputNodeGUID,
                    OutputNodeGUID = edgeData.outputNodeGUID,
                    InputFieldName = edgeData.inputFieldName,
                    OutputFieldName = edgeData.outputFieldName,
                    InputPortIdentifier = edgeData.inputPortIdentifier,
                    OutputPortIdentifier = edgeData.outputPortIdentifier
                });
            }

            return graph;
        }

        static RuntimeBaseNode CreateNode(RuntimeGraph graph, NodeExportData data)
        {
            var runtimeType = Type.GetType(data.runtimeNodeType ?? "");
            if (runtimeType != null && _customCreators.TryGetValue(runtimeType, out var creator))
            {
                var node = creator(graph, data);
                if (node != null) return node;
            }
            if (runtimeType == typeof(RuntimeParameterNode))
            {
                var paramData = JsonUtility.FromJson<ParameterNodeExportData>(data.jsonData ?? "{}");
                return new RuntimeParameterNode(graph)
                {
                    GUID = data.guid,
                    ComputeOrder = data.computeOrder,
                    ParameterGUID = paramData.parameterGUID,
                    ParameterAccessor = paramData.accessor
                };
            }
            if (runtimeType == typeof(RuntimeRelayNode))
            {
                var relayData = JsonUtility.FromJson<RelayNodeExportData>(data.jsonData ?? "{}");
                return new RuntimeRelayNode(graph)
                {
                    GUID = data.guid,
                    ComputeOrder = data.computeOrder,
                    PackInput = relayData.packInput,
                    UnpackOutput = relayData.unpackOutput
                };
            }
            var fallback = new RuntimeDataNode(graph)
            {
                GUID = data.guid,
                ComputeOrder = data.computeOrder,
                NodeType = InferNodeType(data)
            };
            if (fallback.NodeType == "Parameter")
            {
                var paramData = JsonUtility.FromJson<ParameterNodeExportData>(data.jsonData ?? "{}");
                fallback.ParameterGUID = paramData.parameterGUID;
                fallback.ParameterAccessor = paramData.accessor;
            }
            return fallback;
        }

        static string InferNodeType(NodeExportData data)
        {
            var typeName = data.type ?? "";
            if (typeName.Contains("ParameterNode")) return "Parameter";
            if (typeName.Contains("RelayNode")) return "Relay";
            return "Relay";
        }

        static object DeserializeParameterValue(string typeName, string jsonValue)
        {
            if (string.IsNullOrEmpty(jsonValue)) return null;
            try
            {
                var type = Type.GetType(typeName);
                if (type == null) return null;
                if (type == typeof(float)) return float.Parse(jsonValue.Trim('"'));
                if (type == typeof(int)) return int.Parse(jsonValue.Trim('"'));
                if (type == typeof(bool)) return bool.Parse(jsonValue.Trim('"'));
                if (type == typeof(string)) return jsonValue.Trim('"');
                return JsonUtility.FromJson(jsonValue, type);
            }
            catch { return null; }
        }

        [Serializable]
        class ParameterNodeExportData
        {
            public string parameterGUID;
            public int accessor;
        }

        [Serializable]
        class RelayNodeExportData
        {
            public bool packInput;
            public bool unpackOutput;
        }
    }
}
