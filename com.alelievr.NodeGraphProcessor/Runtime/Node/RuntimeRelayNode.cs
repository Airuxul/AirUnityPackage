namespace GraphProcessor
{
    /// <summary>
    /// Runtime node for RelayNode. Pass-through relay logic. PackInput/UnpackOutput exported for future use.
    /// </summary>
    public class RuntimeRelayNode : RuntimeBaseNode
    {
        public bool PackInput { get; set; }
        public bool UnpackOutput { get; set; }

        public RuntimeRelayNode(RuntimeGraph graph) : base(graph) { }

        public override void OnProcess()
        {
            var input = GetInputValue("input");
            SetOutputValue("output", null, input);
        }
    }
}
