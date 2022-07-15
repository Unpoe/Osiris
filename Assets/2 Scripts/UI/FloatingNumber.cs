using UnityEngine;
using TMPro;

namespace Osiris
{
    public class FloatingNumber : MonoBehaviour, IPoolable
{
        [SerializeField] private TextMeshProUGUI label = default;
        [SerializeField] private CanvasGroup canvasGroup = default;
        [Space]
        [SerializeField] private float lifetime = 1f;
        [SerializeField] private float upDistance = 30f;
        [SerializeField] private AnimationCurve animationCurve = default;

        private RectTransform cachedTransform = null;

        private float progress = 0f;
        private Vector2 initialPos, targetPos = Vector2.zero;

        public void Initialize(float number, Vector3 worldPos, Camera mainCamera) {
            label.text = number.ToString();
            progress = 0f;

            cachedTransform = transform as RectTransform;
            Utils.PositionRectTransformInWorldPos(cachedTransform, worldPos, mainCamera);

            initialPos = targetPos = cachedTransform.anchoredPosition;
            targetPos.y += upDistance;

            canvasGroup.alpha = 1f;
        }

        public bool GameUpdate(float dt) {
            progress += dt;
            if(progress > lifetime) {
                return false;
            }

            float e = animationCurve.Evaluate(progress / lifetime);
            cachedTransform.anchoredPosition = Vector2.Lerp(initialPos, targetPos, e);
            canvasGroup.alpha = e;

            return true;
        }

        public void OnGetFromPool() {
            gameObject.SetActive(true);
        }

        public void OnReturnToPool() {
            gameObject.SetActive(false);
        }
    }
}