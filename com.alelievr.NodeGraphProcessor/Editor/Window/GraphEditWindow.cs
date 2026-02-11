using UnityEngine;

namespace GraphProcessor
{
	public class GraphEditWindow : BaseGraphWindow
	{
		private BaseGraph	_tmpGraph;
		private ToolbarView _toolbarView;
	
		protected override void OnDestroy()
		{
			graphView?.Dispose();
			DestroyImmediate(_tmpGraph);
		}

		protected override void InitializeWindow(BaseGraph graph)
		{
			titleContent = new GUIContent("All Graph");

			if (graphView == null)
			{
				graphView = new GraphEditView(this);
				_toolbarView = new ToolbarView(graphView);
				graphView.Add(_toolbarView);
			}

			rootView.Add(graphView);
		}
	}
}


