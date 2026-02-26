using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;

namespace Air.BehaviorTree
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

            var processedParentFields = new HashSet<(BaseNode, string)>();
            foreach (var edge in edges)
            {
                if (edge.outputNode == null || edge.inputNode == null) continue;
                if (edge.outputNode is not ISortChildrenByPosition) continue;
                if (!processedParentFields.Add((edge.outputNode, edge.outputFieldName))) continue;

                var siblingEdges = GraphUtils.GetSortedChildEdges(this, edge.outputNode, edge.outputFieldName);
                if (siblingEdges == null) continue;

                var parentComputeOrder = edge.outputNode.computeOrder;
                for (var i = 0; i < siblingEdges.Count; i++)
                {
                    var childNode = siblingEdges[i].inputNode;
                    childNode.computeOrder = parentComputeOrder * 10000 + i;
                }
            }
        }
    }
}
