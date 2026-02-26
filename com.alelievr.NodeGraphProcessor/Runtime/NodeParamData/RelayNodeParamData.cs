using System;

namespace GraphProcessor
{
    [Serializable]
    class RelayNodeParamData : NodeParamData
    {
        public bool PackInput;
        public bool UnpackOutput;
    }
}