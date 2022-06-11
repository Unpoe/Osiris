using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Osiris
{
    public class BattleEditorUI : MonoBehaviour
    {
        [SerializeField] private ActorDefinition[] definitions = default;
        [Space]
        [SerializeField] private SelectActorPill selectActorPillPrefab = default;
        [SerializeField] private RectTransform actorPillHolder = default;
        [SerializeField] private Toggle allyToggle = default;

        private List<SelectActorPill> actorPillInstances = new List<SelectActorPill>();

        public ActorId selectedActorId { get; private set; }
        public bool ally { get; private set; } = true;

        public void Initialize() {
            // Instantiate actor pills
            for(int i = 0; i < definitions.Length; i++) {
                ActorDefinition actorDef = definitions[i];
                SelectActorPill pill = Instantiate(selectActorPillPrefab, actorPillHolder);
                pill.Initialize(actorDef, OnActorPillClicked);
                pill.SetSelected(false);
                actorPillInstances.Add(pill);
            }

            // Select the first pill by default
            OnActorPillClicked(actorPillInstances[0]);

            // Ally/Enemy toggle
            allyToggle.onValueChanged.AddListener(OnAllyToggleValueChanged);

            // Clear actors button

            // Start battle button

            // Time control slider
        }

        private void OnActorPillClicked(SelectActorPill clickedPill) {
            for(int i = 0; i < actorPillInstances.Count; i++) {
                SelectActorPill instance = actorPillInstances[i];
                instance.SetSelected(false);
            }

            clickedPill.SetSelected(true);
            selectedActorId = clickedPill.actorDefinition.Id;
        }

        private void OnAllyToggleValueChanged(bool value) {
            ally = value;
        }
    }
}