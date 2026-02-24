using UnityEngine;

namespace GraphProcessor
{
    public class BaseGraphRunner : MonoBehaviour
    {
        public enum EPlayMode
        {
            Once,
            Update,
            FixedUpdate,
        }
        
        public TextAsset graphAsset;
        
        public EPlayMode playMode = EPlayMode.Update;

        protected RuntimeGraph _runtimeGraph;

        protected BaseRuntimeGraphProcessor _graphProcessor;

        private void Awake()
        {
            _runtimeGraph = RuntimeGraphBuilder.FromJson(graphAsset.text);
            _graphProcessor = new ProcessGraphProcessor();
            _graphProcessor.InitRuntimeGraph(_runtimeGraph);
        }

        private void Start()
        {
            if(playMode == EPlayMode.Once)
                _graphProcessor.Run();
        }

        private void Update()
        {
            if(playMode == EPlayMode.Update)
                _graphProcessor.Run();
        }

        private void FixedUpdate()
        {
            if(playMode == EPlayMode.FixedUpdate)
                _graphProcessor.Run();
        }
    }
}