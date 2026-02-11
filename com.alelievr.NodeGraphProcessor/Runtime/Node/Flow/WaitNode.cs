using System;
using System.Collections;
using UnityEngine;
// ReSharper disable UnusedMember.Global

namespace GraphProcessor
{
    [System.Serializable, NodeMenuItem("Flow/Wait")]
    public class WaitNode : WaitableNode
    {
        public override string name => "Wait";

        [SerializeField, Input(name = "Seconds")]
        public float waitTime = 1f;

        private static WaitMonoBehaviour waitMonoBehaviour;

        protected override void Process()
        {
            if (waitMonoBehaviour == null)
            {
                var go = new GameObject("WaitGameObject");
                waitMonoBehaviour = go.AddComponent<WaitMonoBehaviour>();
            }

            waitMonoBehaviour.Process(waitTime, ProcessFinished);
        }
    }

    public class WaitMonoBehaviour : MonoBehaviour
    {
        public void Process(float time, Action callback)
        {
            StartCoroutine(ProcessCoroutine(time, callback));
        }

        private IEnumerator ProcessCoroutine(float time, Action callback)
        {
            yield return new WaitForSeconds(time);
            callback.Invoke();
        }
    }
}
