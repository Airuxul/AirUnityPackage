// ReSharper disable UnusedMember.Global

namespace GraphProcessor
{
    [System.Serializable, NodeMenuItem("Test/Renamable")]
    public class RenamableNode : BaseNode
    {
        [Output("Out")]
        public float output;

        [Input("In")]
        public float input;

        public override string name => "Renamable";

        public override bool isRenamable => true;

        protected override void Process() => output = input;
    }
}
