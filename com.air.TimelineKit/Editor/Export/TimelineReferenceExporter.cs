using System;
using System.Collections.Generic;
using System.IO;
using TimelineKit;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TimelineKit.Editor
{
    /// <summary>
    /// Scans a TimelineAsset and generates a TimelineReferenceManifest.
    ///
    /// Only clips that implement ITimelineReferenceCollector participate in collection.
    /// Built-in Unity clip types (ControlPlayableAsset, AnimationPlayableAsset, etc.)
    /// are intentionally skipped — their assets are managed by the Timeline runtime.
    ///
    /// Prefabs already nested inside the owner's Prefab hierarchy are excluded
    /// (they are already in memory and do not need dynamic loading).
    /// </summary>
    public static class TimelineReferenceExporter
    {
        /// <summary>
        /// Build a TimelineReferenceManifest from the given TimelineAsset.
        /// Pass <paramref name="owner"/> (the PlayableDirectorEx GameObject) so that Prefabs
        /// already nested inside its hierarchy are excluded from the manifest.
        /// </summary>
        public static TimelineReferenceManifest Export(TimelineAsset timeline, GameObject owner = null)
        {
            var ownedGuids = owner != null
                ? CollectNestedPrefabGuids(owner)
                : new HashSet<string>();

            var manifest = ScriptableObject.CreateInstance<TimelineReferenceManifest>();
            var assetPath = AssetDatabase.GetAssetPath(timeline);
            manifest.timelinePath = assetPath;
            manifest.timelineGuid = AssetDatabase.AssetPathToGUID(assetPath);

            foreach (var track in timeline.GetOutputTracks())
            {
                if (track == null || track.muted) continue;

                foreach (var clip in track.GetClips())
                {
                    if (clip?.asset is PlayableAsset clipAsset)
                        CollectFromClipAsset(manifest, clipAsset, ownedGuids);
                }
            }

            return manifest;
        }

        // ── Prefab ownership ──────────────────────────────────────────────────────

        private static HashSet<string> CollectNestedPrefabGuids(GameObject owner)
        {
            var guids = new HashSet<string>();
            var prefabAssetRoot = ResolvePrefabAssetRoot(owner);
            if (prefabAssetRoot == null) return guids;

            foreach (var t in prefabAssetRoot.GetComponentsInChildren<Transform>(true))
            {
                var go = t.gameObject;
                if (go == prefabAssetRoot) continue;

                var nearestRoot = PrefabUtility.GetNearestPrefabInstanceRoot(go);
                if (nearestRoot == null || nearestRoot == prefabAssetRoot) continue;

                var src = PrefabUtility.GetCorrespondingObjectFromSource(nearestRoot);
                if (src == null) continue;

                var path = AssetDatabase.GetAssetPath(src);
                var guid = AssetDatabase.AssetPathToGUID(path);
                if (!string.IsNullOrEmpty(guid))
                    guids.Add(guid);
            }

            return guids;
        }

        private static GameObject ResolvePrefabAssetRoot(GameObject owner)
        {
            if (PrefabUtility.IsPartOfPrefabInstance(owner))
            {
                var instanceRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(owner);
                return PrefabUtility.GetCorrespondingObjectFromOriginalSource(instanceRoot);
            }

            if (PrefabUtility.IsPartOfPrefabAsset(owner))
                return owner.transform.root.gameObject;

            return null;
        }

        // ── Clip asset collection ─────────────────────────────────────────────────

        private static void CollectFromClipAsset(TimelineReferenceManifest manifest,
            PlayableAsset asset, HashSet<string> ownedGuids)
        {
            if (asset is not ITimelineReferenceCollector collector) return;

            foreach (var (obj, fieldName) in collector.CollectReferences())
                TryAddAssetReference(manifest, obj, asset.GetType().Name, fieldName, ownedGuids);
        }

        // ── Reference registration ────────────────────────────────────────────────

        private static void TryAddAssetReference(TimelineReferenceManifest manifest,
            UnityEngine.Object asset, string clipType, string fieldName,
            HashSet<string> ownedGuids)
        {
            if (asset == null) return;

            var path = AssetDatabase.GetAssetPath(asset);
            var guid = AssetDatabase.AssetPathToGUID(path);
            if (string.IsNullOrEmpty(guid)) return;

            if (asset is GameObject && ownedGuids.Contains(guid)) return;

            manifest.assetReferences.Add(new TimelineAssetReference
            {
                assetGuid = guid,
                assetPath = path,
                assetType = asset.GetType().FullName,
                resourcePath = ToResourcePath(path),
                addressableKey = string.Empty,
                clipType = clipType,
                fieldName = fieldName
            });
        }

        // ── Utilities ─────────────────────────────────────────────────────────────

        private static string ToResourcePath(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath)) return string.Empty;
            const string marker = "/Resources/";
            int idx = assetPath.IndexOf(marker, StringComparison.Ordinal);
            if (idx < 0) return string.Empty;
            var relative = assetPath[(idx + marker.Length)..];
            var ext = Path.GetExtension(relative);
            return string.IsNullOrEmpty(ext) ? relative : relative[..^ext.Length];
        }
    }
}
