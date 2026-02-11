using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphProcessor
{
	public class GraphEditView : BaseGraphView
	{
		public GraphEditView(EditorWindow window) : base(window) {}

		public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
		{
			BuildStackNodeContextualMenu(evt);
			// 这里实际就是添加了一个新的功能，Stack逻辑
			base.BuildContextualMenu(evt);
		}

		/// <summary>
		/// Add the New Stack entry to the context menu
		/// </summary>
		/// <param name="evt"></param>
		protected void BuildStackNodeContextualMenu(ContextualMenuPopulateEvent evt)
		{
			Vector2 position = (evt.currentTarget as VisualElement).ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
			evt.menu.AppendAction("New Stack", (e) => AddStackNode(new BaseStackNode(position)), DropdownMenuAction.AlwaysEnabled);
		}
	}
}
