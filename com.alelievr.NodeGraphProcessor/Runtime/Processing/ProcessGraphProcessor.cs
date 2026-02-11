namespace GraphProcessor
{
	/// <summary>
	/// Graph processor. Executes nodes in graph.nodes order (set by editor when nodes are rearranged).
	/// </summary>
	public class ProcessGraphProcessor : BaseGraphProcessor
	{
		public ProcessGraphProcessor(BaseGraph graph) : base(graph) {}

		public override void Run()
		{
			foreach (var node in Graph.nodes)
				node.OnProcess();
		}
	}
}
