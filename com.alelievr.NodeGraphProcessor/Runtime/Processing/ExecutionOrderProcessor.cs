using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GraphProcessor
{
    /// <summary>
    /// Processor for flow graphs with StartNode, WaitableNode, ForLoopNode support.
    /// Execution order follows [ExecutionOrder] attribute and node Y position.
    /// </summary>
    public class ExecutionOrderProcessor : BaseGraphProcessor
    {
        private List<StartNode> startNodeList;
        private readonly Dictionary<BaseNode, List<BaseNode>> nonConditionalDependenciesCache = new();

        public bool pause;

        public IEnumerator<BaseNode> currentGraphExecution { get; private set; }

        public ExecutionOrderProcessor(BaseGraph graph) : base(graph) { }

        void InitializeNodeLists()
        {
            startNodeList = Graph.nodes.Where(n => n is StartNode).Cast<StartNode>().ToList();
            if (startNodeList.Count > 0)
                nonConditionalDependenciesCache.Clear();
        }

        public override void Run()
        {
            InitializeNodeLists();
            IEnumerator<BaseNode> enumerator;

            if (startNodeList.Count == 0)
            {
                enumerator = RunTheGraph();
            }
            else
            {
                var nodeToExecute = new Stack<BaseNode>();
                foreach (var s in startNodeList)
                    nodeToExecute.Push(s);
                enumerator = RunTheGraph(nodeToExecute);
            }

            while (enumerator.MoveNext())
                ;
        }

        private void WaitedRun(Stack<BaseNode> nodesToRun)
        {
            var enumerator = RunTheGraph(nodesToRun);

            while (enumerator.MoveNext())
                ;
        }

        private IEnumerable<BaseNode> GatherNonConditionalDependencies(BaseNode node)
        {
            var dependencies = new Stack<BaseNode>();

            dependencies.Push(node);

            while (dependencies.Count > 0)
            {
                var dependency = dependencies.Pop();

                foreach (var d in dependency.GetInputNodes().Where(n => !(n is IConditionalNode)))
                    dependencies.Push(d);

                if (dependency != node)
                    yield return dependency;
            }
        }

        private IEnumerator<BaseNode> RunTheGraph()
        {
            foreach (var node in Graph.nodes)
            {
                node.OnProcess();
                yield return node;
            }
        }

        private static void PushReversed(Stack<BaseNode> stack, IEnumerable<BaseNode> nodes)
        {
            foreach (var n in nodes.Reverse())
                stack.Push(n);
        }

        private void HandleForLoop(ForLoopNode forLoopNode, Stack<BaseNode> nodeToExecute, HashSet<BaseNode> skipConditionalHandling)
        {
            forLoopNode.index = forLoopNode.start - 1;
            PushReversed(nodeToExecute, forLoopNode.GetExecutedNodesLoopCompleted());
            for (var i = forLoopNode.start; i < forLoopNode.end; i++)
            {
                PushReversed(nodeToExecute, forLoopNode.GetExecutedNodesLoopBody());
                nodeToExecute.Push(forLoopNode);
            }
            skipConditionalHandling.Add(forLoopNode);
        }

        private void HandleWaitable(WaitableNode waitableNode, Stack<BaseNode> nodeToExecute)
        {
            PushReversed(nodeToExecute, waitableNode.GetExecutedNodes());
            waitableNode.onProcessFinished += waitedNode =>
            {
                var waitedNodes = new Stack<BaseNode>();
                PushReversed(waitedNodes, waitedNode.GetExecuteAfterNodes());
                WaitedRun(waitedNodes);
                waitableNode.onProcessFinished = null;
            };
        }

        private IEnumerator<BaseNode> RunTheGraph(Stack<BaseNode> nodeToExecute)
        {
            var nodeDependenciesGathered = new HashSet<BaseNode>();
            var skipConditionalHandling = new HashSet<BaseNode>();

            while (nodeToExecute.Count > 0)
            {
                var node = nodeToExecute.Pop();
                var isConditional = node is IConditionalNode && !skipConditionalHandling.Contains(node);
                var depsReady = nodeDependenciesGathered.Contains(node);

                if (isConditional && !depsReady)
                {
                    nodeToExecute.Push(node);
                    nodeDependenciesGathered.Add(node);
                    foreach (var dep in GatherNonConditionalDependencies(node))
                        nodeToExecute.Push(dep);
                    continue;
                }

                if (isConditional)
                    nodeDependenciesGathered.Remove(node);

                node.OnProcess();
                yield return node;

                if (isConditional)
                {
                    switch (node)
                    {
                        case ForLoopNode forLoopNode:
                            HandleForLoop(forLoopNode, nodeToExecute, skipConditionalHandling);
                            break;
                        case WaitableNode waitableNode:
                            HandleWaitable(waitableNode, nodeToExecute);
                            break;
                        case IConditionalNode cNode:
                            PushReversed(nodeToExecute, cNode.GetExecutedNodes());
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Advance the execution of the graph by one node, for debug. Doesn't work for WaitableNode's executeAfter port.
        /// </summary>
        public void Step()
        {
            if (currentGraphExecution == null)
            {
                InitializeNodeLists();
                var nodeToExecute = new Stack<BaseNode>();
                if (startNodeList.Count > 0)
                {
                    foreach (var s in startNodeList)
                        nodeToExecute.Push(s);
                }

                currentGraphExecution = startNodeList.Count == 0 ? RunTheGraph() : RunTheGraph(nodeToExecute);
                currentGraphExecution.MoveNext();
            }
            else if (!currentGraphExecution.MoveNext())
            {
                currentGraphExecution = null;
            }
        }
    }
}
