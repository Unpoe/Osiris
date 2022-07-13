using System.Collections.Generic;
using UnityEngine;

namespace Osiris
{
    public class LifebarGroup : MonoBehaviour
    {
        [SerializeField] private Lifebar lifebarPrefab = default;
        [SerializeField] private RectTransform lifebarHolder = default;

        private Camera mainCamera = null;

        private Pool<Lifebar> lifebarPool = null;
        private List<Lifebar> lifebarInstances = null;

        public void Initialize(Camera mainCamera) {
            this.mainCamera = mainCamera;
            lifebarPool = new Pool<Lifebar>(CreateLifebar);
            lifebarInstances = new List<Lifebar>();
        }

        public void GameUpdate() {
            for (int i = 0; i < lifebarInstances.Count; i++) {
                Lifebar lifebar = lifebarInstances[i];
                lifebar.GameUpdate();
            }
        }

        public void OnActorAdded(Actor actor) {
            Lifebar lifebar = lifebarPool.Get();
            lifebar.Initialize(actor, mainCamera);
            lifebarInstances.Add(lifebar);
        }

        public void OnActorRemoved(Actor actor) {
            Lifebar assignedLifebar = null;
            for(int i = 0; i < lifebarInstances.Count; i++) {
                Lifebar lifebar = lifebarInstances[i];
                if (lifebar.HasActorAssigned(actor)) {
                    assignedLifebar = lifebar;
                    break;
                }
            }

            if(assignedLifebar == null) {
                Debug.LogWarning($"[LifebarGroup] Actor with battle id {actor.battleId} does not have a lifebar assigned. No lifebar will be removed.");
                return;
            }

            lifebarInstances.Remove(assignedLifebar);
            lifebarPool.Return(assignedLifebar);
        }

        private Lifebar CreateLifebar() {
            return Instantiate(lifebarPrefab, lifebarHolder);
        }
    }
}