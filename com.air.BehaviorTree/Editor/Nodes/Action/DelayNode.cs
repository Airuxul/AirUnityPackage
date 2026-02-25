using System;
using UnityEngine;
using GraphProcessor;

namespace BehaviorTree
{
    [Serializable, NodeMenuItem("Behavior Tree/Actions/Delay", typeof(BehaviorTreeGraph))]
    public class DelayNode : ActionNode
    {
        public override Type RuntimeNodeType => typeof(RuntimeBTDelayNode);

        public override string name => "Delay";

        [SerializeField]
        public int delayTicks = 24;

        public override string GetExportJsonData()
        {
            return JsonUtility.ToJson(new DelayNodeExportData { delayTicks = delayTicks });
        }
    }
}
