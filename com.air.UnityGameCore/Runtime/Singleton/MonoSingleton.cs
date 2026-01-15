using UnityEngine;

namespace UntiyGameCore.Runtime.Singleton
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static bool HasInstance => _instance != null;
        public static T TryGetInstance() => HasInstance ? _instance : null;

        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;
#if UNITY_2022_3_OR_NEWER
                _instance = FindAnyObjectByType<T>();
#else
                _instance = FindObjectOfType<T>();
#endif
                if (_instance != null) return _instance;
                var go = new GameObject(typeof(T).Name + " Auto-Generated");
                _instance = go.AddComponent<T>();

                return _instance;
            }
        }

        /// <summary>
        /// Make sure to call base.Awake() in override if you need awake.
        /// </summary>
        protected virtual void Awake()
        {
            InitializeSingleton();
        }

        protected virtual void InitializeSingleton()
        {
            if (!Application.isPlaying) return;

            _instance = this as T;
        }
    }
}