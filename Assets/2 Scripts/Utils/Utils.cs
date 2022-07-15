using UnityEngine;

namespace Osiris
{
    public static class Utils
    {
        public static float Remap(float value, float from1, float to1, float from2, float to2) {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        public static void PositionRectTransformInWorldPos(RectTransform transform, Vector3 worldPos, Camera camera) {
            Vector3 screenPoint = camera.WorldToViewportPoint(worldPos);
            Vector2 screenPos = new Vector2(screenPoint.x, screenPoint.y);

            transform.anchorMin = screenPos;
            transform.anchorMax = screenPos;
            transform.anchoredPosition = Vector2.zero;
        }
    }
}