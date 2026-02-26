using System;

namespace GraphProcessor
{
    [Serializable]
    class ParameterNodeParamData : NodeParamData
    {
        public string ParameterGUID;
        public int Accessor;
    }
}