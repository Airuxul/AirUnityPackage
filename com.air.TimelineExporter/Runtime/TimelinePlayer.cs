using System;
using System.Collections.Generic;
using UnityEngine;

namespace TimelineExporter
{
    /// <summary>
    /// Simulates Timeline playback from exported TimelineData.
    /// Drives clip lifecycle (OnClipEnter/Exit) and ProcessFrame via IPlayableContext.
    /// </summary>
    public class TimelinePlayer
    {
        private readonly TimelineData data;
        private readonly SimulatedPlayableContext context = new();
        private readonly Dictionary<string, ISimulatedPlayableBehaviour> behaviourMap = new();
        private readonly Dictionary<string, (TimelineClipData clip, TimelineTrackData track)> activeClips = new();
        private readonly Dictionary<string, ITrackMixer> trackMixers = new();
        private readonly List<ITrackBlendHandler> trackBlendHandlers = new();
        private readonly HashSet<Animator> animatorsToRestoreOnStop = new();
        private double currentTime;
        private bool isPlaying;
        private UnityEngine.Timeline.TimelineAsset cachedTimelineAsset;

        /// <summary>
        /// Binding key -> GameObject for ControlTrack etc. Key = ControlClipData.sourceBindingKey.
        /// </summary>
        public Dictionary<string, GameObject> Bindings { get; } = new();

        /// <summary>
        /// Binding key -> TimelinePlayer for sub-timelines (ControlTrack updateDirector).
        /// </summary>
        public Dictionary<string, TimelinePlayer> SubTimelinePlayers { get; } = new();

        /// <summary>
        /// Optional root to search children by track name when binding key not found.
        /// </summary>
        public Transform BindingRoot { get; set; }

        public TimelineData Data => data;
        public double CurrentTime => currentTime;
        public double Duration => data?.Duration ?? 0;
        public bool IsPlaying => isPlaying;

        public event Action<double> OnTimeUpdated;
        public event Action OnPlaybackFinished;

        public TimelinePlayer(TimelineData timelineData)
        {
            data = timelineData ?? throw new ArgumentNullException(nameof(timelineData));
        }

        /// <summary>
        /// Register behaviour for clip type. Key = TimelineClipData.ClipType.
        /// </summary>
        public void RegisterBehaviour(string clipType, ISimulatedPlayableBehaviour behaviour)
        {
            if (!string.IsNullOrEmpty(clipType) && behaviour != null)
                behaviourMap[clipType] = behaviour;
        }

        /// <summary>
        /// Register track blend handler (e.g. AnimationTrackBlendHandler). Handlers process tracks that need mixer blend.
        /// </summary>
        public void RegisterTrackBlendHandler(ITrackBlendHandler handler)
        {
            if (handler != null && !trackBlendHandlers.Contains(handler))
                trackBlendHandlers.Add(handler);
        }

        internal IPlayableContext GetContext() => context;

        internal UnityEngine.Timeline.TimelineAsset GetCachedTimelineAsset()
        {
            if (cachedTimelineAsset == null && data != null)
                cachedTimelineAsset = TimelineAssetLoader.Load(data);
            return cachedTimelineAsset;
        }

        internal bool IsClipActive(string clipId) => activeClips.ContainsKey(clipId);

        internal ITrackMixer GetOrCreateTrackMixer(string key, Func<ITrackMixer> factory)
        {
            if (trackMixers.TryGetValue(key, out var mixer))
                return mixer;
            mixer = factory();
            trackMixers[key] = mixer;
            return mixer;
        }

        internal bool HasTrackMixer(string key) => !string.IsNullOrEmpty(key) && trackMixers.ContainsKey(key);

        internal void DestroyTrackMixer(string key)
        {
            if (string.IsNullOrEmpty(key)) return;
            if (trackMixers.TryGetValue(key, out var mixer))
            {
                mixer.Destroy();
                trackMixers.Remove(key);
            }
        }

        /// <summary>
        /// Registers an animator to be re-enabled when Stop() is called. Used when track has no active clips.
        /// </summary>
        internal void RegisterAnimatorToRestoreOnStop(Animator animator)
        {
            if (animator != null) animatorsToRestoreOnStop.Add(animator);
        }

        internal void TryEnterClip(TimelineClipData clip, TimelineTrackData track) => TryEnterClipInternal(clip, track);

        internal void TryProcessFrame(TimelineClipData clip, TimelineTrackData track, float weight) => TryProcessFrameInternal(clip, track, weight);

        public void Play()
        {
            currentTime = 0;
            isPlaying = true;
            context.LocalTime = 0;
            context.Duration = data.Duration;
            context.DeltaTime = 0;
            context.EffectiveWeight = 1f;
            foreach (var b in behaviourMap.Values)
                b.OnGraphStart(context, null);
        }

        public void Play(double startTime)
        {
            currentTime = Math.Max(0, startTime);
            isPlaying = true;
            context.LocalTime = startTime;
            context.Duration = data.Duration;
            context.DeltaTime = 0;
            context.EffectiveWeight = 1f;
            foreach (var b in behaviourMap.Values)
                b.OnGraphStart(context, null);
        }

