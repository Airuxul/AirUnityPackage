using UnityEngine;
using GraphProcessor;

namespace Air.BehaviorTree
{
    /// <summary>
    /// Graph asset for behavior trees. Inherits from BaseGraph to work with NodeGraphProcessor.
    /// </summary>
    [CreateAssetMenu(fileName = "New Behavior Tree", menuName = "Air/Behavior Tree")]
    public class BehaviorTreeGraph : BaseGraph
    {
    }
}
