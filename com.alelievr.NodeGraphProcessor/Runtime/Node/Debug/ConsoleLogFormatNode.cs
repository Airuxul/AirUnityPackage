using UnityEngine;
// ReSharper disable UnusedMember.Global

namespace GraphProcessor
{
    [System.Serializable, NodeMenuItem("Debug/Console Log Format")]
    public class ConsoleLogFormatNode : LinearConditionalNode
    {
        public override string name => "Console Log Format";

        [Input("Object")]
        public object obj;

        [Input("Log"), SerializeField]
        public string logText = "{0}";

        [Setting("Log Type")]
        public LogType logType = LogType.Log;

        protected override void Process()
        {
            var logStr = string.Format(logText, obj != null ? obj.ToString() : logText);
            switch (logType)
            {
                case LogType.Error:
                case LogType.Exception:
                    Debug.LogError(logStr);
                    break;
                case LogType.Assert:
                    Debug.LogAssertion(logStr);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(logStr);
                    break;
                case LogType.Log:
                    Debug.Log(logStr);
                    break;
            }
        }
    }
}
