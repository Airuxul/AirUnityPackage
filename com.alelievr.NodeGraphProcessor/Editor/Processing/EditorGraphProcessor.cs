namespace GraphProcessor
{
    /// <summary>
    /// Editor processor. Exports BaseGraph to RuntimeGraph and runs it (same execution path as runtime).
    /// </summary>
    public class EditorGraphProcessor
    {
        BaseGraph graph;
        RuntimeGraph runtimeGraph;

        public EditorGraphProcessor(BaseGraph graph)
        {
            this.graph = graph;
        }

        public void UpdateComputeOrder()
        {
            BuildRuntimeGraph();
        }

        public void Run()
        {
            if (runtimeGraph == null)
                BuildRuntimeGraph();
            if (runtimeGraph != null)
                RuntimeGraphRunner.Run(runtimeGraph);
        }

        void BuildRuntimeGraph()
        {
            var data = GraphExporter.Export(graph);
            runtimeGraph = RuntimeGraphBuilder.Build(data);
        }
    }
}
