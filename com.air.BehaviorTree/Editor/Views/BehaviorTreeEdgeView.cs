using System.Linq;
using GraphProcessor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviorTree
{
    /// <summary>
    /// Edge view that draws order numbers for control nodes with sortChildrenByPosition.
    /// </summary>
    public class BehaviorTreeEdgeView : EdgeView
    {
        readonly Label orderLabel = new();

        static readonly string edgeOrderStyle = "BehaviorTreeStyles/BehaviorTreeEdgeOrder";

        public BehaviorTreeEdgeView()
        {
            orderLabel.AddToClassList("behavior-tree-edge-order");
            orderLabel.styleSheets.Add(Resources.Load<StyleSheet>(edgeOrderStyle));
            orderLabel.pickingMode = PickingMode.Ignore;
            Add(orderLabel);
        }

        public override void OnPortChanged(bool isInput)
        {
            base.OnPortChanged(isInput);
            schedule.Execute(UpdateOrderLabel).ExecuteLater(1);
        }

        protected override void OnCustomStyleResolved(ICustomStyle styles)
        {
            base.OnCustomStyleResolved(styles);
            schedule.Execute(UpdateOrderLabel).ExecuteLater(1);
        }

        public void UpdateOrderLabel()
        {
            if (serializedEdge?.outputNode == null
                || serializedEdge?.inputNode == null
                || serializedEdge.outputNode is not ControlNode controlNode)
            {
                orderLabel.style.display = DisplayStyle.None;
                return;
            }

            var graph = owner?.graph;
            if (graph == null)
            {
                orderLabel.style.display = DisplayStyle.None;
                return;
            }

            var siblingEdges = graph.edges
                .Where(e => e.outputNode == controlNode && e.outputFieldName == "output")
                .OrderBy(e => e.inputNode.position.y)
                .ToList();

            var index = siblingEdges.FindIndex(e => e.GUID == serializedEdge.GUID);
            if (index < 0)
            {
                orderLabel.style.display = DisplayStyle.None;
                return;
            }

            orderLabel.text = (index + 1).ToString();
            orderLabel.style.display = DisplayStyle.Flex;

            UpdateOrderLabelPosition();
        }

        void UpdateOrderLabelPosition()
        {
            if (input == null || output == null) return;

            var inputCenter = input?.GetGlobalCenter() ?? Vector2.zero;
            var outputCenter = output?.GetGlobalCenter() ?? Vector2.zero;
            var midpoint = (inputCenter + outputCenter) * 0.5f;

            var localMid = parent?.WorldToLocal(midpoint) ?? midpoint;
            orderLabel.style.left = localMid.x;
            orderLabel.style.top = localMid.y;
        }
    }
}
