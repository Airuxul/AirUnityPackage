using System;
using UnityEngine;

namespace GraphProcessor
{
    [Serializable]
    public class LogNode : BaseNode
    {
        public override Type RuntimeNodeType => typeof(RuntimeLogNode);

        public override string name => "Log";
        
        [Input] 
        public string inputMessage;
        
        public string defaltMessage = "Default log message";
        
        public override string GetExportJsonData()
        {
            return GetExportJsonData(new LogNodeParamData
            {
                DefaltMessage = defaltMessage
            });
        }
    }
}