namespace GraphProcessor
{
    /// <summary>
    /// Runtime node for ParameterNode. Handles Get/Set exposed parameters.
    /// </summary>
    public class RuntimeParameterNode : RuntimeBaseNode
    {
        public RuntimeParameterNode(RuntimeGraph graph, NodeExportData exportData) : base(graph, exportData)
        {
            var nodeParamData = GetNodeParamDataFromJson<ParameterNodeParamData>(exportData.jsonData ?? "{}");
            ParameterGUID = nodeParamData.ParameterGUID;
            ParameterAccessor = nodeParamData.Accessor;
        }

        public string ParameterGUID { get; set; }
        public int ParameterAccessor { get; set; }

        public override void OnProcess()
        {
            if (ParameterAccessor == 0)
            {
                var val = graph.GetExposedParameter(ParameterGUID);
                SetOutputValue("output", "output", val);
            }
            else
            {
                var val = GetInputValue("input", "input");
                graph.SetExposedParameter(ParameterGUID, val);
            }
        }
    }
}