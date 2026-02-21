using System.Linq;
using UnityEngine;
using GraphProcessor;

namespace Air.BehaviorTree
{
    /// <summary>
    /// Processes a behavior tree graph at runtime. Finds the root node and executes the tree.
    /// </summary>
    public class BehaviorTreeProcessor
    {
        readonly BehaviorTreeGraph graph;
        readonly IBehaviorTreeContext context;
        RootNode rootNode;

        public BehaviorTreeProcessor(BehaviorTreeGraph graph, IBehaviorTreeContext context)
        {
            this.graph = graph;
            this.context = context;
            FindRoot();
        }

        void FindRoot()
        {
            rootNode = graph.nodes.OfType<RootNode>().FirstOrDefault();
            if (rootNode == null)
                Debug.LogError("[BehaviorTree] No Root node found in the graph.");
        }

        /// <summary>
        /// Execute the behavior tree once. Call every frame or at desired tick rate.
        /// </summary>
        /// <returns>Status of the root execution.</returns>
        public BTStatus Tick()
        {
            if (rootNode == null)
                return BTStatus.Failure;

            return rootNode.Execute(context);
        }
    }
}
