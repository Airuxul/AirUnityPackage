using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace GraphProcessor
{
    public static class GraphExportUtils
    {
        const string ExportPath = "Assets/GraphExports";
        const uint BinaryMagic = 0x47504752; // "GPGR"

        /// <summary>
        /// Export graph to JSON. Returns the saved path or null if cancelled.
        /// </summary>
        public static string ExportToJson(BaseGraph graph)
        {
            if (graph == null) return null;
            var data = GraphExporter.Export(graph);
            var json = JsonUtility.ToJson(data, true);
            if (!Directory.Exists(ExportPath))
                Directory.CreateDirectory(ExportPath);
            var path = EditorUtility.SaveFilePanel("Export Graph JSON", ExportPath, graph.name + ".json", "json");
            if (string.IsNullOrEmpty(path)) return null;
            File.WriteAllText(path, json);
            AssetDatabase.Refresh();
            Debug.Log($"Graph exported to {path}");
            return path;
        }

        /// <summary>
        /// Export graph to binary. Returns the saved path or null if cancelled.
        /// </summary>
        public static string ExportToBinary(BaseGraph graph)
        {
            if (graph == null) return null;
            var data = GraphExporter.Export(graph);
            var json = JsonUtility.ToJson(data, false);
            var bytes = Encoding.UTF8.GetBytes(json);
            if (!Directory.Exists(ExportPath))
                Directory.CreateDirectory(ExportPath);
            var path = EditorUtility.SaveFilePanel("Export Graph Binary", ExportPath, graph.name + ".bytes", "bytes");
            if (string.IsNullOrEmpty(path)) return null;
            using (var fs = File.Create(path))
            using (var writer = new BinaryWriter(fs, Encoding.UTF8, false))
            {
                writer.Write(BinaryMagic);
                writer.Write(bytes.Length);
                writer.Write(bytes);
            }
            AssetDatabase.Refresh();
            Debug.Log($"Graph exported to {path}");
            return path;
        }
    }
}
