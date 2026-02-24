namespace GraphProcessor
{
    /// <summary>
    /// Editor processor. Exports BaseGraph to RuntimeGraph and runs it (same execution path as runtime).
    /// </summary>
    public class EditorGraphProcessor
    {
        BaseGraph graph;
        RuntimeGraph runtimeGraph;
        private readonly ProcessGraphProcessor RuntimeGraphRunner;

        public EditorGraphProcessor(BaseGraph graph)
        {
            this.graph = graph;
            RuntimeGraphRunner = new ProcessGraphProcessor(RuntimeGraphBuilder.Build(GraphExporter.Export(graph)));
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
                RuntimeGraphRunner.Run();
        }

        void BuildRuntimeGraph()
        {
            var data = GraphExporter.Export(graph);
            runtimeGraph = RuntimeGraphBuilder.Build(data);
        }
    }
}
