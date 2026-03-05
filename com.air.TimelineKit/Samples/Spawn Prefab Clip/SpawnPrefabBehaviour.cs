using TimelineKit;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// Behaviour that spawns a prefab when the clip begins and destroys it when it ends.
///
/// The prefab asset is retrieved from PlayableDirectorEx's preloaded cache (if available),
/// falling back to a direct reference on the clip. This pattern mirrors Unity's
/// PrefabControlPlayable but integrates with the PlayableDirectorEx preload pipeline.
///
/// Lifecycle:
///   OnBehaviourPlay  → Instantiate, SetActive(true)
///   OnBehaviourPause → SetActive(false) when clip exits (effectivePlayState == Paused)
///   OnPlayableDestroy → Destroy instance
/// </summary>
public class SpawnPrefabBehaviour : PlayableBehaviourEx
{
    /// <summary>
    /// Direct prefab reference used when PlayableDirectorEx is not present or the
    /// asset was not preloaded (e.g. plain Resources workflow without manifest).
    /// </summary>
    public GameObject Prefab { get; set; }

    /// <summary>
    /// Resources.Load key or Addressable key from the manifest.
    /// When set, the behaviour asks PlayableDirectorEx for the cached asset first.
    /// </summary>
    public string AssetKey { get; set; }

    /// <summary>
    /// Transform to parent the spawned instance to.
    /// Defaults to the PlayableDirectorEx's transform if not set.
    /// </summary>
    public Transform SpawnParent { get; set; }

    private GameObject _instance;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        base.OnBehaviourPlay(playable, info);

        if (_instance != null)
        {
            _instance.SetActive(true);
            return;
        }

        var prefab = ResolvePrefab();
        if (prefab == null) return;

        _instance = Object.Instantiate(prefab, SpawnParent, false);
        _instance.SetActive(true);
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        base.OnBehaviourPause(playable, info);

        // Only deactivate when the clip actually exits; ignore graph-level pauses.
        if (_instance != null && info.effectivePlayState == PlayState.Paused)
            _instance.SetActive(false);
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        if (_instance == null) return;

        if (Application.isPlaying)
            Object.Destroy(_instance);
        else
            Object.DestroyImmediate(_instance);

        _instance = null;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private GameObject ResolvePrefab()
    {
        if (!string.IsNullOrEmpty(AssetKey))
        {
            var cached = GetDirectorEx()?.GetLoadedAsset<GameObject>(AssetKey);
            if (cached != null) return cached;
        }

        return Prefab;
    }
}
