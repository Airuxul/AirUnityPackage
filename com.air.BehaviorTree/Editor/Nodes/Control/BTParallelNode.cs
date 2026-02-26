using System;
using GraphProcessor;

namespace Air.BehaviorTree
{
    [Serializable, NodeMenuItem("Behavior Tree/Control/Parallel", typeof(BehaviorTreeGraph))]
    public class BTParallelNode : BTControlNode
    {
        public override Type RuntimeNodeType => typeof(RuntimeBTParallelNode);

        [Input]
        public int successThreshold = -1;
        
        public override string GetExportJsonData()
        {
            return GetExportJsonData(new ParallelNodeParamData {SuccessThreshold = successThreshold});
        }
    }
}