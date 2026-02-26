namespace GraphProcessor
{
    /// <summary>
    /// Runtime node for ParameterNode. Handles Get/Set exposed parameters.
    /// </summary>
    public class RuntimeParameterNode : RuntimeBaseNodeWithParam<ParameterNodeParamData>
    {
        public RuntimeParameterNode(RuntimeGraph graph, NodeExportData exportData) : base(graph, exportData) { }

        public override void OnProcess()
        {
            if (ParamData.Accessor == 0)
            {
                var val = graph.GetExposedParameter(ParamData.ParameterGUID);
                SetOutputValue("output", "output", val);
            }
            else
            {
                var val = GetInputValue("input", "input");
                graph.SetExposedParameter(ParamData.ParameterGUID, val);
            }
        }
    }
}