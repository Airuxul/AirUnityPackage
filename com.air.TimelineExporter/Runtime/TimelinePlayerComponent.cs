using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEngine.Playables;
using UnityEngine.Timeline;
#endif

namespace TimelineExporter
{
    /// <summary>
    /// MonoBehaviour wrapper for TimelinePlayer. Load TimelineData and drive Update.
    /// For ControlTrack: add bindings (key = exported sourceBindingKey from ControlClipData).
    /// </summary>
    public class TimelinePlayerComponent : MonoBehaviour
    {
        [SerializeField] private TextAsset timelineJson;
        [SerializeField] private BindingEntry[] bindings = Array.Empty<BindingEntry>();

        private TimelinePlayer player;

#if UNITY_EDITOR
        private PlayableDirector previewDirector;
        public PlayableDirector PreviewDirector => previewDirector;
        public TimelineAsset ResolvedPreviewAsset { get; private set; }
#endif
        
        [Serializable]
        public struct BindingEntry
        {
            public string key;
            public GameObject target;
        }

        public TimelinePlayer Player => player;

        private void Start()
        {
            TimelineData data = timelineJson != null ? TimelineDataLoader.LoadFromJson(timelineJson.text) : null;

            if (data == null) return;

            player = new TimelinePlayer(data)
            {
                BindingRoot = transform
            };

            foreach (var b in bindings)
            {
                if (!string.IsNullOrEmpty(b.key) && b.target != null)
                    player.Bindings[b.key] = b.target;
            }

            PopulateBindingsFromHierarchy(transform, player.Bindings);

            player.RegisterBehaviour("CustomClip", new CustomRuntimeBehaviour());
            player.RegisterBehaviour("ControlPlayableAsset", new ControlBehaviour());
            player.RegisterBehaviour("AnimationPlayableAsset", new AnimationBehaviour());
            player.RegisterTrackBlendHandler(new AnimationTrackBlendHandler());
            player.Play();

#if UNITY_EDITOR
            ResolvedPreviewAsset = TimelineAssetLoader.Load(data);
            if (ResolvedPreviewAsset != null)
            {
                previewDirector = GetComponent<PlayableDirector>();
                if (previewDirector == null)
                    previewDirector = gameObject.AddComponent<PlayableDirector>();
                previewDirector.playOnAwake = false;
                previewDirector.playableAsset = ResolvedPreviewAsset;
                previewDirector.timeUpdateMode = DirectorUpdateMode.Manual;
                previewDirector.extrapolationMode = DirectorWrapMode.Hold;
            }
#else
            // 非编辑器下，移除预览用的PlayableDirector
            var _previewDirector = GetComponent<PlayableDirector>();
            Destroy(_previewDirector);
#endif
        }

        private void Update()
        {
            player?.Update(Time.deltaTime);

#if UNITY_EDITOR
            if (previewDirector != null && player != null)
            {
                previewDirector.time = player.CurrentTime;
            }
#endif
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            if (previewDirector != null && previewDirector.gameObject == gameObject)
            {
                Destroy(previewDirector);
            }
#endif
        }

        private static void PopulateBindingsFromHierarchy(Transform root, Dictionary<string, GameObject> bindings)
        {
            if (root == null || bindings == null) return;
            for (var i = 0; i < root.childCount; i++)
            {
                var child = root.GetChild(i);
                if (!string.IsNullOrEmpty(child.name) && !bindings.ContainsKey(child.name))
                    bindings[child.name] = child.gameObject;
                PopulateBindingsFromHierarchy(child, bindings);
            }
        }
    }
}
