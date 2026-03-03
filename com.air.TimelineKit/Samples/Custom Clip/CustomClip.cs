using System;
using System.Collections.Generic;
using TimelineKit;
using UnityEngine.Timeline;

/// <summary>
/// Example clip asset. Add serialized data fields here and push them onto the behaviour
/// in SetupBehaviour. Implement ITimelineReferenceCollector to declare which Unity Object
/// fields should appear in a TimelineReferenceManifest for dynamic loading setup.
/// </summary>
[Serializable]
public class CustomClip : ClipEx<CustomPlayableBehaviour>, ITimelineClipAsset, ITimelineReferenceCollector
{
    public ClipCaps clipCaps => ClipCaps.Blending;

    public string message;

    protected override void SetupBehaviour(CustomPlayableBehaviour behaviour, UnityEngine.GameObject owner)
    {
        behaviour.Message = message;
    }

    public IEnumerable<(UnityEngine.Object asset, string fieldName)> CollectReferences()
    {
        // Yield any Unity Object fields that need to be tracked for dynamic loading.
        // Example: if (myPrefab) yield return (myPrefab, nameof(myPrefab));
        yield break;
    }
}
