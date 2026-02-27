using System;
using UnityEngine;

namespace TimelineExporter
{
    /// <summary>
    /// AnimationPlayableAsset data for TimelinePlayer. Stored in TimelineClipData.CustomData as JSON.
    /// </summary>
    [Serializable]
    public class AnimationClipData
    {
        /// <summary>
        /// Track name for binding lookup (Animator or GameObject).
        /// </summary>
        public string trackName;

        /// <summary>
        /// Resource ID for standalone AnimationClip in Resources. Empty when clip is sub-asset of Timeline.
        /// </summary>
        public string animationClipRefId;

        /// <summary>
        /// Track index for sub-asset extraction from loaded Timeline. -1 when using animationClipRefId.
        /// </summary>
        public int trackBindingIndex = -1;

        /// <summary>
        /// Clip index within track for sub-asset extraction. -1 when using animationClipRefId.
        /// </summary>
        public int clipIndex = -1;

        /// <summary>
        /// Position offset (Vector3).
        /// </summary>
        public Vector3 position;

        /// <summary>
        /// Euler angles offset (Vector3).
        /// </summary>
        public Vector3 eulerAngles;

        /// <summary>
        /// Remove start offset for root motion alignment.
        /// </summary>
        public bool removeStartOffset = true;

        /// <summary>
        /// Apply foot IK for humanoid animations.
        /// </summary>
        public bool applyFootIK = true;

        /// <summary>
        /// Loop mode: 0=UseSourceAsset, 1=On, 2=Off.
        /// </summary>
        public int loop;
    }
}
