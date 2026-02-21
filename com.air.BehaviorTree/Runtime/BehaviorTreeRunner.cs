using UnityEngine;

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
        
        [SerializeField]
        [Tooltip("If true, record execution state for editor debug visualization.")]
        bool enableDebug = true;

        BehaviorTreeProcessor processor;
        IBehaviorTreeContext context;
        BehaviorTreeDebugState debugState;

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

        /// <summary>
        /// Debug state for editor visualization. Only valid when enableDebug is true and after first tick.
        /// </summary>
        public BehaviorTreeDebugState DebugState => debugState;

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
                debugState = null;
                return;
            }

            var defaultContext = new DefaultBehaviorTreeContext(gameObject);
            if (enableDebug)
            {
                debugState = new BehaviorTreeDebugState();
                context = new DebugBehaviorTreeContext(defaultContext, debugState);
            }
            else
            {
                debugState = null;
                context = defaultContext;
            }

            processor = new BehaviorTreeProcessor(behaviorTree, context);
        }
    }
}
