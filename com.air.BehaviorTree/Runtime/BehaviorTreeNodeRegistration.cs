using UnityEngine;
using GraphProcessor;

namespace Air.BehaviorTree
{
    /// <summary>
    /// Registers behavior tree node creators with RuntimeGraphBuilder at startup.
    /// </summary>
    public static class BehaviorTreeNodeRegistration
    {
        // 也可以通过反射的方式，这样性能更友好些
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Register()
        {
            RuntimeGraphBuilder.RegisterNodeCreator(typeof(RuntimeRootBaseNode), (graph, data) => new RuntimeRootBaseNode(graph, data));
            RuntimeGraphBuilder.RegisterNodeCreator(typeof(RuntimeBTSequenceNode), (graph, data) => new RuntimeBTSequenceNode(graph, data));
            RuntimeGraphBuilder.RegisterNodeCreator(typeof(RuntimeBTSelectorNode), (graph, data) => new RuntimeBTSelectorNode(graph, data));
            RuntimeGraphBuilder.RegisterNodeCreator(typeof(RuntimeBTParallelNode), (graph, data) => new RuntimeBTParallelNode(graph, data));
            RuntimeGraphBuilder.RegisterNodeCreator(typeof(RuntimeBTLogNode), (graph, data) => new RuntimeBTLogNode(graph, data));
            RuntimeGraphBuilder.RegisterNodeCreator(typeof(RuntimeBTWaitNode), (graph, data) => new RuntimeBTWaitNode(graph, data));
            RuntimeGraphBuilder.RegisterNodeCreator(typeof(RuntimeBTInvertNode), (graph, data) => new RuntimeBTInvertNode(graph, data));
        }
    }
}
