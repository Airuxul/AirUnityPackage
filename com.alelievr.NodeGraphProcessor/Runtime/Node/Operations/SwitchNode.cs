// ReSharper disable UnusedMember.Global

namespace GraphProcessor
{
    [System.Serializable, NodeMenuItem("Operations/Switch")]
    public class SwitchNode : BaseNode
    {
        [Input(name = "In")]
        public float input;

        [Output(name = "Out")]
        public float output;

        public override string name => "Switch";

        protected override void Process()
        {
            output = input * 42;
        }
    }
}
