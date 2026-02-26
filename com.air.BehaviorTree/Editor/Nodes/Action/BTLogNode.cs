using System;
using UnityEngine;
using GraphProcessor;

namespace Air.BehaviorTree
{
    [Serializable, NodeMenuItem("Behavior Tree/Actions/Log", typeof(BehaviorTreeGraph))]
    public class BTLogNode : BTActionNode
    {
        public override Type RuntimeNodeType => typeof(RuntimeBTLogNode);

        public override string name => "Log";

        [Input]
        [SerializeField]
        [AllowDefaultEdit]
        [ExportFieldsAsPorts]
        public LogNodeParamData LogNodeParamData = new() { LogMessage = "" };

        public override string GetExportJsonData()
        {
            return GetExportJsonData(LogNodeParamData);
        }
    }
}
