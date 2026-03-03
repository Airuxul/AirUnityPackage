using System.Collections.Generic;
using UnityEngine;

namespace TimelineKit
{
    /// <summary>
    /// Implement on custom clip assets to explicitly declare which Unity Object references
    /// should be included in a TimelineReferenceManifest for dynamic loading setup.
    ///
    /// The editor exporter (TimelineReferenceExporter) uses this when generating a manifest
    /// from the PlayableDirectorEx Inspector. Only clips implementing this interface are
    /// collected; built-in Unity clip types are skipped.
    /// </summary>
    public interface ITimelineReferenceCollector
    {
        /// <summary>
        /// Yield each (asset, fieldName) pair that should be tracked.
        /// Skip null assets; scene objects are automatically excluded by the exporter.
        /// </summary>
        IEnumerable<(Object asset, string fieldName)> CollectReferences();
    }
}
