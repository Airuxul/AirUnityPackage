using UnityEngine;

namespace GraphProcessor
{
    public class RuntimeLogNode : RuntimeBaseNode
    {
        public string DefaltMessage { get; set; }
        
        public RuntimeLogNode(RuntimeGraph graph, NodeExportData exportData) : base(graph, exportData)
        {
            var nodeParamData = GetNodeParamDataFromJson<LogNodeParamData>(exportData.jsonData ?? "{}");
            DefaltMessage = nodeParamData.DefaltMessage;
        }

        public override void OnProcess()
        {
            var inputMessage = GetInputValue("inputMessage").ToString();
            if (string.IsNullOrEmpty(inputMessage))
            {
                inputMessage = DefaltMessage;
            }
            Debug.Log(inputMessage);
        }
    }
}