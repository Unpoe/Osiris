using System.Collections.Generic;
using UnityEngine;

namespace Osiris
{
    public class ActorFactory : MonoBehaviour
    {
        [SerializeField] private ActorTable actorTable = default;

        private List<Actor>[] actorInstances = null;

        public void Initialize() {
            actorInstances = new List<Actor>[actorTable.GetDefinitions().Count];
        }

        public Actor Get(ActorId actorId) {
            Actor instance;

            int id = (int)actorId;
            List<Actor> instances;
            if(actorInstances[id] == null) {
                actorInstances[id] = new List<Actor>();
            }
            instances = actorInstances[id];

            if(instances.Count > 0) {
                instance = instances[0];
                instances.Remove(instance);

                instance.gameObject.SetActive(true);
            } else {
                ActorDefinition actorDef = actorTable.GetDefinition(actorId);

                instance = Instantiate(actorDef.Prefab, transform);
                instance.GameAwake();
            }

            return instance;
        }

        public void Reclaim(Actor actor) {
            int id = (int)actor.Id;
            List<Actor> instances;
            if (actorInstances[id] == null) {
                actorInstances[id] = new List<Actor>();
            }
            instances = actorInstances[id];

            instances.Add(actor);
            actor.gameObject.SetActive(false);
        }
    }
}