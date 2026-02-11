using System;

namespace GraphProcessor
{
	/// <summary>
	/// Factory for creating graph processors based on graph content or explicit mode.
	/// </summary>
	public static class GraphProcessorFactory
	{
		/// <summary>
		/// Creates a processor based on explicit execution order preference.
		/// </summary>
		/// <param name="graph">Graph to process.</param>
		/// <param name="useExecutionOrder">When true, uses ExecutionOrderProcessor; otherwise ProcessGraphProcessor.</param>
		/// <returns>An IGraphProcessor instance.</returns>
		public static IGraphProcessor Create(BaseGraph graph, bool useExecutionOrder)
		{
			if (graph == null)
				throw new ArgumentNullException(nameof(graph));

			if (useExecutionOrder)
				return new ExecutionOrderProcessor(graph);
			return new ProcessGraphProcessor(graph);
		}

		/// <summary>
		/// Creates a processor by auto-detecting graph structure. Uses ExecutionOrderProcessor when graph contains StartNode.
		/// </summary>
		/// <param name="graph">Graph to process.</param>
		/// <returns>An IGraphProcessor instance.</returns>
		public static IGraphProcessor Create(BaseGraph graph)
		{
			if (graph == null)
				throw new ArgumentNullException(nameof(graph));

			bool hasStartNode = graph.nodes.Exists(n => n is StartNode);
			return Create(graph, hasStartNode);
		}
	}
}
