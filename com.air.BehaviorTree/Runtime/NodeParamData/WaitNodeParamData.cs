using System;
using GraphProcessor;

namespace Air.BehaviorTree
{
    [Serializable]
    public class WaitNodeParamData : NodeParamData
    {
        public int WaitTicks;
    }
}
