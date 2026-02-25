using UnityEngine;
using GraphProcessor;

namespace BehaviorTree
{
    /// <summary>
    /// MonoBehaviour that runs a behavior tree from a JSON asset.
    /// </summary>
    public class BehaviorTreeRunner : MonoBehaviour
    {
        public enum PlayMode
        {
            Once,
            Update,
            FixedUpdate
        }

        [SerializeField]
        public TextAsset graphAsset;

        [SerializeField]
        public PlayMode playMode = PlayMode.Update;

        private RuntimeGraph _runtimeGraph;
        private BehaviorTreeProcessor _processor;

        private void Awake()
        {
            if (graphAsset == null)
            {
                Debug.LogWarning("BehaviorTreeRunner: No graph asset assigned.");
                return;
            }

            _runtimeGraph = RuntimeGraphBuilder.FromJson(graphAsset.text);
            _processor = new BehaviorTreeProcessor();
            _processor.Init(_runtimeGraph);
        }

        private void Start()
        {
            if (playMode == PlayMode.Once && _processor != null)
                _processor.Tick();
        }

        private void Update()
        {
            if (playMode == PlayMode.Update && _processor != null)
                _processor.Tick();
        }

        private void FixedUpdate()
        {
            if (playMode == PlayMode.FixedUpdate && _processor != null)
                _processor.Tick();
        }

        /// <summary>
        /// Manually tick the tree once.
        /// </summary>
        public BehaviorTreeStatus Tick() => _processor?.Tick() ?? BehaviorTreeStatus.Failure;
    }
}
