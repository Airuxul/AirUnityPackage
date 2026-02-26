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
            graph.SourceGraphPath = data.sourceGraphPath ?? "";

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
            return null;
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
        
        #region auto generated
        /// <summary>
        /// Register a custom node creator. Call from [RuntimeInitializeOnLoadMethod] in dependent packages.
        /// </summary>
        public static void RegisterNodeCreator(Type runtimeNodeType, Func<RuntimeGraph, NodeExportData, RuntimeBaseNode> creator)
        {
            if (runtimeNodeType == null || creator == null) return;
            _customCreators[runtimeNodeType] = creator;
        }

        private static readonly Dictionary<Type, Func<RuntimeGraph, NodeExportData, RuntimeBaseNode>> _customCreators = new()
        {
            {typeof(RuntimeParameterNode), CreateRuntimeParameterNode},
            {typeof(RuntimeRelayNode), CreateRuntimeRelayNode},
            {typeof(RuntimeLogNode), CreateRuntimeLogNode},
        };

        static RuntimeParameterNode CreateRuntimeParameterNode(RuntimeGraph graph, NodeExportData nodeExportData)
        {
            return new RuntimeParameterNode(graph, nodeExportData);
        }

        static RuntimeRelayNode CreateRuntimeRelayNode(RuntimeGraph graph, NodeExportData nodeExportData)
        {
            return new RuntimeRelayNode(graph, nodeExportData);
        }
        
        static RuntimeLogNode CreateRuntimeLogNode(RuntimeGraph graph, NodeExportData nodeExportData)
        {
            return new RuntimeLogNode(graph, nodeExportData);
        }
        #endregion
    }
}
