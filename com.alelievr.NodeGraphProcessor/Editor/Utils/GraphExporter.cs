using UnityEngine;
using UnityEditor;

namespace GraphProcessor
{
    /// <summary>
    /// Exports BaseGraph (SO) to GraphExportData for JSON serialization.
    /// </summary>
    public static class GraphExporter
    {
        public static GraphExportData Export(BaseGraph graph)
        {
            var data = new GraphExportData
            {
                sourceGraphPath = AssetDatabase.GetAssetPath(graph)
            };

            graph.Deserialize();
            graph.UpdateComputeOrder();

            foreach (var node in graph.nodes)
            {
                if (node == null) continue;
                data.nodes.Add(ExportNode(node));
            }

            foreach (var edge in graph.edges)
            {
                if (edge.inputNode == null || edge.outputNode == null) continue;
                data.edges.Add(new EdgeExportData
                {
                    guid = edge.GUID,
                    inputNodeGUID = edge.inputNode.GUID,
                    outputNodeGUID = edge.outputNode.GUID,
                    inputFieldName = edge.inputFieldName,
                    outputFieldName = edge.outputFieldName,
                    inputPortIdentifier = edge.inputPortIdentifier,
                    outputPortIdentifier = edge.outputPortIdentifier
                });
            }

            foreach (var param in graph.exposedParameters)
            {
                if (param == null) continue;
                data.exposedParameters.Add(ExportParameter(param));
            }

            return data;
        }

        static NodeExportData ExportNode(BaseNode node)
        {
            var data = new NodeExportData
            {
                guid = node.GUID,
                type = node.GetType().AssemblyQualifiedName,
                runtimeNodeType = node.RuntimeNodeType != null ? node.RuntimeNodeType.AssemblyQualifiedName : "",
                computeOrder = node.computeOrder,
                jsonData = ExportNodeData(node)
            };
            return data;
        }

        static string ExportNodeData(BaseNode node)
        {
            var custom = node.GetExportJsonData();
            if (!string.IsNullOrEmpty(custom)) return custom;
            return "{}";
        }

        static ExposedParameterExportData ExportParameter(ExposedParameter param)
        {
            var paramType = param.GetValueType();
            string jsonValue = "";

            if (param.value != null)
            {
                if (paramType.IsPrimitive || paramType == typeof(string))
                    jsonValue = param.value.ToString();
                else
                    jsonValue = JsonUtility.ToJson(param.value);
            }

            return new ExposedParameterExportData
            {
                guid = param.guid,
                name = param.name,
                type = paramType.AssemblyQualifiedName,
                jsonValue = jsonValue
            };
        }
    }
}
