using UnityEngine;

namespace GraphProcessor
{
    public class RuntimeLogNode : RuntimeBaseNodeWithParam<LogNodeParamData>
    {
        public RuntimeLogNode(RuntimeGraph graph, NodeExportData exportData) : base(graph, exportData) { }

        public override void OnProcess()
        {
            var inputMessage = GetInputValue("inputMessage").ToString();
            if (string.IsNullOrEmpty(inputMessage))
            {
                inputMessage = ParamData.DefaltMessage;
            }
            Debug.Log(inputMessage);
        }
    }
}