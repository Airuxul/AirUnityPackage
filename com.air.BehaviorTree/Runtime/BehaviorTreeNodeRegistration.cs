using System;
using UnityEngine;
using GraphProcessor;

namespace BehaviorTree
{
    /// <summary>
    /// Registers behavior tree node creators with RuntimeGraphBuilder at startup.
    /// </summary>
    public static class BehaviorTreeNodeRegistration
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Register()
        {
            RuntimeGraphBuilder.RegisterNodeCreator(typeof(RuntimeRootBaseNode), CreateRootNode);
            RuntimeGraphBuilder.RegisterNodeCreator(typeof(RuntimeBTSequenceNode), CreateSequenceNode);
            RuntimeGraphBuilder.RegisterNodeCreator(typeof(RuntimeBTSelectorNode), CreateSelectorNode);
            RuntimeGraphBuilder.RegisterNodeCreator(typeof(RuntimeBTLogNode), CreateLogNode);
            RuntimeGraphBuilder.RegisterNodeCreator(typeof(RuntimeBTDelayNode), CreateDelayNode);
        }

        static RuntimeRootBaseNode CreateRootNode(RuntimeGraph graph, NodeExportData data)
        {
            var node = new RuntimeRootBaseNode(graph)
            {
                GUID = data.guid,
                Order = data.computeOrder
            };
            return node;
        }

        static RuntimeBTSequenceNode CreateSequenceNode(RuntimeGraph graph, NodeExportData data)
        {
            var node = new RuntimeBTSequenceNode(graph)
            {
                GUID = data.guid,
                Order = data.computeOrder
            };
            return node;
        }

        static RuntimeBTSelectorNode CreateSelectorNode(RuntimeGraph graph, NodeExportData data)
        {
            var node = new RuntimeBTSelectorNode(graph)
            {
                GUID = data.guid,
                Order = data.computeOrder
            };
            return node;
        }

        static RuntimeBTLogNode CreateLogNode(RuntimeGraph graph, NodeExportData data)
        {
            var export = JsonUtility.FromJson<LogNodeExportData>(data.jsonData ?? "{}");
            var node = new RuntimeBTLogNode(graph)
            {
                GUID = data.guid,
                Order = data.computeOrder,
                LogMessage = export?.logMessage ?? ""
            };
            return node;
        }

        static RuntimeBTDelayNode CreateDelayNode(RuntimeGraph graph, NodeExportData data)
        {
            var export = JsonUtility.FromJson<DelayNodeExportData>(data.jsonData ?? "{}");
            var node = new RuntimeBTDelayNode(graph)
            {
                GUID = data.guid,
                Order = data.computeOrder,
                DelayTicks = export?.delayTicks ?? 24
            };
            return node;
        }
    }
}
