using BehaviorTree.Action;
using GraphProcessor;

namespace BehaviorTree
{
    /// <summary>
    /// Sequence node: executes children in order. Fails on first failure; succeeds when all succeed.
    /// </summary>
    public class RuntimeBTSequenceNode : RuntimeBTControlNode
    {
        private int current;
        
        public RuntimeBTSequenceNode(RuntimeGraph graph) : base(graph)
        {
        }

        protected override void OnInit()
        {
            current = 0;
        }

        protected override BehaviorTreeStatus OnUpdate()
        {
            var children = GetChildren();
            while(current < children.Count)
            {
                var curNode = children[current];
                curNode.OnProcess();
                if (curNode.Status != BehaviorTreeStatus.Success)
                {
                    return curNode.Status;
                }
                current += 1;
            }
            return BehaviorTreeStatus.Success;
        }
    }
}
