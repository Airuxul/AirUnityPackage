using System;
using System.Collections.Generic;

namespace GraphProcessor
{
    /// <summary>
    /// JSON-serializable export data for graph. Used for BaseGraph export and RuntimeGraph import.
    /// </summary>
    [Serializable]
    public class GraphExportData
    {
        public List<NodeExportData> nodes = new List<NodeExportData>();
        public List<EdgeExportData> edges = new List<EdgeExportData>();
        public List<ExposedParameterExportData> exposedParameters = new List<ExposedParameterExportData>();
    }

    [Serializable]
    public class NodeExportData
    {
        public string guid;
        public string type;
        public string runtimeNodeType;
        public int computeOrder;
        public string jsonData;
    }

    [Serializable]
    public class EdgeExportData
    {
        public string guid;
        public string inputNodeGUID;
        public string outputNodeGUID;
        public string inputFieldName;
        public string outputFieldName;
        public string inputPortIdentifier;
        public string outputPortIdentifier;
    }

    [Serializable]
    public class ExposedParameterExportData
    {
        public string guid;
        public string name;
        public string type;
        public string jsonValue;
    }
}
