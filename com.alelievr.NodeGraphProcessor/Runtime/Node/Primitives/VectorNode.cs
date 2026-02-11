using UnityEngine;
// ReSharper disable UnusedMember.Global

namespace GraphProcessor
{
    [System.Serializable, NodeMenuItem("Primitives/Vector 4")]
    public class VectorNode : BaseNode
    {
        [Output(name = "Out")]
        public Vector4 output;

        [Input(name = "In"), SerializeField]
        public Vector4 input;

        public override string name => "Vector 4";

        protected override void Process()
        {
            output = input;
        }
    }
}
