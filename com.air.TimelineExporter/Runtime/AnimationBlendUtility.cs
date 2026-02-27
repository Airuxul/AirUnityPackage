using System.Collections.Generic;
using UnityEngine;

namespace TimelineExporter
{
    /// <summary>
    /// Blends multiple AnimationClip samples without Playables. Samples each clip to temp, blends transforms, applies to target.
    /// </summary>
    public static class AnimationBlendUtility
    {
        public static void SampleAndBlend(GameObject target, IReadOnlyList<(AnimationClip clip, float weight, float sampleTime, AnimationClipData animData)> clips)
        {
            if (target == null || clips == null || clips.Count == 0) return;

            var totalWeight = 0f;
            foreach (var c in clips) totalWeight += c.weight;
            if (totalWeight <= 0) return;

            var animator = target.GetComponent<Animator>();
            if (animator != null) animator.enabled = false;

            if (clips.Count == 1 && clips[0].weight >= 0.999f && clips[0].clip != null && !clips[0].clip.legacy)
            {
                clips[0].clip.SampleAnimation(target, clips[0].sampleTime);
            }
            else
            {
                BlendMultiple(target, clips, totalWeight);
            }

            // Do not re-enable animator here. Caller (mixer/behaviour) keeps it disabled during control and re-enables on exit.
        }

        private static void BlendMultiple(GameObject target,
            IReadOnlyList<(AnimationClip clip, float weight, float sampleTime, AnimationClipData animData)> clips,
            float totalWeight)
        {
            var temps = new List<(GameObject go, float w)>();
            foreach (var c in clips)
            {
                if (c.clip == null || c.clip.legacy || c.weight <= 0) continue;

                var temp = Object.Instantiate(target);
                temp.SetActive(false);
                var anim = temp.GetComponent<Animator>();
                if (anim != null) anim.enabled = false;

                var len = c.clip.length;
                if (len > 0)
                {
                    var loop = c.animData != null && (c.animData.loop == 1 || (c.animData.loop == 0 && c.clip.isLooping));
                    var t = loop ? Mathf.Repeat(c.sampleTime, len) : Mathf.Clamp(c.sampleTime, 0, len);
                    c.clip.SampleAnimation(temp, t);
                }
                temps.Add((temp, c.weight));
            }

            if (temps.Count == 0) return;

            try
            {
                var dstTransforms = target.GetComponentsInChildren<Transform>(true);
                var srcArrays = new Transform[temps.Count][];
                for (int j = 0; j < temps.Count; j++)
                    srcArrays[j] = temps[j].go.GetComponentsInChildren<Transform>(true);

                for (int i = 0; i < dstTransforms.Length; i++)
                {
                    var dst = dstTransforms[i];
                    var pos = Vector3.zero;
                    var scale = Vector3.zero;
                    var rotAccum = Quaternion.identity;
                    var wSum = 0f;

                    for (int j = 0; j < temps.Count; j++)
                    {
                        var w = temps[j].w / totalWeight;
                        if (i >= srcArrays[j].Length) continue;

                        var src = srcArrays[j][i];
                        pos += src.position * w;
                        scale += src.localScale * w;
                        rotAccum = Quaternion.Slerp(rotAccum, src.rotation, w / (wSum + w));
                        wSum += w;
                    }

                    if (wSum > 0)
                    {
                        dst.position = pos / wSum;
                        dst.rotation = rotAccum;
                        dst.localScale = scale / wSum;
                    }
                }
            }
            finally
            {
                foreach (var (go, _) in temps) Object.Destroy(go);
            }
        }
    }
}
