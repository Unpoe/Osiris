using System.Collections.Generic;
using UnityEngine;

namespace Osiris
{
    public class ActorFactory : MonoBehaviour
    {
        [SerializeField] private Actor actorPrefab = default;
        [SerializeField] private DummyActor dummyActorPrefab = default;

        private List<Actor> instances = new List<Actor>();

        public Actor Get(bool dummy) {
            Actor instance;
            if (dummy) {
                instance = Instantiate(dummyActorPrefab, transform);
                instance.GameAwake();
                return instance;
            }

            if(instances.Count > 0) {
                instance = instances[0];
                instances.Remove(instance);

                instance.gameObject.SetActive(true);
            } else {
                instance = Instantiate(actorPrefab, transform);
                instance.GameAwake();
            }

            return instance;
        }

        public void Reclaim(Actor actor) {
            DummyActor dummyActor = actor as DummyActor;
            if(dummyActor != null) {
                Destroy(dummyActor.gameObject);
                return;
            }

            instances.Add(actor);
            actor.gameObject.SetActive(false);
        }
    }
}