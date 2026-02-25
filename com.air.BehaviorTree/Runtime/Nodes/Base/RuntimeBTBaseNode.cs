using GraphProcessor;

namespace BehaviorTree
{
    /// <summary>
    /// Base class for behavior tree runtime nodes. Extends RuntimeBaseNode with BT status semantics.
    /// </summary>
    public abstract class RuntimeBTBaseNode : RuntimeBaseNode
    {
        protected RuntimeBTBaseNode(RuntimeGraph graph) : base(graph)
        {
        }

        /// <summary>
        /// When node first Execute
        /// </summary>
        protected virtual void OnInit() { }

        /// <summary>
        /// Execute the node and return its status.
        /// </summary>
        public abstract BehaviorTreeStatus OnUpdate();
        
        /// <summary>
        /// ProcessGraphProcessor calls OnProcess; we delegate to OnExecute and store status.
        /// </summary>
        public override void OnProcess()
        {
            if (Status != BehaviorTreeStatus.Running)
                OnInit();
            Status = OnUpdate();
        }

        /// <summary>
        /// Last execution status. Used by parent composite nodes.
        /// </summary>
        public BehaviorTreeStatus Status { get; protected set; }


    }
}
