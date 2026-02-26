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

        [SerializeField]
        public string logMessage = "";

        public override string GetExportJsonData()
        {
            return GetExportJsonData(new LogNodeParamData { LogMessage = logMessage });
        }
    }
}
