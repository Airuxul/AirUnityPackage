namespace GraphProcessor
{
    /// <summary>
    /// Runtime node for ParameterNode. Handles Get/Set exposed parameters.
    /// </summary>
    public class RuntimeParameterNode : RuntimeBaseNode
    {
        public string ParameterGUID { get; set; }
        public int ParameterAccessor { get; set; }

        public RuntimeParameterNode(RuntimeGraph graph) : base(graph) { }

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