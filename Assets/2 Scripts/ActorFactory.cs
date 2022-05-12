using System.Collections.Generic;
using UnityEngine;

namespace Osiris
{
    public class ActorFactory : MonoBehaviour
    {
        [SerializeField] private Actor actorPrefab = default;

        private List<Actor> instances = new List<Actor>();

        public Actor Get() {
            Actor instance;
            if(instances.Count > 0) {
                instance = instances[0];
                instances.Remove(instance);

                instance.gameObject.SetActive(true);
            } else {
                instance = Instantiate(actorPrefab, transform);
            }

            return instance;
        }

        public void Reclaim(Actor actor) {
            instances.Add(actor);
            actor.gameObject.SetActive(false);
        }
    }
}