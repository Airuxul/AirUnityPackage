using System;

namespace GraphProcessor
{
    [Serializable]
    public class ParameterNodeParamData : NodeParamData
    {
        public string ParameterGUID;
        public int Accessor;
    }
}