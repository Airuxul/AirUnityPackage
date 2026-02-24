using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GraphProcessor
{
    /// <summary>
    /// Synchronous graph processor for RuntimeGraph.
    /// Processes all nodes in compute order.
    /// </summary>
    public class ProcessGraphProcessor : BaseRuntimeGraphProcessor
    {
        List<RuntimeBaseNode> processList;

        public ProcessGraphProcessor(IRuntimeGraph graph) : base(graph)
        {
            processList = graph.Nodes.OrderBy(n => n.ComputeOrder).ToList();
        }

        /// <summary>
        /// Process all nodes following the compute order (from export).
        /// </summary>
        public override void Run()
        {
            int count = processList.Count;
            for (int i = 0; i < count; i++)
            {
                var node = processList[i];
                if (node.CanProcess)
                    node.OnProcess();
            }
        }
    }
}
