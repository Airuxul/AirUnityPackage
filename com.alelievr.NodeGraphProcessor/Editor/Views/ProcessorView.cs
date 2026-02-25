using UnityEngine.UIElements;

namespace GraphProcessor
{
	public class ProcessorView : PinnedElementView
	{
		EditorGraphProcessor	processor;

		public ProcessorView()
		{
			title = "Process panel";
		}

		protected override void Initialize(BaseGraphView graphView)
		{
			processor = new EditorGraphProcessor(graphView.graph);

			graphView.computeOrderUpdated += processor.UpdateComputeOrder;

			Button	b = new Button(OnPlay) { name = "ActionButton", text = "Play !" };

			content.Add(b);
		}

		void OnPlay()
		{
			processor.Run();
		}
	}
}
