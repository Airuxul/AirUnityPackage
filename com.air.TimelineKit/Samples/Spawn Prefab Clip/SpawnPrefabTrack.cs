using TimelineKit;
using UnityEngine;
using UnityEngine.Timeline;

/// <summary>
/// Track that hosts SpawnPrefabClips. Does not require a scene binding —
/// each clip manages its own prefab instance independently.
/// </summary>
[TrackClipType(typeof(SpawnPrefabClip))]
[TrackColor(0.6f, 0.3f, 0.8f)]
public class SpawnPrefabTrack : TrackEx<SpawnPrefabClip, SpawnPrefabMixerBehaviour> { }
