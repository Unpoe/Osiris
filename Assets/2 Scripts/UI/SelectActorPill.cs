using System;
using UnityEngine;
using UnityEngine.UI;

namespace Osiris
{
    public class SelectActorPill : MonoBehaviour
    {
        [SerializeField] private Button button = default;
        [SerializeField] private Image portraitImage = default;
        [SerializeField] private Image overlayImage = default;
        [SerializeField] private Sprite eraseSprite = default;

        public ActorId actorId { get; private set; } = ActorId.None;

        private Action<SelectActorPill> onPillClicked;

        public void Initialize(ActorDefinition actorDef, Action<SelectActorPill> onPillClicked) {
            this.onPillClicked = onPillClicked;

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(delegate { this.onPillClicked?.Invoke(this); });

            if(actorDef != null) {
                actorId = actorDef.Id;
                portraitImage.sprite = actorDef.Portrait;
            } else {
                actorId = ActorId.None;
                portraitImage.sprite = eraseSprite;
            }
        }

        public void SetSelected(bool selected) {
            Color overlayColor = overlayImage.color;
            overlayColor.a = selected ? 0f : 0.7f;
            overlayImage.color = overlayColor;
        }
    }
}