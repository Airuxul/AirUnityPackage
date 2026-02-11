using UnityEngine;
// ReSharper disable UnusedMember.Global

namespace GraphProcessor
{
    [System.Serializable, NodeMenuItem("Primitives/String")]
    public class StringNode : BaseNode
    {
        [Output(name = "Out"), SerializeField]
        public string output;

        public override string name => "String";
    }
}
