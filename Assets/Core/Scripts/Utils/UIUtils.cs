using System;
using UnityEngine;

namespace HyperCasual.Core
{
    /// <summary>
    ///     Contains UI helper methods
    /// </summary>
    public static class UIUtils
    {
        /// <summary>
        ///     Resizes a RectTransform to match the screen safe area.
        ///     The resize algorithm only works for full screen canvas/parents.
        /// </summary>
        public static void ResizeToSafeArea(this RectTransform rectTransform, Canvas canvas)
        {
            if (rectTransform == null)
                throw new ArgumentNullException(nameof(rectTransform));

            var safeArea = Screen.safeArea;
            var canvasRect = canvas.pixelRect;

            var anchorMin = safeArea.position;
            var anchorMax = safeArea.position + safeArea.size;
            anchorMin.x /= canvasRect.width;
            anchorMin.y /= canvasRect.height;
            anchorMax.x /= canvasRect.width;
            anchorMax.y /= canvasRect.height;

#if UNITY_EDITOR
            rectTransform.anchorMin = new Vector2(0f, 0.05f);
            rectTransform.anchorMax = new Vector2(1f, 0.95f);
#else
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
#endif
        }
    }
}