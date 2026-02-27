using System;
using UnityEngine;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

namespace TimelineExporter
{
    /// <summary>
    /// Simulates ControlTrack for TimelinePlayer. Handles activation, particles, directors, ITimeControl.
    /// </summary>
    public class ControlBehaviour : ISimulatedPlayableBehaviour
    {
        public void OnGraphStart(IPlayableContext context, IClipContext clipContext) { }
        public void OnGraphStop(IPlayableContext context, IClipContext clipContext) { }
        public void PrepareFrame(IPlayableContext context, IClipContext clipContext) { }

        public void OnBehaviourPlay(IPlayableContext context, IClipContext clipCtx)
        {
            if (!TryGetContext(clipCtx, out var control, out var go)) return;

            if (control.active) go.SetActive(true);
            SetParticles(go, control, play: true);
            WithSubPlayer(control, clipCtx, p => p.Play());
            WithITimeControl(go, control, tc => { tc.OnControlTimeStart(); tc.SetTime(0); });
        }

        public void OnBehaviourPause(IPlayableContext context, IClipContext clipCtx)
        {
            if (!TryGetContext(clipCtx, out var control, out var go)) return;

            if (control.active && control.postPlayback == 0)
            {
                if (HasPrefabRef(control)) Object.Destroy(go);
                else go.SetActive(false);
            }
            WithSubPlayer(control, clipCtx, p => p.Stop());
            WithITimeControl(go, control, tc => tc.OnControlTimeStop());
            SetParticles(go, control, play: false);
        }

        public void ProcessFrame(IPlayableContext context, IClipContext clipCtx)
        {
            if (!TryGetContext(clipCtx, out var control, out var go)) return;

            WithSubPlayer(control, clipCtx, p => p.Update(context.DeltaTime));
            WithITimeControl(go, control, tc => tc.SetTime((float)context.LocalTime));
        }

        private static bool TryGetContext(IClipContext clipCtx, out ControlClipData control, out GameObject go)
        {
            control = null;
            go = null;
            if (clipCtx == null) return false;
            control = ClipContextResolver.ParseCustomData<ControlClipData>(clipCtx.Clip);
            if (control == null) return false;
            go = ResolveGameObject(control, clipCtx);
            return go != null;
        }

        private static GameObject ResolveGameObject(ControlClipData control, IClipContext clipCtx)
        {
            var go = ClipContextResolver.ResolveGameObject(clipCtx, control.sourceBindingKey, control.trackName, clipCtx.Clip?.DisplayName);
            if (go != null) return go;
            return HasPrefabRef(control)
                ? ClipContextResolver.TryInstantiatePrefab(clipCtx, control.prefabRefId, control.prefabResourcePath, control.sourceBindingKey, control.trackName, clipCtx.Clip?.DisplayName ?? "prefab")
                : null;
        }

        private static bool HasPrefabRef(ControlClipData control) =>
            !string.IsNullOrEmpty(control.prefabRefId) || !string.IsNullOrEmpty(control.prefabResourcePath);

        private static void SetParticles(GameObject go, ControlClipData control, bool play)
        {
            if (!control.updateParticle) return;
            var particles = control.searchHierarchy ? go.GetComponentsInChildren<ParticleSystem>(true) : go.GetComponents<ParticleSystem>();
            foreach (var ps in particles)
                if (play) ps.Play(true); else ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        private static void WithSubPlayer(ControlClipData control, IClipContext clipCtx, Action<TimelinePlayer> action)
        {
            if (!control.updateDirector || clipCtx.SubTimelinePlayers == null) return;
            if (ClipContextResolver.TryGetSubPlayer(clipCtx.SubTimelinePlayers, out var sub, control.sourceBindingKey, control.trackName))
                action(sub);
        }

        private static void WithITimeControl(GameObject go, ControlClipData control, Action<ITimeControl> action)
        {
            if (!control.updateITimeControl) return;
            foreach (var mb in go.GetComponentsInChildren<MonoBehaviour>())
            {
                // ReSharper disable once SuspiciousTypeConversion.Global - ITimeControl implemented by user scripts
                if (mb is ITimeControl tc) action(tc);
            }
        }
    }
}
