using UnityEngine;
// ReSharper disable UnusedMember.Global

namespace GraphProcessor
{
    [System.Serializable, NodeMenuItem("Primitives/Color")]
    public class ColorNode : BaseNode
    {
        [Output(name = "Color"), SerializeField]
        public new Color color;

        public override string name => "Color";
    }
}
