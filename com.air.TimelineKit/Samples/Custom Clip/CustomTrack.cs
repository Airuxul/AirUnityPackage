using TimelineKit;
using UnityEngine.Timeline;

/// <summary>
/// Example track. Accepts CustomClip and uses CustomMixPlayableBehaviour for blending.
/// TrackEx handles mixer creation — no code body needed.
/// </summary>
[TrackClipType(typeof(CustomClip))]
public class CustomTrack : TrackEx<CustomClip, CustomMixPlayableBehaviour> { }
