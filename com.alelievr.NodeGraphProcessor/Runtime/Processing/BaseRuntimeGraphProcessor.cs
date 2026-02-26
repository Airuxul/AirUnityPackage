namespace GraphProcessor
{
    /// <summary>
    /// Base processor for runtime graph execution.
    /// Processes IRuntimeGraph (RuntimeGraph) instead of BaseGraph (SO).
    /// </summary>
    public abstract class BaseRuntimeGraphProcessor
    {
        protected RuntimeGraph graph;

        public virtual void InitRuntimeGraph(RuntimeGraph _graph)
        {
            graph = _graph;
        }

        /// <summary>
        /// Execute the graph.
        /// </summary>
        public abstract void Run();

        public void Run(RuntimeGraph _graph)
        {
            InitRuntimeGraph(_graph);
            Run();
        }
    }
}
