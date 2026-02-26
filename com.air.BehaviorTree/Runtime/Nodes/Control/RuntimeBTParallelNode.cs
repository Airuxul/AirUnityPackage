using GraphProcessor;

namespace Air.BehaviorTree
{
    public class RuntimeBTParallelNode : RuntimeBTControlNode
    {
        private readonly int SuccessThreshold;
        
        public RuntimeBTParallelNode(RuntimeGraph graph, NodeExportData exportData) : base(graph, exportData)
        {
            var nodeParamData = GetNodeParamDataFromJson<ParallelNodeParamData>(exportData.jsonData ?? "{}");
            SuccessThreshold = nodeParamData.SuccessThreshold;
        }

        protected override BehaviorTreeStatus OnUpdate()
        {
            int successCount = 0;
            bool anyRunning = false;
            foreach (var child in GetChildren())
            {
                child.OnProcess();
                if (child.Status == BehaviorTreeStatus.Success) successCount++;
                if (child.Status == BehaviorTreeStatus.Running) anyRunning = true;
            }
            if (SuccessThreshold > 0 && successCount >= SuccessThreshold) return BehaviorTreeStatus.Success;
            if (anyRunning) return BehaviorTreeStatus.Running;
            return BehaviorTreeStatus.Failure;
        }
    }
}