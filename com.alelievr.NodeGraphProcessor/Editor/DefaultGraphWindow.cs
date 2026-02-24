using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace GraphProcessor
{
    public class DefaultGraphWindow : BaseGraphWindow
    {
        [MenuItem("Window/Graph Processor/Open Graph")]
        static void OpenFromMenu()
        {
            var graph = Selection.activeObject as BaseGraph;
            if (graph != null)
                Open(graph);
        }

        [MenuItem("Window/Graph Processor/Open Graph", true)]
        static bool OpenFromMenuValidate()
        {
            return Selection.activeObject is BaseGraph;
        }

        public static void Open(BaseGraph graph)
        {
            var window = GetWindow<DefaultGraphWindow>();
            window.InitializeGraph(graph);
        }

        protected override void InitializeWindow(BaseGraph graph)
        {
            var graphView = new BaseGraphView(this);
            rootView.Add(new ToolbarView(graphView));
            rootView.Add(graphView);
        }
    }
}
