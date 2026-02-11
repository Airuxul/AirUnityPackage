// ReSharper disable UnusedMember.Global

namespace GraphProcessor
{
    [System.Serializable, NodeMenuItem("Debug/Print")]
    public class PrintNode : BaseNode
    {
        [Input]
        public object obj;

        public override string name => "Print";
    }

    [System.Serializable, NodeMenuItem("Debug/Print Conditional")]
    public class ConditionalPrintNode : LinearConditionalNode
    {
        [Input]
        public object obj;

        public override string name => "Print";
    }
}
