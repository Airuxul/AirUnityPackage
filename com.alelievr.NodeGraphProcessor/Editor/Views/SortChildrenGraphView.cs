using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphProcessor
{
    /// <summary>
    /// Graph view that uses <see cref="SortChildrenEdgeView"/> to draw order labels
    /// when parent nodes implement <see cref="ISortChildrenByPosition"/>.
    /// </summary>
    public class SortChildrenGraphView : BaseGraphView
    {
        public SortChildrenGraphView(UnityEditor.EditorWindow window) : base(window)
        {
            computeOrderUpdated += RefreshEdgeOrderLabels;
            onNodePositionChanged += _ => RefreshEdgeOrderLabels();
        }

        public override EdgeView CreateEdgeView()
        {
            return new SortChildrenEdgeView();
        }

        void RefreshEdgeOrderLabels()
        {
            foreach (var edgeView in edgeViews)
            {
                if (edgeView is SortChildrenEdgeView sortEdge)
                    sortEdge.UpdateOrderLabel();
            }
        }
    }
}
