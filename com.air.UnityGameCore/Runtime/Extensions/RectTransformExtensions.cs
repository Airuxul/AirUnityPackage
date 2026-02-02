using UnityEngine;

namespace Air.UnityGameCore.Runtime.Extensions
{
    public static class RectTransformExtensions
    {
        /// <summary>
        /// 填充父节点
        /// </summary>
        /// <param name="rectTransform">RectTransform to fill parent</param>
        public static void FillParent(this RectTransform rectTransform) {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
        }
    }
}