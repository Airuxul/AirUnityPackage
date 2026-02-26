using GraphProcessor;
using UnityEditor;
using UnityEngine;
using Status = UnityEngine.UIElements.DropdownMenuAction.Status;

namespace Air.BehaviorTree
{
    public class BehaviorTreeToolbarView : ToolbarView
    {
        public BehaviorTreeToolbarView(BaseGraphView graphView) : base(graphView)
        {
        }

        protected override void AddButtons()
        {
            AddButton("Center", graphView.ResetPositionAndZoom);

            bool exposedParamsVisible = graphView.GetPinnedElementStatus<ExposedParameterView>() != Status.Hidden;
            showParameters = AddToggle("Show Parameters", exposedParamsVisible, (v) => graphView.ToggleView< ExposedParameterView>());

            AddDropDownButton(new GUIContent("Export"), ShowExportMenu, false);
            AddButton("Show In Project", () => EditorGUIUtility.PingObject(graphView.graph), false);
        }
    }
}