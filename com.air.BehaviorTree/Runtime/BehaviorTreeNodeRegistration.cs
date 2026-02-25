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
            RuntimeGraphBuilder.RegisterNodeCreator(typeof(RuntimeBTWaitNode), CreateDelayNode);
        }

        static RuntimeRootBaseNode CreateRootNode(RuntimeGraph graph, NodeExportData data)
        {
            return new RuntimeRootBaseNode(graph, data);
        }

        static RuntimeBTSequenceNode CreateSequenceNode(RuntimeGraph graph, NodeExportData data)
        {
            return new RuntimeBTSequenceNode(graph, data);
        }

        static RuntimeBTSelectorNode CreateSelectorNode(RuntimeGraph graph, NodeExportData data)
        {
            return new RuntimeBTSelectorNode(graph, data);
        }

        static RuntimeBTLogNode CreateLogNode(RuntimeGraph graph, NodeExportData data)
        {
            return new RuntimeBTLogNode(graph, data);
        }

        static RuntimeBTWaitNode CreateDelayNode(RuntimeGraph graph, NodeExportData data)
        {
            return new RuntimeBTWaitNode(graph, data);
        }
    }
}
