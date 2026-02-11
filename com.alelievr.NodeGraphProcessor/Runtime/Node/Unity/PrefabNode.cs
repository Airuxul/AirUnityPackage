using UnityEngine;
// ReSharper disable UnusedMember.Global

namespace GraphProcessor
{
    [System.Serializable, NodeMenuItem("Unity/Prefab")]
    public class PrefabNode : BaseNode
    {
        [Output(name = "Out"), SerializeField]
        public GameObject output;

        public override string name => "Prefab";
    }
}
