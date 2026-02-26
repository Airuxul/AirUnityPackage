namespace GraphProcessor
{
    /// <summary>
    /// Runtime node for RelayNode. Pass-through relay logic. PackInput/UnpackOutput exported for future use.
    /// </summary>
    public class RuntimeRelayNode : RuntimeBaseNodeWithParam<RelayNodeParamData>
    {
        public RuntimeRelayNode(RuntimeGraph graph, NodeExportData exportData) : base(graph, exportData) { }

        public bool PackInput => ParamData.PackInput;
        public bool UnpackOutput => ParamData.UnpackOutput;

        public override void OnProcess()
        {
            var input = GetInputValue("input");
            SetOutputValue("output", null, input);
        }
    }
}
