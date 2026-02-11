namespace GraphProcessor
{

	/// <summary>
	/// Graph processor
	/// </summary>
	public abstract class BaseGraphProcessor : IGraphProcessor
	{
		protected readonly BaseGraph Graph;
		
		/// <summary>
		/// Manage graph scheduling and processing
		/// </summary>
		/// <param name="graph">Graph to be processed</param>
		public BaseGraphProcessor(BaseGraph graph)
		{
			Graph = graph;
		}

		/// <summary>
		/// Schedule the graph into the job system
		/// </summary>
		public abstract void Run();
	}
}
