// ReSharper disable UnusedMember.Global

using UnityEngine;

namespace GraphProcessor
{
    [System.Serializable, NodeMenuItem("Debug/Message")]
    public class MessageNode : BaseNode
    {
        private const string InputIsNot42Error = "Input is not 42 !";

        [Input(name = "In")]
        public float input;

        public override string name => "MessageNode";

        [Setting("Message Type")]
        public NodeMessageType messageType = NodeMessageType.Error;

        protected override void Process()
        {
            if (!Mathf.Approximately(input, 42))
                AddMessage(InputIsNot42Error, messageType);
            else
                RemoveMessage(InputIsNot42Error);
        }
    }
}
