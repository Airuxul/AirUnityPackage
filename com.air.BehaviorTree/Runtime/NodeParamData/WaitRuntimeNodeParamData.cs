using System;
using GraphProcessor;

namespace BehaviorTree
{
    [Serializable]
    public class BTWaitNodeParamData : NodeParamData
    {
        public int WaitTicks;
    }
}
