using UnityEngine;
// ReSharper disable UnusedMember.Global
// ReSharper disable NotAccessedField.Global

namespace GraphProcessor
{
    [System.Serializable, NodeMenuItem("Unity/Set GameObject Info")]
    public class SetGameObjectInfoNode : BaseNode
    {
        [Input(name = "Position")]
        public Vector3 goPosition;

        [Input(name = "EulerAngles")]
        public Vector3 eulerAngles;

        [Input(name = "Scale")]
        public Vector3 scale;

        [Output(name = "Input")]
        public GameObject input;

        protected override void Process()
        {
            if (!input)
                return;

            if (IsInputPortConnected(nameof(goPosition)))
                input.transform.position = goPosition;

            if (IsInputPortConnected(nameof(eulerAngles)))
                input.transform.eulerAngles = eulerAngles;

            if (IsInputPortConnected(nameof(scale)))
                input.transform.localScale = scale;
        }
    }
}