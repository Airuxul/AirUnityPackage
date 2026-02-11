// ReSharper disable UnusedMember.Global

namespace GraphProcessor
{
    [System.Serializable, NodeMenuItem("Test/Inspector")]
    public class InspectorNode : BaseNode
    {
        [Input(name = "In")]
        public float input;

        [Output(name = "Out")]
        public float output;

        [ShowInInspector]
        public bool additionalSettings;

        [ShowInInspector]
        public string additionalParam;

        public override string name => "InspectorNode";

        protected override void Process()
        {
            output = input * 42;
        }
    }
}
