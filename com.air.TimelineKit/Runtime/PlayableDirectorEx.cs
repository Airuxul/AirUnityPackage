using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace TimelineKit
{
    /// <summary>
    /// MonoBehaviour wrapper for PlayableDirector with reference preloading support.
    ///
    /// Workflow:
    ///   1. Assign a TimelineReferenceManifest via the Inspector "Export Reference Manifest" button.
    ///   2. Call PlayWithPreload() — assets are loaded into the loader cache, then the Timeline plays.
    ///      Clip behaviours (e.g. SpawnPrefabBehaviour) use GetLoadedAsset() to use preloaded assets.
    ///   3. Call Unload() to release the loader cache when done.
    ///
    /// Inject a custom ITimelineAssetLoader (e.g. Addressables) via the Loader property
    /// before calling any play method. Defaults to ResourcesAssetLoader.
    /// </summary>
    [RequireComponent(typeof(PlayableDirector))]
    public class PlayableDirectorEx : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("References manifest generated from the Timeline asset. Required for preloading.")]
        private TimelineReferenceManifest _manifest;

        [SerializeField]
        [Tooltip("If true, preload assets and play on Awake.")]
        private bool _playOnAwake;

        // ── Public API ────────────────────────────────────────────────────────────

        /// <summary>The underlying PlayableDirector.</summary>
        public PlayableDirector Director => _director;

        /// <summary>
        /// Manifest listing all asset references in the Timeline.
        /// Assigning a new manifest resets the preloaded state.
        /// </summary>
        public TimelineReferenceManifest Manifest
        {
            get => _manifest;
            set { _manifest = value; IsPreloaded = false; }
        }

        /// <summary>
        /// Asset loader backend. Defaults to ResourcesAssetLoader.
        /// Assign an Addressables or AssetBundle loader before preloading.
        /// </summary>
        public ITimelineAssetLoader Loader
        {
            get => _loader ??= new ResourcesAssetLoader();
            set => _loader = value;
        }

        /// <summary>True after all manifest assets have been loaded.</summary>
        public bool IsPreloaded { get; private set; }

        /// <summary>True while the PlayableDirector is playing.</summary>
        public bool IsPlaying => _director != null && _director.state == PlayState.Playing;

        /// <summary>Invoked when preloading finishes, before playback starts.</summary>
        public event Action OnPreloadComplete;

        /// <summary>Invoked immediately after playback begins.</summary>
        public event Action OnPlaybackStarted;

        /// <summary>Invoked when Stop() is called or the Timeline reaches its end.</summary>
        public event Action OnPlaybackStopped;

        // ── Playback ──────────────────────────────────────────────────────────────

        /// <summary>Play immediately without preloading.</summary>
        public void Play()
        {
            _director.Play();
            OnPlaybackStarted?.Invoke();
        }

        /// <summary>Preload all manifest assets then start playback (non-blocking).</summary>
        public void PlayWithPreload() => StartCoroutine(PlayWithPreloadRoutine());

        /// <summary>Same as PlayWithPreload() but returns the Coroutine for yielding.</summary>
        public Coroutine PlayWithPreloadCoroutine() => StartCoroutine(PlayWithPreloadRoutine());

        public void Pause() => _director.Pause();

        public void Resume()
        {
            _director.Play();
            OnPlaybackStarted?.Invoke();
        }

        public void Stop()
        {
            _director.Stop();
            OnPlaybackStopped?.Invoke();
        }

        // ── Preloading ────────────────────────────────────────────────────────────

        /// <summary>Preload all manifest assets without starting playback.</summary>
        public Coroutine Preload() => StartCoroutine(PreloadRoutine());

        /// <summary>
        /// Returns a preloaded asset by loader key (resourcePath or addressableKey).
        /// Clip behaviours call this to avoid reloading at runtime.
        /// </summary>
        public T GetLoadedAsset<T>(string key) where T : UnityEngine.Object
            => _loadedAssets.TryGetValue(key, out var asset) ? asset as T : null;

        /// <summary>Release all preloaded assets and reset state.</summary>
        public void Unload()
        {
            foreach (var key in _loadedAssets.Keys)
                Loader.Unload(key);
            _loadedAssets.Clear();

            IsPreloaded = false;
        }

        // ── Unity lifecycle ───────────────────────────────────────────────────────

        private void Awake()
        {
            _director = GetComponent<PlayableDirector>();
            _director.stopped += _ => OnPlaybackStopped?.Invoke();

            if (_playOnAwake)
                StartCoroutine(PlayWithPreloadRoutine());
        }

        private void OnDestroy() => Unload();

        // ── Coroutines ────────────────────────────────────────────────────────────

        private IEnumerator PlayWithPreloadRoutine()
        {
            if (!IsPreloaded)
                yield return PreloadRoutine();

            Play();
        }

        private IEnumerator PreloadRoutine()
        {
            IsPreloaded = false;

            if (_manifest != null)
            {
                // Preload clip asset references into memory (no instantiation).
                // These assets (AnimationClips, AudioClips, prefab assets, etc.) are
                // cached so the loader back-end does not reload them during playback.
                foreach (var reference in _manifest.assetReferences)
                {
                    var key = ResolveKey(reference.resourcePath, reference.addressableKey);
                    if (string.IsNullOrEmpty(key) || _loadedAssets.ContainsKey(key))
                        continue;

                    var asset = Loader.Load<UnityEngine.Object>(key);
                    if (asset != null)
                        _loadedAssets[key] = asset;

                    yield return null;
                }
            }

            IsPreloaded = true;
            OnPreloadComplete?.Invoke();
        }

        private static string ResolveKey(string resourcePath, string addressableKey)
        {
            if (!string.IsNullOrEmpty(resourcePath)) return resourcePath;
            if (!string.IsNullOrEmpty(addressableKey)) return addressableKey;
            return null;
        }

        // ── Private fields ────────────────────────────────────────────────────────

        private PlayableDirector _director;
        private ITimelineAssetLoader _loader;
        private readonly Dictionary<string, UnityEngine.Object> _loadedAssets = new();
    }
}
