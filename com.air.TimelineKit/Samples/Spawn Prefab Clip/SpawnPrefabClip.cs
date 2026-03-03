using System;
using System.Collections.Generic;
using TimelineKit;
using UnityEngine;
using UnityEngine.Timeline;

/// <summary>
/// Clip asset that spawns a prefab for its duration.
///
/// Implements ITimelineReferenceCollector so the prefab is included in the
/// TimelineReferenceManifest when exported from PlayableDirectorEx's Inspector.
/// PlayableDirectorEx will then preload the prefab via its ITimelineAssetLoader
/// before the Timeline plays.
///
/// Usage:
///   1. Add a SpawnPrefabTrack to the Timeline.
///   2. Drop a SpawnPrefabClip onto the track and assign a prefab.
///   3. On the PlayableDirectorEx component click "Export Reference Manifest".
///   4. Call PlayableDirectorEx.PlayWithPreload() — the prefab is preloaded
///      before playback begins.
/// </summary>
[Serializable]
public class SpawnPrefabClip : ClipEx<SpawnPrefabBehaviour>, ITimelineClipAsset, ITimelineReferenceCollector
{
    [Tooltip("Prefab to instantiate while this clip is active.")]
    public GameObject prefab;

    public ClipCaps clipCaps => ClipCaps.None;

    protected override void SetupBehaviour(SpawnPrefabBehaviour behaviour, GameObject owner)
    {
        behaviour.Prefab = prefab;

        // Provide the preload key so the behaviour can look up the cached asset at runtime.
        if (prefab != null)
            behaviour.AssetKey = prefab.name; // matches the resourcePath key (filename without extension)

        // Default spawn parent: the PlayableDirectorEx transform.
        if (owner != null)
            behaviour.SpawnParent = owner.transform;
    }

    /// <summary>
    /// Declares the prefab field so it is included in the TimelineReferenceManifest.
    /// </summary>
    public IEnumerable<(UnityEngine.Object asset, string fieldName)> CollectReferences()
    {
        if (prefab != null)
            yield return (prefab, nameof(prefab));
    }
}
