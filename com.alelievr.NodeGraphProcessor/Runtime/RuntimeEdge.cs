namespace GraphProcessor
{
    /// <summary>
    /// Lightweight runtime edge representation.
    /// </summary>
    public class RuntimeEdge
    {
        public string GUID { get; set; }
        public string InputNodeGUID { get; set; }
        public string OutputNodeGUID { get; set; }
        public string InputFieldName { get; set; }
        public string OutputFieldName { get; set; }
        public string InputPortIdentifier { get; set; }
        public string OutputPortIdentifier { get; set; }
    }
}
