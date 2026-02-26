namespace GraphProcessor
{
    /// <summary>
    /// Editor processor. Exports BaseGraph to RuntimeGraph and runs it (same execution path as runtime).
    /// </summary>
    public class EditorGraphProcessor
    {
        private BaseGraph graph;
        private RuntimeGraph runtimeGraph;
        private readonly ProcessGraphProcessor RuntimeGraphRunner;

        public EditorGraphProcessor(BaseGraph graph)
        {
            this.graph = graph;
            RuntimeGraphRunner = new ProcessGraphProcessor();
        }

        public void UpdateComputeOrder()
        {
            BuildRuntimeGraph();
        }

        public void Run()
        {
            if (runtimeGraph != null)
                RuntimeGraphRunner.Run();
        }

        void BuildRuntimeGraph()
        {
            var data = GraphExporter.Export(graph);
            runtimeGraph = RuntimeGraphBuilder.Build(data);
            RuntimeGraphRunner.Run(runtimeGraph);
        }
    }
}
