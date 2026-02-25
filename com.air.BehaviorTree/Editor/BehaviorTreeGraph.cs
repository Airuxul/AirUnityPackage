using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GraphProcessor;

namespace BehaviorTree
{
    /// <summary>
    /// Behavior tree graph type. Used by the editor and for asset creation.
    /// </summary>
    [CreateAssetMenu(menuName = "Behavior Tree/Graph", fileName = "BehaviorTree")]
    public class BehaviorTreeGraph : BaseGraph
    {
        public override void UpdateComputeOrder(ComputeOrderType type = ComputeOrderType.DepthFirst)
        {
            base.UpdateComputeOrder(type);

            var processedParents = new HashSet<ControlNode>();
            foreach (var edge in edges)
            {
                if (edge.outputNode == null || edge.inputNode == null) continue;
                if (edge.outputNode is not ControlNode controlNode) continue;
                if (edge.outputFieldName != "output") continue;
                if (!processedParents.Add(controlNode)) continue;

                var siblingEdges = edges
                    .Where(e => e.outputNode == controlNode && e.outputFieldName == "output")
                    .OrderBy(e => e.inputNode.position.y)
                    .ToList();

                for (var i = 0; i < siblingEdges.Count; i++)
                {
                    var childNode = siblingEdges[i].inputNode;
                    childNode.computeOrder = controlNode.computeOrder * 10000 + i;
                }
            }
        }
    }
}
