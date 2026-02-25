using System;
using GraphProcessor;

namespace BehaviorTree
{
    [Serializable]
    public class BTLogNodeParamData : NodeParamData
    {
        public string LogMessage;
    }
}
