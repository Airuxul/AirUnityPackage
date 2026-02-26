using System;
using GraphProcessor;

namespace Air.BehaviorTree
{
    [Serializable]
    public class LogNodeParamData : NodeParamData
    {
        public string LogMessage;
    }
}
