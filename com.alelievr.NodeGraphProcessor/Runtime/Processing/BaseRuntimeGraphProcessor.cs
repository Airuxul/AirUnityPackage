namespace GraphProcessor
{
    /// <summary>
    /// Base processor for runtime graph execution.
    /// Processes IRuntimeGraph (RuntimeGraph) instead of BaseGraph (SO).
    /// </summary>
    public abstract class BaseRuntimeGraphProcessor
    {
        protected RuntimeGraph graph;

        /// <summary>
        /// Manage graph scheduling and processing.
        /// </summary>
        /// <param name="graph">Runtime graph to be processed</param>
        public BaseRuntimeGraphProcessor(RuntimeGraph graph)
        {
            this.graph = graph;
        }
        
        // public BaseRuntimeGraphProcessor()

        /// <summary>
        /// Execute the graph.
        /// </summary>
        public abstract void Run();
    }
}
