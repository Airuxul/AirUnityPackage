using System;
using UnityEngine;
using GraphProcessor;

namespace BehaviorTree
{
    [Serializable, NodeMenuItem("Behavior Tree/Actions/Wait", typeof(BehaviorTreeGraph))]
    public class WaitNode : ActionNode
    {
        public override Type RuntimeNodeType => typeof(RuntimeBTWaitNode);

        public override string name => "Wait";

        [SerializeField]
        [Input]
        public int waitTicks = 24;

        public override string GetExportJsonData()
        {
            return JsonUtility.ToJson(new BTWaitNodeParamData { WaitTicks = waitTicks });
        }
    }
}
