using System;
using UnityEngine;
using GraphProcessor;

namespace Air.BehaviorTree
{
    [Serializable, NodeMenuItem("Behavior Tree/Actions/Wait", typeof(BehaviorTreeGraph))]
    public class BTWaitNode : BTActionNode
    {
        public override Type RuntimeNodeType => typeof(RuntimeBTWaitNode);

        public override string name => "Wait";

        [Input]
        [SerializeField]
        [AllowDefaultEdit]
        [ExportFieldsAsPorts]
        public WaitNodeParamData WaitNodeParamData = new() {WaitTicks = 24};

        public override string GetExportJsonData()
        {
            return GetExportJsonData(WaitNodeParamData);
        }
    }
}
