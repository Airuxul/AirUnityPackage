using System;
using UnityEngine;

namespace GraphProcessor
{
    public class FixedUpdateBehaviour : MonoBehaviour
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
        }

        public void FixedUpdate()
        {
            _processor?.Run();
        }
    }
}