using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

namespace TimelineExporter.Editor
{
    /// <summary>
    /// Exports TimelineClip to TimelineClipData. Handles clip-type-specific CustomData (Control, Animation, etc.).
    /// </summary>
    public static class TimelineExporterClip
    {
        /// <summary>
        /// Exports a TimelineClip to TimelineClipData.
        /// </summary>
        public static TimelineClipData Export(TimelineClip clip, TrackAsset track, int trackIndex, int clipIndex,
            Dictionary<string, TimelineResourceData> resourceMap)
        {
            var clipData = new TimelineClipData
            {
                Id = clip.displayName ?? clip.asset?.name ?? "Clip",
                StartTime = clip.start,
                Duration = clip.duration,
                ClipIn = clip.clipIn,
                TimeScale = clip.timeScale,
                MixInDuration = clip.hasBlendIn ? clip.blendInDuration : clip.easeInDuration,
                MixOutDuration = clip.hasBlendOut ? clip.blendOutDuration : clip.easeOutDuration,
                ClipType = clip.asset?.GetType().Name ?? "Unknown",
                DisplayName = clip.displayName
            };

            if (clip.asset != null)
            {
                var refId = AddOrGetResource(clip.asset, "ClipAsset", resourceMap);
                clipData.ReferenceIds.Add(refId);

                if (clip.asset is ControlPlayableAsset controlAsset)
                    clipData.CustomData = ExportControlClipData(controlAsset, track, resourceMap);
                else if (clip.asset is AnimationPlayableAsset animAsset)
                    clipData.CustomData = ExportAnimationClipData(animAsset, track, trackIndex, clipIndex, resourceMap);
            }

            return clipData;
        }

        private static string AddOrGetResource(Object obj, string resourceType,
            Dictionary<string, TimelineResourceData> resourceMap)
        {
            if (obj == null) return "";
            var path = AssetDatabase.GetAssetPath(obj);
            var guid = !string.IsNullOrEmpty(path) ? AssetDatabase.AssetPathToGUID(path) : "";
            var refId = !string.IsNullOrEmpty(guid) ? guid : $"inst_{obj.GetInstanceID()}";

            if (!resourceMap.ContainsKey(refId))
            {
                var resourcePath = GetResourcePathFromObject(obj);
                resourceMap[refId] = new TimelineResourceData
                {
                    Id = refId,
                    AssetPath = path ?? "",
                    AssetType = obj.GetType().FullName,
                    Guid = guid,
                    ResourcePath = resourcePath,
                    ResourceType = resourceType
                };
            }

            return refId;
        }

        private static string ExportAnimationClipData(AnimationPlayableAsset asset, TrackAsset track,
            int trackIndex, int clipIndex, Dictionary<string, TimelineResourceData> resourceMap)
        {
            string animationClipRefId = "";
            var animClip = asset.clip;
            if (animClip != null)
            {
                var clipPath = AssetDatabase.GetAssetPath(animClip);
                var assetPath = AssetDatabase.GetAssetPath(asset);
                var isSubAsset = !string.IsNullOrEmpty(clipPath) && clipPath == assetPath;
                if (!isSubAsset && !string.IsNullOrEmpty(clipPath) &&
                    clipPath.IndexOf("/Resources/", System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    animationClipRefId = AddOrGetResource(animClip, "AnimationClip", resourceMap);
                }
            }

            var animData = new AnimationClipData
            {
                trackName = track.name,
                animationClipRefId = animationClipRefId,
                trackBindingIndex = string.IsNullOrEmpty(animationClipRefId) ? trackIndex : -1,
                clipIndex = string.IsNullOrEmpty(animationClipRefId) ? clipIndex : -1,
                position = asset.position,
                eulerAngles = asset.eulerAngles,
                removeStartOffset = asset.removeStartOffset,
                applyFootIK = asset.applyFootIK,
                loop = (int)asset.loop
            };
            return JsonUtility.ToJson(animData);
        }

        private static string ExportControlClipData(ControlPlayableAsset asset, TrackAsset track,
            Dictionary<string, TimelineResourceData> resourceMap)
        {
            var so = new SerializedObject(asset);
            var prefabObj = GetPrefabObject(so.FindProperty("prefabGameObject"));
            if (prefabObj == null)
                prefabObj = GetPrefabObjectFromExposedRef(so.FindProperty("sourceGameObject"));
            string prefabRefId = "";
            if (prefabObj != null)
            {
                var path = AssetDatabase.GetAssetPath(prefabObj);
                if (!string.IsNullOrEmpty(path) && path.IndexOf("/Resources/", System.StringComparison.OrdinalIgnoreCase) >= 0)
                    prefabRefId = AddOrGetResource(prefabObj, "Prefab", resourceMap);
            }
            var controlData = new ControlClipData
            {
                sourceBindingKey = GetExposedReferenceName(so.FindProperty("sourceGameObject")),
                trackName = track.name,
                prefabRefId = prefabRefId,
                active = so.FindProperty("active").boolValue,
                updateDirector = so.FindProperty("updateDirector").boolValue,
                updateParticle = so.FindProperty("updateParticle").boolValue,
                updateITimeControl = so.FindProperty("updateITimeControl").boolValue,
                searchHierarchy = so.FindProperty("searchHierarchy").boolValue,
                postPlayback = so.FindProperty("postPlayback").enumValueIndex
            };
            return JsonUtility.ToJson(controlData);
        }

        private static Object GetPrefabObject(SerializedProperty prop)
        {
            return prop?.objectReferenceValue;
        }

        private static Object GetPrefabObjectFromExposedRef(SerializedProperty prop)
        {
            if (prop == null) return null;
            var defaultValue = prop.FindPropertyRelative("defaultValue");
            return defaultValue?.objectReferenceValue;
        }

        private static string GetResourcePathFromObject(Object obj)
        {
            if (obj == null) return "";
            return GetResourcePathFromAssetPath(AssetDatabase.GetAssetPath(obj));
        }

        private static string GetResourcePathFromAssetPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return "";
            const string resourcesToken = "/Resources/";
            var idx = path.IndexOf(resourcesToken, System.StringComparison.OrdinalIgnoreCase);
            if (idx < 0) return "";
            var subPath = path.Substring(idx + resourcesToken.Length);
            return string.IsNullOrEmpty(subPath) ? "" : System.IO.Path.ChangeExtension(subPath, null);
        }

        private static string GetExposedReferenceName(SerializedProperty prop)
        {
            if (prop == null) return "";
            var exposedName = prop.FindPropertyRelative("exposedName");
            return exposedName != null ? exposedName.stringValue : "";
        }
    }
}
