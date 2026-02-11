using UnityEngine;
// ReSharper disable UnusedMember.Global
// ReSharper disable NotAccessedField.Global

namespace GraphProcessor
{
    [System.Serializable, NodeMenuItem("Unity/Output GameObject Info")]
    public class OutputGameObjectInfoNode : BaseNode
    {
        [Input(name = "Input")]
        public GameObject input;
        
        [Output(name = "Position")]
        public Vector3 goPosition;

        [Output(name = "EulerAngles")]
        public Vector3 eulerAngles;

        [Output(name = "Scale")]
        public Vector3 scale;

        protected override void Process()
        {
            if (!input)
            {
                return;
            }

            goPosition = input.transform.position;
            eulerAngles = input.transform.eulerAngles;
            scale = input.transform.localScale;
        }
    }
}