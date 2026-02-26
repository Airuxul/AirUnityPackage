using System;

namespace GraphProcessor
{
    [Serializable]
    public class RelayNodeParamData : NodeParamData
    {
        public bool PackInput;
        public bool UnpackOutput;
    }
}