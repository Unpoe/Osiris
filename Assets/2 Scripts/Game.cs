using System.Collections.Generic;
using UnityEngine;

namespace Osiris
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private HexGrid grid = default;
        [SerializeField] private ActorFactory actorFactory = default;
        [Space]
        public int mapWidth = 6;
        public int mapHeight = 6;
        [Space]
        public int initialGold = 5;

        private Gold gold = null;

        private List<Actor> allyActors = new List<Actor>();
        private List<Actor> enemyActors = new List<Actor>();

        private void Awake() {
            CustomRandom.SetSeed(0);

            gold = new Gold(initialGold);

            grid.Initialize(mapWidth, mapHeight);

            AddActor(0, 0, true, false);
            AddActor(3, 5, false, false);
        }

        private void Update() {
            float dt = Time.deltaTime;

            UpdateActorList(dt, ref allyActors);
            UpdateActorList(dt, ref enemyActors);
        }

        private void AddActor(int x, int z, bool isAlly, bool dummy) {
            Actor newActor = actorFactory.Get(dummy);
            HexCell startingCell = grid.GetCell(x, z);
            HexDirection startingDir = isAlly ? HexDirection.NE : HexDirection.SW;

            newActor.Initialize(isAlly, 10, startingCell, startingDir, GetActorList, grid.FindPath);

            List<Actor> actors = GetActorList(isAlly);
            actors.Add(newActor);
        }

        private List<Actor> GetActorList(bool isAlly) {
            return isAlly ? allyActors : enemyActors;
        }

        private void UpdateActorList(float dt, ref List<Actor> actors) {
            for (int i = 0; i < actors.Count; i++) {
                Actor actor = actors[i];
                if (!actor.GameUpdate(dt)) {
                    int lastIndex = actors.Count - 1;
                    actors[i] = actors[lastIndex];
                    actors.RemoveAt(lastIndex);
                    i -= 1;
                    actorFactory.Reclaim(actor);
                }
            }
        }
    }
}