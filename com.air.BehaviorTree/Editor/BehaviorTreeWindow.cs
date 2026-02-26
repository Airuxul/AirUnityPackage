using UnityEditor;
using GraphProcessor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Air.BehaviorTree
{
    public class BehaviorTreeWindow : BaseGraphWindow
    {
        [OnOpenAsset(0)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            var asset = EditorUtility.InstanceIDToObject(instanceID) as BehaviorTreeGraph;
            if (asset == null)
                return false;

            var window = GetWindow<BehaviorTreeWindow>();
            window.InitializeGraph(asset);
            return true;
        }

        protected override void InitializeWindow(BaseGraph _graph)
        {
            titleContent = new GUIContent("Behavior Tree");
            if (graphView == null)
            {
                graphView = new BehaviorTreeGraphView(this);
                var toolbar = new BehaviorTreeToolbarView(graphView);
                graphView.Add(toolbar);
            }
            rootView.Add(graphView);
        }

        protected override void Update()
        {
            base.Update();
            if (graphView is BehaviorTreeGraphView btView)
                btView.UpdateDebugStatus();
        }
    }
}
