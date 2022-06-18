using UnityEngine;
using UnityEngine.UI;

namespace Osiris
{
    public class Lifebar : MonoBehaviour
    {
        [SerializeField] private Image fillImage = default;

        public void UpdateLifebar(Vector2 anchorPos, float fillAmount) {
            RectTransform cachedTransform = transform as RectTransform;
            cachedTransform.anchoredPosition = anchorPos;

            fillImage.fillAmount = fillAmount;
        }
    }
}