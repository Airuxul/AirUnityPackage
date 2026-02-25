using System;
using UnityEngine;
using GraphProcessor;

namespace BehaviorTree
{
    [Serializable, NodeMenuItem("Behavior Tree/Actions/Log", typeof(BehaviorTreeGraph))]
    public class LogNode : ActionNode
    {
        public override Type RuntimeNodeType => typeof(RuntimeBTLogNode);

        public override string name => "Log";

        [SerializeField]
        public string logMessage = "";

        public override string GetExportJsonData()
        {
            return JsonUtility.ToJson(new BTLogNodeParamData { LogMessage = logMessage });
        }
    }
}