        public void Pause() => isPlaying = false;

        public void Stop()
        {
            isPlaying = false;
            context.LocalTime = currentTime;
            context.Duration = data.Duration;
            context.EffectiveWeight = 0f;
            foreach (var m in trackMixers.Values) m.Destroy();
            trackMixers.Clear();
            // Do not re-enable animators. Keep last pose to avoid default state playing.
            animatorsToRestoreOnStop.Clear();
            foreach (var kv in activeClips)
                TryExitClip(kv.Value.clip);
            activeClips.Clear();
            foreach (var b in behaviourMap.Values)
                b.OnGraphStop(context, null);
            currentTime = 0;
        }

        public void SetTime(double time)
        {
            currentTime = Mathf.Clamp((float)time, 0, (float)Duration);
        }

        public void Update(float deltaTime)
        {
            if (!isPlaying || data == null) return;

            context.DeltaTime = deltaTime;
            currentTime += deltaTime;
            OnTimeUpdated?.Invoke(currentTime);

            var toExit = CollectClipsToExit();
            foreach (var clip in toExit) TryExitClip(clip);

            if (currentTime >= data.Duration) FinishPlayback();
        }

        private List<TimelineClipData> CollectClipsToExit()
        {
            var toExit = new List<TimelineClipData>();
            foreach (var track in data.Tracks)
            {
                if (track.Muted) continue;

                var handled = false;
                foreach (var handler in trackBlendHandlers)
                {
                    if (handler.CanHandle(track))
                    {
                        handler.Process(this, track, toExit);
                        handled = true;
                        break;
                    }
                }
                if (handled) continue;

                foreach (var clip in track.Clips)
                {
                    var inRange = currentTime >= clip.StartTime && currentTime < clip.EndTime;
                    var wasActive = activeClips.ContainsKey(clip.Id);

                    if (inRange && !wasActive) TryEnterClipInternal(clip, track);
                    else if (!inRange && wasActive) toExit.Add(clip);

                    if (inRange) TryProcessFrame(clip, track);
                }
            }
            return toExit;
        }

        private void FinishPlayback()
        {
            isPlaying = false;
            currentTime = data.Duration;
            foreach (var m in trackMixers.Values) m.Destroy();
            trackMixers.Clear();
            // Do not re-enable animators. Keep last pose to avoid default state (e.g. MoveY) playing.
            animatorsToRestoreOnStop.Clear();
            foreach (var kv in activeClips) TryExitClip(kv.Value.clip);
            activeClips.Clear();
            context.LocalTime = currentTime;
            context.Duration = data.Duration;
            foreach (var b in behaviourMap.Values) b.OnGraphStop(context, null);
            OnPlaybackFinished?.Invoke();
        }

        private void TryEnterClipInternal(TimelineClipData clip, TimelineTrackData track)
        {
            activeClips[clip.Id] = (clip, track);
            if (!behaviourMap.TryGetValue(clip.ClipType ?? "", out var behaviour)) return;

            UpdateContextForClip(clip, 1f);
            var clipContext = CreateClipContext(clip, track);
            behaviour.OnBehaviourPlay(clipContext.Context, clipContext);
        }

        private void TryExitClip(TimelineClipData clip)
        {
            if (!behaviourMap.TryGetValue(clip.ClipType ?? "", out var behaviour))
            {
                activeClips.Remove(clip.Id);
                return;
            }

            var (_, track) = activeClips[clip.Id];
            activeClips.Remove(clip.Id);

            context.LocalTime = clip.Duration;
            context.Duration = clip.Duration;
            context.EffectiveWeight = 0f;
            var clipContext = CreateClipContext(clip, track);
            behaviour.OnBehaviourPause(clipContext.Context, clipContext);
        }

        private void TryProcessFrame(TimelineClipData clip, TimelineTrackData track)
        {
            TryProcessFrameInternal(clip, track, 1f);
        }

        private void TryProcessFrameInternal(TimelineClipData clip, TimelineTrackData track, float weight)
        {
            if (!behaviourMap.TryGetValue(clip.ClipType ?? "", out var behaviour)) return;

            UpdateContextForClip(clip, weight);
            var clipContext = CreateClipContext(clip, track);
            behaviour.PrepareFrame(clipContext.Context, clipContext);
            behaviour.ProcessFrame(clipContext.Context, clipContext);
        }

        private void UpdateContextForClip(TimelineClipData clip, float weight = 1f)
        {
            context.LocalTime = currentTime - clip.StartTime;
            context.Duration = clip.Duration;
            context.EffectiveWeight = weight;
        }

        private ClipPlayableContext CreateClipContext(TimelineClipData clip, TimelineTrackData track)
        {
            if (cachedTimelineAsset == null && data != null)
                cachedTimelineAsset = TimelineAssetLoader.Load(data);

            return new ClipPlayableContext
            {
                Context = context,
                Clip = clip,
                Track = track,
                TimelineData = data,
                Bindings = Bindings,
                SubTimelinePlayers = SubTimelinePlayers,
                BindingRoot = BindingRoot,
                CachedTimelineAsset = cachedTimelineAsset
            };
        }
    }
}
