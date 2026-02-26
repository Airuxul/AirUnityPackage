using System;

namespace GraphProcessor
{
    [Serializable]
    public class LogNode : BaseNode
    {
        public override Type RuntimeNodeType => typeof(RuntimeLogNode);

        public override string name => "Log";
        
        [Input]
        [AllowDefaultEdit]
        [ExportFieldsAsPorts]
        public LogNodeParamData logNodeParamData = new (){DefaltMessage = "Default log message" };
        
        public override string GetExportJsonData()
        {
            return GetExportJsonData(logNodeParamData);
        }
    }
}