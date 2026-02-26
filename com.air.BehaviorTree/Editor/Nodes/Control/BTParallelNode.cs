using System;
using GraphProcessor;
using UnityEngine;

namespace Air.BehaviorTree
{
    [Serializable, NodeMenuItem("Behavior Tree/Control/Parallel", typeof(BehaviorTreeGraph))]
    public class BTParallelNode : BTControlNode
    {
        public override Type RuntimeNodeType => typeof(RuntimeBTParallelNode);

        public override string name => "Parallel";

        [Input]
        [SerializeField]
        [AllowDefaultEdit]
        [ExportFieldsAsPorts]
        public ParallelNodeParamData ParallelNodeParamData = new() { SuccessThreshold = -1 };

        public override string GetExportJsonData()
        {
            return GetExportJsonData(ParallelNodeParamData);
        }
    }
}