using UnityEngine;
using UnityEngine.Events;
// ReSharper disable UnusedMember.Global

namespace GraphProcessor
{
    [System.Serializable, NodeMenuItem("Unity/Unity Event")]
    public class UnityEventNode : BaseNode
    {
        [Input(name = "In")]
        public float input;

        [Output(name = "Out")]
        public float output;

        public UnityEvent evt;

        public override string name => "Unity Event Node";

        protected override void Process()
        {
            output = input * 42;
        }
    }
}
