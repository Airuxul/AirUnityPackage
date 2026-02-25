using UnityEngine;
using GraphProcessor;

namespace BehaviorTree
{
    /// <summary>
    /// Leaf node that logs a message and returns Success.
    /// </summary>
    public class RuntimeBTLogNode : RuntimeBTBaseNode
    {
        public string LogMessage { get; set; }

        public RuntimeBTLogNode(RuntimeGraph graph, NodeExportData exportData) : base(graph, exportData)
        {
            var nodeParamData = GetNodeParamDataFromJson<BTLogNodeParamData>(exportData.jsonData ?? "{}");
            LogMessage = nodeParamData.LogMessage;
        }

        protected override BehaviorTreeStatus OnUpdate()
        {
            var msg = string.IsNullOrEmpty(LogMessage) ? "Log" : LogMessage;
            Debug.Log(msg);
            return BehaviorTreeStatus.Success;
        }
    }
}
