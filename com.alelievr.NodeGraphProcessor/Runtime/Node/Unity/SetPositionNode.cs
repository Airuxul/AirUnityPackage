using UnityEngine;
// ReSharper disable UnusedMember.Global

namespace GraphProcessor
{
    [System.Serializable, NodeMenuItem("Unity/Set Position")]
    public class SetPositionNode : LinearConditionalNode
    {
        public override string name => "SetPosition";

        [Input(name = "GameObject")] public GameObject go;

        [Input(name = "Position")] public Vector4 setPosition;

        [Setting("IsLocal")] public bool isLocal;

        protected override void Process()
        {
            if (!go)
            {
                return;
            }

            if (isLocal)
            {
                go.transform.localPosition = setPosition;
            }
            else
            {
                go.transform.position = setPosition;
            }
        }
    }
}