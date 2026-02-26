namespace GraphProcessor
{
    /// <summary>
    /// Generic base for runtime nodes that load parameters from JSON export data.
    /// Eliminates repeated GetNodeParamDataFromJson and property assignment in derived constructors.
    /// </summary>
    public abstract class RuntimeBaseNodeWithParam<TParamData> : RuntimeBaseNode where TParamData : NodeParamData
    {
        /// <summary>
        /// Deserialized parameter data from node export JSON. Use in derived OnProcess or other logic.
        /// </summary>
        public TParamData ParamData { get; }

        protected RuntimeBaseNodeWithParam(RuntimeGraph graph, NodeExportData exportData) : base(graph, exportData)
        {
            ParamData = GetNodeParamDataFromJson<TParamData>(exportData.jsonData ?? "{}");
        }
    }
}
