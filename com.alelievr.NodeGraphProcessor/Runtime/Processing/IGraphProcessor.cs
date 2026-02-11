namespace GraphProcessor
{
	/// <summary>
	/// Interface for graph processors that execute node graphs.
	/// </summary>
	public interface IGraphProcessor
	{
		/// <summary>
		/// Executes the graph.
		/// </summary>
		void Run();
	}
}
