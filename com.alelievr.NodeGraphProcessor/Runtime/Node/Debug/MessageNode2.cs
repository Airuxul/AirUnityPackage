// ReSharper disable UnusedMember.Global

namespace GraphProcessor
{
    [System.Serializable, NodeMenuItem("Debug/Message 2")]
    public class MessageNode2 : BaseNode
    {
        [Input(name = "In")]
        public float input;

        [Output(name = "Out")]
        public float output;

        public override string name => "MessageNode2";

        protected override void Process()
        {
            output = input * 42;
        }
    }
}
