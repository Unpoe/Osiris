using UnityEngine;
using UnityEngine.UI;

namespace Osiris
{
    public class Lifebar : MonoBehaviour, IPoolable
    {
        [SerializeField] private Image fillImage = default;

        private Actor actor = null;
        private Camera mainCamera = null;

        public void Initialize(Actor actor, Camera mainCamera) {
            this.actor = actor;
            this.mainCamera = mainCamera;
        }

        public void GameUpdate() {
            fillImage.fillAmount = actor.GetLifePercentage();

            Vector3 lifebarWorldPos = actor.GetLifebarWorldPosition();
            Vector3 screenPoint = mainCamera.WorldToViewportPoint(lifebarWorldPos);
            Vector2 screenPos = new Vector2(screenPoint.x, screenPoint.y);

            RectTransform cachedTransform = transform as RectTransform;
            cachedTransform.anchorMin = screenPos;
            cachedTransform.anchorMax = screenPos;
            cachedTransform.anchoredPosition = Vector2.zero;
        }

        public bool HasActorAssigned(Actor actor) {
            return this.actor.battleId.Equals(actor.battleId);
        }

        public void OnGetFromPool() {
            gameObject.SetActive(true);
        }

        public void OnReturnToPool() {
            gameObject.SetActive(false);

            actor = null;
            mainCamera = null;
        }
    }
}