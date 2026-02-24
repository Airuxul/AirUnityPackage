using System.IO;

namespace GraphProcessor
{
    /// <summary>
    /// Helper for loading and running RuntimeGraph from JSON or binary.
    /// </summary>
    public static class RuntimeGraphRunner
    {
        /// <summary>
        /// Load graph from file. Supports .json (UTF-8) and .bytes (binary format).
        /// </summary>
        public static RuntimeGraph LoadFromFile(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            if (ext == ".bytes")
            {
                using var fs = File.OpenRead(path);
                return RuntimeGraphBuilder.FromBinary(fs);
            }
            var json = File.ReadAllText(path);
            return RuntimeGraphBuilder.FromJson(json);
        }

        /// <summary>
        /// Load graph from JSON string.
        /// </summary>
        public static RuntimeGraph LoadFromJson(string json)
        {
            return RuntimeGraphBuilder.FromJson(json);
        }

        /// <summary>
        /// Load graph from binary bytes.
        /// </summary>
        public static RuntimeGraph LoadFromBinary(byte[] bytes)
        {
            return RuntimeGraphBuilder.FromBinary(bytes);
        }

        /// <summary>
        /// Run the graph once using ProcessGraphProcessor.
        /// </summary>
        public static void Run(RuntimeGraph graph)
        {
            var processor = new ProcessGraphProcessor(graph);
            processor.Run();
        }
    }
}
