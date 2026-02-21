using UnityEngine;
using GraphProcessor;

namespace Air.BehaviorTree
{
    /// <summary>
    /// MonoBehaviour that runs a behavior tree. Attach to a GameObject and assign a BehaviorTreeGraph.
    /// </summary>
    public class BehaviorTreeRunner : MonoBehaviour
    {
        [SerializeField]
        BehaviorTreeGraph behaviorTree;

        [SerializeField]
        [Tooltip("If true, tick every Update. If false, use manual Tick() calls.")]
        bool tickEveryFrame = true;

        BehaviorTreeProcessor processor;
        IBehaviorTreeContext context;

        /// <summary>
        /// The behavior tree graph to execute.
        /// </summary>
        public BehaviorTreeGraph BehaviorTree
        {
            get => behaviorTree;
            set
            {
                behaviorTree = value;
                InitializeProcessor();
            }
        }

        /// <summary>
        /// Context passed to nodes. Set before first tick if using custom context.
        /// </summary>
        public IBehaviorTreeContext Context
        {
            get => context;
            set => context = value;
        }

        void Awake()
        {
            InitializeProcessor();
        }

        void Update()
        {
            if (tickEveryFrame && processor != null)
                processor.Tick();
        }

        /// <summary>
        /// Manually tick the behavior tree once.
        /// </summary>
        /// <returns>Status of the root execution.</returns>
        public BTStatus Tick()
        {
            return processor?.Tick() ?? BTStatus.Failure;
        }

        void InitializeProcessor()
        {
            if (behaviorTree == null)
            {
                processor = null;
                return;
            }

            context ??= new DefaultBehaviorTreeContext(gameObject);
            processor = new BehaviorTreeProcessor(behaviorTree, context);
        }
    }

    /// <summary>
    /// Default context implementation. Can be extended for game-specific data.
    /// </summary>
    public class DefaultBehaviorTreeContext : IBehaviorTreeContext
    {
        public GameObject GameObject { get; }
        public Transform Transform { get; }

        public DefaultBehaviorTreeContext(GameObject gameObject)
        {
            GameObject = gameObject;
            Transform = gameObject.transform;
        }
    }
}
