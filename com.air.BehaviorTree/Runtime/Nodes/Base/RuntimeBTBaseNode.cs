using GraphProcessor;
using UnityEngine;

namespace Air.BehaviorTree
{
    /// <summary>
    /// Base class for behavior tree runtime nodes. Extends RuntimeBaseNode with BT status semantics.
    /// </summary>
    public abstract class RuntimeBTBaseNode : RuntimeBaseNode
    {
        protected RuntimeBTBaseNode(RuntimeGraph graph, NodeExportData exportData) : base(graph, exportData)
        {
        }

        public BehaviorTreeStatus Status { get; private set; }
        
        public float LastExecutionTime { get; private set; }

        /// <summary>
        /// When node first Execute
        /// </summary>
        protected virtual void OnInit() { }

        /// <summary>
        /// Execute the node and return its status.
        /// </summary>
        protected abstract BehaviorTreeStatus OnUpdate();
        
        /// <summary>
        /// ProcessGraphProcessor calls OnProcess; we delegate to OnExecute and store status.
        /// </summary>
        public override void OnProcess()
        {
            if (Status != BehaviorTreeStatus.Running)
            {
                OnInit();
            }
            Status = OnUpdate();
            LastExecutionTime = Time.time;
        }
    }
}
