using GraphProcessor;

namespace BehaviorTree
{
    /// <summary>
    /// Executes a behavior tree by traversing from root. Uses tree structure, not compute order.
    /// </summary>
    public class BehaviorTreeProcessor
    {
        private RuntimeGraph _graph;
        private RuntimeRootBaseNode _rootBase;

        public void Init(RuntimeGraph graph)
        {
            _graph = graph;
            _rootBase = FindRoot();
            InitParameterNodes();
        }

        /// <summary>
        /// Process ParameterNodes with Accessor=0 (Get) to push ExposedParameters to connected input nodes.
        /// </summary>
        void InitParameterNodes()
        {
            foreach (var node in _graph.Guid2Nodes.Values)
            {
                if (node is RuntimeParameterNode param && param.ParameterAccessor == 0)
                    param.OnProcess();
            }
        }

        /// <summary>
        /// Execute one tick of the behavior tree.
        /// </summary>
        public BehaviorTreeStatus Tick()
        {
            if (_rootBase == null)
                return BehaviorTreeStatus.Failure;

            _rootBase.OnProcess();
            return _rootBase.Status;
        }

        RuntimeRootBaseNode FindRoot()
        {
            foreach (var node in _graph.Guid2Nodes.Values)
            {
                if (node is RuntimeRootBaseNode root)
                    return root;
            }
            return null;
        }
    }
}
