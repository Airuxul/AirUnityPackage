using GraphProcessor;

namespace Air.BehaviorTree
{
    /// <summary>
    /// Leaf node that delays for a number of ticks, then returns Success.
    /// </summary>
    public class RuntimeBTWaitNode : RuntimeBTBaseNode
    {
        public int WaitTicks { get; set; }

        private int _elapsedTicks;

        public RuntimeBTWaitNode(RuntimeGraph graph, NodeExportData exportData) : base(graph, exportData)
        {
            var nodeParamData = GetNodeParamDataFromJson<WaitNodeParamData>(exportData.jsonData ?? "{}");
            WaitTicks = nodeParamData.WaitTicks;
            // 注册输入监听
            RegisterInputPort<int>("waitTicks", v => WaitTicks = v);
        }

        protected override void OnInit()
        {
            _elapsedTicks = 0;
        }

        protected override BehaviorTreeStatus OnUpdate()
        {
            _elapsedTicks++;
            return _elapsedTicks >= WaitTicks ? BehaviorTreeStatus.Success : BehaviorTreeStatus.Running;
        }
    }
}
