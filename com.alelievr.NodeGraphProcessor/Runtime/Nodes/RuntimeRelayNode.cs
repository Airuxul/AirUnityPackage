namespace GraphProcessor
{
    /// <summary>
    /// Runtime node for RelayNode. Pass-through relay logic. PackInput/UnpackOutput exported for future use.
    /// </summary>
    public class RuntimeRelayNode : RuntimeBaseNode
    {
        public RuntimeRelayNode(RuntimeGraph graph, NodeExportData exportData) : base(graph, exportData)
        {
            var nodeParamData = GetNodeParamDataFromJson<RelayNodeParamData>(exportData.jsonData ?? "{}");
            PackInput = nodeParamData.PackInput;
            UnpackOutput = nodeParamData.UnpackOutput;
        }

        public bool PackInput { get; set; }
        public bool UnpackOutput { get; set; }


        public override void OnProcess()
        {
            var input = GetInputValue("input");
            SetOutputValue("output", null, input);
        }
    }
}
