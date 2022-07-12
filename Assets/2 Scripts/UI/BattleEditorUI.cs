using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Osiris
{
    public class BattleEditorUI : MonoBehaviour
    {
        [SerializeField] private ActorTable actorTable = default;
        [Space]
        [SerializeField] private SelectActorPill selectActorPillPrefab = default;
        [SerializeField] private RectTransform actorPillHolder = default;
        [SerializeField] private Toggle allyToggle = default;
        [SerializeField] private Button clearActorsButton = default;
        [SerializeField] private Button startBattleButton = default;
        [SerializeField] private Button restartBattleButton = default;

        private List<SelectActorPill> actorPillInstances = new List<SelectActorPill>();

        public ActorId selectedActorId { get; private set; }
        public bool ally { get; private set; } = true;

        public void Initialize(Action clearActorsAction, Action startBattleAction, Action restartBattleAction) {
            // Instantiate actor pills
            IReadOnlyList<ActorDefinition> definitions = actorTable.GetDefinitionsWithoutMocks();
            for(int i = 0; i < definitions.Count; i++) {
                ActorDefinition actorDef = definitions[i];
                SelectActorPill pill = Instantiate(selectActorPillPrefab, actorPillHolder);
                pill.Initialize(actorDef, OnActorPillClicked);
                pill.SetSelected(false);
                actorPillInstances.Add(pill);
            }
            // Instantiate an extra pill for the erase button
            SelectActorPill erasePill = Instantiate(selectActorPillPrefab, actorPillHolder);
            erasePill.Initialize(null, OnActorPillClicked);
            erasePill.SetSelected(false);
            actorPillInstances.Add(erasePill);

            // Select the first pill by default
            OnActorPillClicked(actorPillInstances[0]);

            // Ally/Enemy toggle
            allyToggle.onValueChanged.AddListener(OnAllyToggleValueChanged);

            // Clear actors button
            clearActorsButton.onClick.RemoveAllListeners();
            clearActorsButton.onClick.AddListener(delegate { clearActorsAction?.Invoke(); });

            // Start battle button
            startBattleButton.onClick.RemoveAllListeners();
            startBattleButton.onClick.AddListener(delegate { startBattleAction?.Invoke(); });

            // Restart battle button
            restartBattleButton.onClick.RemoveAllListeners();
            restartBattleButton.onClick.AddListener(delegate { restartBattleAction?.Invoke(); });
        }

        private void OnActorPillClicked(SelectActorPill clickedPill) {
            for(int i = 0; i < actorPillInstances.Count; i++) {
                SelectActorPill instance = actorPillInstances[i];
                instance.SetSelected(false);
            }

            clickedPill.SetSelected(true);
            selectedActorId = clickedPill.actorId;
        }

        private void OnAllyToggleValueChanged(bool value) {
            ally = value;
        }
    }
}