using System;
using GraphProcessor;

namespace Air.BehaviorTree
{
    [Serializable]
    public class ParallelNodeParamData : NodeParamData
    {
        public int SuccessThreshold;
    }
}