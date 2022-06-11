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

        public ActorDefinition actorDefinition { get; private set; }

        private Action<SelectActorPill> onPillClicked;

        public void Initialize(ActorDefinition actorDef, Action<SelectActorPill> onPillClicked) {
            actorDefinition = actorDef;
            this.onPillClicked = onPillClicked;

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(delegate { this.onPillClicked?.Invoke(this); });

            portraitImage.sprite = actorDef.Portrait;
        }

        public void SetSelected(bool selected) {
            Color overlayColor = overlayImage.color;
            overlayColor.a = selected ? 0f : 0.7f;
            overlayImage.color = overlayColor;
        }
    }
}