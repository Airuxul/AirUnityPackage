using System.Collections.Generic;
using UnityEngine;

namespace Air.BehaviorTree
{
    /// <summary>
    /// Default context implementation. Can be extended for game-specific data.
    /// </summary>
    public class DefaultBehaviorTreeContext : IBehaviorTreeNodeStateContext
    {
        public GameObject GameObject { get; }
        public Transform Transform { get; }
        public Dictionary<string, object> NodeState { get; } = new Dictionary<string, object>();

        public DefaultBehaviorTreeContext(GameObject gameObject)
        {
            GameObject = gameObject;
            Transform = gameObject.transform;
        }
    }
}