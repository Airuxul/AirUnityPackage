using System;
using System.Collections.Generic;
using GraphProcessor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviorTree
{
    /// <summary>
    /// Graph view for behavior trees. Uses custom edge view that draws order numbers.
    /// </summary>
    public class BehaviorTreeGraphView : BaseGraphView
    {
        const string RuntimeRunningClass = "bt-runtime-running";
        const string RuntimeSuccessClass = "bt-runtime-success";
        const string RuntimeFailureClass = "bt-runtime-failure";

        static readonly string RuntimeStatusStyle = "BehaviorTreeStyles/BehaviorTreeRuntimeStatus";

        BehaviorTreeRunner _debugTarget;

        public BehaviorTreeGraphView(EditorWindow window) : base(window)
        {
            computeOrderUpdated += RefreshEdgeOrderLabels;
            onNodePositionChanged += _ => RefreshEdgeOrderLabels();
            var statusStyle = Resources.Load<StyleSheet>(RuntimeStatusStyle);
            if (statusStyle != null)
                styleSheets.Add(statusStyle);
        }

        static readonly Color RunningColor = new(1f, 0.78f, 0f);
        static readonly Color SuccessColor = new(0f, 0.78f, 0.31f);
        static readonly Color FailureColor = new(1f, 0.31f, 0.31f);

        private readonly Dictionary<string, float> guid2LastNodeExecutionTime = new();

        public void UpdateDebugStatus()
        {
            if (!Application.isPlaying)
            {
                ClearAllRuntimeStatus();
                return;
            }

            var selectGo = Selection.activeGameObject;
            if (selectGo == null || !selectGo.TryGetComponent(out _debugTarget))
            {
                ClearAllRuntimeStatus();
                return;
            }

            foreach (var (guid, node) in _debugTarget.RuntimeGraph.Guid2Nodes)
            {
                if (!graph.nodesPerGUID.TryGetValue(guid, out var baseNode)
                    || !nodeViewsPerNode.TryGetValue(baseNode, out var nodeView)
                    || node is not RuntimeBTBaseNode btNode)
                {
                    continue;
                }

                var lastExecutionTime = guid2LastNodeExecutionTime.GetValueOrDefault(guid, 0f);
                if (btNode.LastExecutionTime > lastExecutionTime)
                {
                    ApplyRuntimeStatus(nodeView, btNode.Status);
                    guid2LastNodeExecutionTime[guid] = btNode.LastExecutionTime;
                }
                // 如果节点在0.1秒内没有更新状态，且不是Running状态，则清除状态显示
                else if (Time.time - btNode.LastExecutionTime > 0.2 && btNode.Status != BehaviorTreeStatus.Running)
                {
                    ClearRuntimeStatus(nodeView);
                }
            }
        }

        void ApplyRuntimeStatus(BaseNodeView view, BehaviorTreeStatus status)
        {
            switch (status)
            {
                case BehaviorTreeStatus.Running:
                    view.AddToClassList(RuntimeRunningClass);
                    ApplyRuntimeStatus(view, RunningColor);
                    break;
                case BehaviorTreeStatus.Success:
                case BehaviorTreeStatus.Failure:
                    view.AddToClassList(RuntimeSuccessClass);
                    ApplyRuntimeStatus(view, status == BehaviorTreeStatus.Success ? SuccessColor : FailureColor);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void ApplyRuntimeStatus(BaseNodeView view, Color color)
        {
            var nodeBorder = view.Q("node-border");
            var target = nodeBorder ?? view;
            target.style.borderTopWidth = 3;
            target.style.borderBottomWidth = 3;
            target.style.borderLeftWidth = 3;
            target.style.borderRightWidth = 3;
            target.style.borderTopColor = color;
            target.style.borderBottomColor = color;
            target.style.borderLeftColor = color;
            target.style.borderRightColor = color;
        }

        void ClearRuntimeStatus(BaseNodeView view)
        {
            var nodeBorder = view.Q("node-border");
            var target = nodeBorder ?? view;
            target.style.borderTopWidth = 0;
            target.style.borderBottomWidth = 0;
            target.style.borderLeftWidth = 0;
            target.style.borderRightWidth = 0;
        }

        void ClearAllRuntimeStatus()
        {
            foreach (var view in nodeViewsPerNode.Values)
            {
                if (view == null) continue;
                view.RemoveFromClassList(RuntimeRunningClass);
                view.RemoveFromClassList(RuntimeSuccessClass);
                view.RemoveFromClassList(RuntimeFailureClass);
                ClearRuntimeStatus(view);
            }
            
            guid2LastNodeExecutionTime.Clear();
        }

        public override EdgeView CreateEdgeView()
        {
            return new BehaviorTreeEdgeView();
        }

        void RefreshEdgeOrderLabels()
        {
            foreach (var edgeView in edgeViews)
            {
                if (edgeView is BehaviorTreeEdgeView btEdge)
                    btEdge.UpdateOrderLabel();
            }
        }
    }
}