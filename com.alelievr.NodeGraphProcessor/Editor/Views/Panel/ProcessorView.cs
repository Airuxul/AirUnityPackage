using UnityEngine.UIElements;

namespace GraphProcessor
{
	public class ProcessorView : PinnedElementView
	{
		public ProcessorView()
		{
			title = "Process panel";
		}

		protected override void Initialize(BaseGraphView graphView)
		{
			Button	b = new Button(OnPlay) { name = "ActionButton", text = "Play !" };
			content.Add(b);
		}

		void OnPlay()
		{
			var processor = GraphProcessorFactory.Create(graphView.graph);
			processor.Run();
		}
	}
}
