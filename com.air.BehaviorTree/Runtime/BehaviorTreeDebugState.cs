using System.Collections.Generic;
using UnityEngine;

namespace Air.BehaviorTree
{
    /// <summary>
    /// Records per-node execution state for editor debug visualization.
    /// Tracks last status and last executed frame for each node.
    /// </summary>
    public class BehaviorTreeDebugState
    {
        readonly Dictionary<string, BTStatus> nodeStatus = new Dictionary<string, BTStatus>();
        readonly Dictionary<string, float> nodeExecutionStartTime = new Dictionary<string, float>();
        readonly Dictionary<string, float> nodeExecutionEndTime = new Dictionary<string, float>();

        /// <summary>
        /// Minimum duration (seconds) to show white border after execution ends.
        /// </summary>
        public const float FlashMinDuration = 0.3f;

        /// <summary>
        /// Record that a node started executing.
        /// </summary>
        public void RecordExecutionStart(string nodeGUID)
        {
            if (string.IsNullOrEmpty(nodeGUID))
                return;

            if (nodeExecutionStartTime.TryGetValue(nodeGUID, out var prevStart) &&
                nodeExecutionEndTime.TryGetValue(nodeGUID, out var prevEnd))
            {
                var hideAt = Mathf.Max(prevEnd, prevStart + FlashMinDuration);
                if (Time.time < hideAt)
                    return;
            }

            nodeExecutionStartTime[nodeGUID] = Time.time;
        }

        /// <summary>
        /// Record that a node finished executing with the given status.
        /// </summary>
        public void RecordExecutionEnd(string nodeGUID, BTStatus status)
        {
            if (string.IsNullOrEmpty(nodeGUID))
                return;

            nodeStatus[nodeGUID] = status;
            nodeExecutionEndTime[nodeGUID] = Time.time;
        }

        /// <summary>
        /// Get the last recorded status for a node.
        /// </summary>
        public BTStatus GetStatus(string nodeGUID)
        {
            return nodeStatus.TryGetValue(nodeGUID, out var s) ? s : BTStatus.Failure;
        }

        /// <summary>
        /// Returns true if the node should show white border: execution ended recently, stays visible at least FlashMinDuration.
        /// </summary>
        public bool ShouldFlash(string nodeGUID)
        {
            if (string.IsNullOrEmpty(nodeGUID) || !nodeExecutionStartTime.TryGetValue(nodeGUID, out var startTime))
                return false;

            if (!nodeExecutionEndTime.TryGetValue(nodeGUID, out var endTime))
                return true;

            var hideAt = Mathf.Max(endTime, startTime + FlashMinDuration);
            return Time.time < hideAt;
        }
    }
}
