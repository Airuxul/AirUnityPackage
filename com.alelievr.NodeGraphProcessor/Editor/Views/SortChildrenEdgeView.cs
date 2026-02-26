using UnityEngine;
using UnityEngine.UIElements;

namespace GraphProcessor
{
    /// <summary>
    /// Edge view that draws order numbers when the output node implements <see cref="ISortChildrenByPosition"/>.
    /// </summary>
    public class SortChildrenEdgeView : EdgeView
    {
        readonly Label orderLabel = new();

        static readonly string edgeOrderStyle = "GraphProcessorStyles/EdgeOrder";

        public SortChildrenEdgeView()
        {
            orderLabel.AddToClassList("edge-order-label");
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
            if (serializedEdge?.outputNode == null || serializedEdge?.inputNode == null)
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

            var index = GraphUtils.GetSortedChildEdgeIndex(graph, serializedEdge);
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
