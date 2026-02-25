using GraphProcessor;

namespace BehaviorTree
{
    /// <summary>
    /// Leaf node that delays for a number of ticks, then returns Success.
    /// </summary>
    public class RuntimeBTDelayNode : RuntimeBTBaseNode
    {
        public int DelayTicks { get; set; }

        private int _elapsedTicks;

        public RuntimeBTDelayNode(RuntimeGraph graph) : base(graph)
        {
        }

        protected override void OnInit()
        {
            _elapsedTicks = 0;
        }

        protected override BehaviorTreeStatus OnUpdate()
        {
            _elapsedTicks++;
            return _elapsedTicks >= DelayTicks ? BehaviorTreeStatus.Success : BehaviorTreeStatus.Running;
        }
    }
}
