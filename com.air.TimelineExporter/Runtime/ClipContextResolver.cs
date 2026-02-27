using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

namespace TimelineExporter
{
    /// <summary>
    /// Generic resolver for clip context: resource path lookup, GameObject binding, transform search, CustomData parsing.
    /// Used by ControlTrackBehaviour, AnimationTrackBehaviour and other behaviours.
    /// </summary>
    public static class ClipContextResolver
    {
        /// <summary>
        /// Gets resource path from TimelineData.Resources by refId.
        /// </summary>
        public static string GetResourcePath(TimelineData data, string refId)
        {
            if (data?.Resources == null || string.IsNullOrEmpty(refId)) return "";
            foreach (var r in data.Resources)
            {
                if (r.Id == refId)
                    return r.ResourcePath ?? "";
            }
            return "";
        }

        /// <summary>
        /// Resolves GameObject from IClipContext. Tries bindings first, then BindingRoot.Find, then FindInChildren.
        /// </summary>
        public static GameObject ResolveGameObject(IClipContext clipCtx, params string[] bindingKeys)
        {
            if (clipCtx == null) return null;
            foreach (var key in bindingKeys)
            {
                if (string.IsNullOrEmpty(key)) continue;
                var go = TryResolveKey(clipCtx, key);
                if (go != null) return go;
            }
            return null;
        }

        private static GameObject TryResolveKey(IClipContext clipCtx, string key)
        {
            if (clipCtx.Bindings != null && clipCtx.Bindings.TryGetValue(key, out var go)) return go;
            if (clipCtx.BindingRoot == null) return null;
            var t = clipCtx.BindingRoot.Find(key) ?? FindInChildren(clipCtx.BindingRoot, key);
            return t?.gameObject;
        }

        /// <summary>
        /// Finds transform by name in hierarchy (depth-first).
        /// </summary>
        public static Transform FindInChildren(Transform root, string name)
        {
            if (root == null || string.IsNullOrEmpty(name)) return null;
            if (root.name == name) return root;
            for (int i = 0; i < root.childCount; i++)
            {
                var found = FindInChildren(root.GetChild(i), name);
                if (found != null) return found;
            }
            return null;
        }

        /// <summary>
        /// Parses CustomData JSON to typed object.
        /// </summary>
        public static T ParseCustomData<T>(TimelineClipData clip) where T : class
        {
            if (string.IsNullOrEmpty(clip?.CustomData)) return null;
            try
            {
                return JsonUtility.FromJson<T>(clip.CustomData);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Loads AnimationClip for playback. Uses animationClipRefId for standalone, or extracts from Timeline for sub-assets.
        /// </summary>
        public static AnimationClip ResolveAnimationClip(TimelineData data, AnimationClipData clipData, TimelineAsset cachedTimeline = null)
        {
            if (data == null || clipData == null) return null;

            if (!string.IsNullOrEmpty(clipData.animationClipRefId))
            {
                var resourcePath = GetResourcePath(data, clipData.animationClipRefId);
                if (!string.IsNullOrEmpty(resourcePath))
                {
                    var clip = Resources.Load<AnimationClip>(resourcePath);
                    if (clip != null) return clip;
                }
            }

            if (clipData.trackBindingIndex >= 0 && clipData.clipIndex >= 0)
            {
                var timeline = cachedTimeline ?? TimelineAssetLoader.Load(data);
                return TimelineAssetLoader.GetAnimationClipFromTimeline(timeline, clipData.trackBindingIndex, clipData.clipIndex);
            }

            return null;
        }

        /// <summary>
        /// Loads prefab from Resources and instantiates under BindingRoot. Registers binding on success.
        /// </summary>
        public static GameObject TryInstantiatePrefab(IClipContext clipCtx, string prefabRefId, string prefabResourcePath, params string[] bindingKeys)
        {
            if (clipCtx?.BindingRoot == null) return null;

            var resourcePath = !string.IsNullOrEmpty(prefabRefId) && clipCtx.TimelineData != null
                ? GetResourcePath(clipCtx.TimelineData, prefabRefId)
                : prefabResourcePath;
            if (string.IsNullOrEmpty(resourcePath)) return null;

            var prefab = Resources.Load<GameObject>(resourcePath);
            if (prefab == null) return null;

            var instance = Object.Instantiate(prefab, clipCtx.BindingRoot);
            var key = bindingKeys.Length > 0 ? bindingKeys[0] : "prefab";
            for (int i = 0; i < bindingKeys.Length; i++)
            {
                if (!string.IsNullOrEmpty(bindingKeys[i]))
                {
                    key = bindingKeys[i];
                    break;
                }
            }
            clipCtx.TryRegisterBinding(key, instance);
            return instance;
        }

        /// <summary>
        /// Tries to get SubTimelinePlayer by binding keys.
        /// </summary>
        public static bool TryGetSubPlayer(IReadOnlyDictionary<string, TimelinePlayer> subPlayers, out TimelinePlayer player, params string[] bindingKeys)
        {
            player = null;
            if (subPlayers == null) return false;
            foreach (var key in bindingKeys)
            {
                if (!string.IsNullOrEmpty(key) && subPlayers.TryGetValue(key, out player))
                    return true;
            }
            return false;
        }
    }
}
