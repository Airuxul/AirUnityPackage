using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Callbacks;
using GraphProcessor;

namespace Air.BehaviorTree.Editor
{
    /// <summary>
    /// Editor window for editing behavior tree graphs.
    /// </summary>
    public class BehaviorTreeWindow : BaseGraphWindow
    {
        private ToolbarView	_toolbarView;
        
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

        protected override void InitializeWindow(BaseGraph graph)
        {
            titleContent = new GUIContent("Behavior Tree");
            
            if (graphView == null)
            {
                graphView = new BaseGraphView(this);
                _toolbarView = new ToolbarView(graphView);
                graphView.Add(_toolbarView);
            }
            rootView.Add(graphView);
        }
    }
}
