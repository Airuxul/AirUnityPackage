using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphProcessor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Air.BehaviorTree.Editor
{
    /// <summary>
    /// Graph view for behavior trees. Renders order numbers on edges when output has [OrderByPosition].
    /// Shows flash effect on nodes when they execute at runtime (Play mode).
    /// </summary>
    public class BehaviorTreeGraphView : BaseGraphView
    {
        VisualElement edgeOrderOverlay;
        VisualElement nodeFlashOverlay;
        readonly Dictionary<EdgeView, Label> edgeOrderLabels = new Dictionary<EdgeView, Label>();
        readonly Dictionary<BaseNodeView, VisualElement> nodeFlashElements = new Dictionary<BaseNodeView, VisualElement>();
        static readonly string OrderLabelStylePath = "BehaviorTreeStyles/EdgeOrderLabel";
        static readonly string NodeFlashStylePath = "BehaviorTreeStyles/NodeFlash";
        bool edgeLabelUpdateScheduled;

        public BehaviorTreeGraphView(EditorWindow window) : base(window)
        {
            var flashStyle = Resources.Load<StyleSheet>(NodeFlashStylePath);
            if (flashStyle != null)
                styleSheets.Add(flashStyle);

            initialized += () =>
            {
                EnsureEdgeOrderOverlay();
                EnsureNodeFlashOverlay();
                schedule.Execute(UpdateEdgeOrderLabels).StartingIn(100);
                graph.onGraphChanges += _ => ScheduleEdgeLabelUpdate();
                onNodePositionChanged += _ => ScheduleEdgeLabelUpdate();
            };
        }

        void EnsureNodeFlashOverlay()
        {
            if (nodeFlashOverlay != null)
                return;

            nodeFlashOverlay = new VisualElement
            {
                pickingMode = PickingMode.Ignore,
                style =
                {
                    position = Position.Absolute,
                    left = 0, top = 0, right = 0, bottom = 0,
                    overflow = Overflow.Visible
                }
            };
            var flashStyle = Resources.Load<StyleSheet>(NodeFlashStylePath);
            if (flashStyle != null)
                nodeFlashOverlay.styleSheets.Add(flashStyle);
            contentViewContainer.Add(nodeFlashOverlay);
        }

        /// <summary>
        /// Updates debug visuals (white border on executing nodes). Call every frame in Play mode.
        /// Non-runtime: hides all flash effects.
        /// </summary>
        public void UpdateDebugVisuals()
        {
            if (graph == null)
                return;

            var btGraph = graph as BehaviorTreeGraph;
            if (btGraph == null)
                return;

            BehaviorTreeDebugState debugState = null;
            if (Application.isPlaying)
            {
                var selectedGo = Selection.activeGameObject;
                if (selectedGo != null && selectedGo.TryGetComponent<BehaviorTreeRunner>(out var selectedRunner) &&
                    selectedRunner.BehaviorTree == btGraph && selectedRunner.DebugState != null)
                {
                    debugState = selectedRunner.DebugState;
                }
            }

            EnsureNodeFlashOverlay();

            foreach (var kv in nodeViewsPerNode)
            {
                var node = kv.Key;
                var view = kv.Value;
                if (!(node is BehaviorTreeNode))
                    continue;

                var shouldFlash = debugState != null && debugState.ShouldFlash(node.GUID);

                if (shouldFlash)
                {
                    if (!nodeFlashElements.TryGetValue(view, out var flashEl))
                    {
                        flashEl = new VisualElement
                        {
                            pickingMode = PickingMode.Ignore,
                            name = "bt-node-flash-overlay"
                        };
                        flashEl.AddToClassList("bt-node-flash");
                        nodeFlashOverlay.Add(flashEl);
                        nodeFlashElements[view] = flashEl;
                    }

                    var viewWorldBound = view.worldBound;
                    var localRect = contentViewContainer.WorldToLocal(viewWorldBound);
                    flashEl.style.position = Position.Absolute;
                    flashEl.style.left = localRect.x - 4;
                    flashEl.style.top = localRect.y - 4;
                    flashEl.style.width = localRect.width + 8;
                    flashEl.style.height = localRect.height + 8;
                    flashEl.style.display = DisplayStyle.Flex;
                }
                else if (nodeFlashElements.TryGetValue(view, out var flashEl))
                {
                    flashEl.style.display = DisplayStyle.None;
                }
            }

            foreach (var kv in nodeFlashElements.ToList())
            {
                if (!nodeViewsPerNode.ContainsValue(kv.Key))
                {
                    kv.Value.RemoveFromHierarchy();
                    nodeFlashElements.Remove(kv.Key);
                }
            }
        }

        void ScheduleEdgeLabelUpdate()
        {
            if (edgeLabelUpdateScheduled)
                return;
            edgeLabelUpdateScheduled = true;
            schedule.Execute(() =>
            {
                UpdateEdgeOrderLabels();
                edgeLabelUpdateScheduled = false;
            }).StartingIn(0);
        }

        void EnsureEdgeOrderOverlay()
        {
            if (edgeOrderOverlay != null)
                return;

            edgeOrderOverlay = new VisualElement
            {
                pickingMode = PickingMode.Ignore,
                style =
                {
                    position = Position.Absolute,
                    left = 0, top = 0, right = 0, bottom = 0,
                    overflow = Overflow.Visible
                }
            };
            var styleSheet = Resources.Load<StyleSheet>(OrderLabelStylePath);
            if (styleSheet != null)
                edgeOrderOverlay.styleSheets.Add(styleSheet);
            contentViewContainer.Add(edgeOrderOverlay);
        }

        public override EdgeView CreateEdgeView()
        {
            return new BehaviorTreeEdgeView();
        }

        void UpdateEdgeOrderLabels()
        {
            EnsureEdgeOrderOverlay();

            var btEdgeViews = edgeViews.OfType<BehaviorTreeEdgeView>().ToList();

            foreach (var kv in edgeOrderLabels.ToList())
            {
                if (!edgeViews.Contains(kv.Key))
                {
                    kv.Value.RemoveFromHierarchy();
                    edgeOrderLabels.Remove(kv.Key);
                }
            }

            foreach (var edgeView in btEdgeViews)
            {
                var order = GetEdgeOrder(edgeView);
                if (!order.HasValue)
                {
                    if (edgeOrderLabels.TryGetValue(edgeView, out var oldLabel))
                    {
                        oldLabel.RemoveFromHierarchy();
                        edgeOrderLabels.Remove(edgeView);
                    }
                    continue;
                }

                if (!edgeOrderLabels.TryGetValue(edgeView, out var label))
                {
                    label = new Label
                    {
                        pickingMode = PickingMode.Ignore,
                        text = order.Value.ToString()
                    };
                    label.AddToClassList("bt-edge-order-label");
                    label.AddToClassList("visible");
                    edgeOrderOverlay.Add(label);
                    edgeOrderLabels[edgeView] = label;
                }
                else
                {
                    label.text = order.Value.ToString();
                }

                Vector2 center;
                var layout = edgeView.layout;
                if (layout.width > 0 && layout.height > 0)
                {
                    center = new Vector2(layout.x + layout.width * 0.5f, layout.y + layout.height * 0.5f);
                }
                else if (edgeView.input != null && edgeView.output != null)
                {
                    var inputCenter = edgeView.input.worldBound.center;
                    var outputCenter = edgeView.output.worldBound.center;
                    var localCenter = contentViewContainer.WorldToLocal((inputCenter + outputCenter) * 0.5f);
                    center = new Vector2(localCenter.x, localCenter.y);
                }
                else
                {
                    continue;
                }

                label.style.position = Position.Absolute;
                label.style.left = center.x - 9;
                label.style.top = center.y - 9;
                label.style.width = 18;
                label.style.height = 18;
                label.style.display = DisplayStyle.Flex;
            }
        }

        int? GetEdgeOrder(EdgeView edgeView)
        {
            var serializedEdge = edgeView.serializedEdge;
            if (serializedEdge?.outputNode == null || serializedEdge?.inputNode == null)
                return null;

            var outputNode = serializedEdge.outputNode;
            if (!HasOrderByPosition(outputNode))
                return null;

            var outputPort = outputNode.GetPort(serializedEdge.outputFieldName, serializedEdge.outputPortIdentifier);
            if (outputPort == null)
                return null;

            var edges = outputPort.GetEdges();
            if (edges == null || edges.Count <= 1)
                return null;

            var sortedEdges = edges
                .OrderBy(e => e.inputNode.position.y)
                .ThenBy(e => e.inputNode.position.x)
                .ToList();

            int index = sortedEdges.IndexOf(serializedEdge);
            return index >= 0 ? index + 1 : (int?)null;
        }

        static bool HasOrderByPosition(BaseNode node)
        {
            foreach (var field in node.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (field.GetCustomAttribute<OutputAttribute>() != null &&
                    field.GetCustomAttribute<OrderByPositionAttribute>() != null)
                    return true;
            }
            return false;
        }
    }
}
