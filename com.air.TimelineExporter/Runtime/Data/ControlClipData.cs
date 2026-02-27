using System;

namespace TimelineExporter
{
    /// <summary>
    /// ControlPlayableAsset data for TimelinePlayer. Stored in TimelineClipData.CustomData as JSON.
    /// </summary>
    [Serializable]
    public class ControlClipData
    {
        /// <summary>
        /// Binding key for resolving GameObject at runtime (from ExposedReference.exposedName).
        /// </summary>
        public string sourceBindingKey;

        /// <summary>
        /// Track name for fallback binding. Use "Control Track" etc. as key when sourceBindingKey is empty.
        /// </summary>
        public string trackName;

        /// <summary>
        /// Id of resource in TimelineData.Resources for prefab instantiation. Empty if not a prefab.
        /// </summary>
        public string prefabRefId;

        /// <summary>
        /// Legacy: direct Resources path. Used when prefabRefId is empty (backward compat).
        /// </summary>
        public string prefabResourcePath;

        public bool active = true;
        public bool updateDirector = true;
        public bool updateParticle = true;
        public bool updateITimeControl = true;
        public bool searchHierarchy;

        /// <summary>
        /// 0=Revert, 1=LeaveAsIs, 2=LeaveActive (ActivationControlPlayable.PostPlaybackState).
        /// </summary>
        public int postPlayback;
    }
}
