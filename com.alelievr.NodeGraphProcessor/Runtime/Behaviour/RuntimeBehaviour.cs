using UnityEngine;

namespace GraphProcessor
{
    public class RuntimeBehaviour : MonoBehaviour
    {
        public BaseGraph graph;
        public bool isExecutionOrder = true;

        private IGraphProcessor _processor;

        private void Start()
        {
            if (graph == null)
                return;

            _processor = GraphProcessorFactory.Create(graph, isExecutionOrder);
            graph.SetParameterValue("GameObject", gameObject);
            _processor.Run();
        }
    }
}