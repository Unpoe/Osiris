using System.Collections.Generic;
using UnityEngine;

namespace Osiris
{
    public class Game : MonoBehaviour
    {
        [SerializeField, Range(1, 10)] private float timeScale = 1f;
        [Space]
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
            actorFactory.Initialize();

            NewGame();
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.N)) {
                ClearGame();
                NewGame();
                return;
            }

            float dt = Time.deltaTime * timeScale;

            UpdateActorList(dt, ref allyActors);
            UpdateActorList(dt, ref enemyActors);
        }

        private void NewGame() {
            AddActor(0, 0, true, ActorId.Waifu);
            //AddActor(2, 1, true, ActorId.Waifu);
            //AddActor(4, 0, true, ActorId.Waifu);
            AddActor(3, 5, false, ActorId.Vampire);
        }

        private void ClearGame() {
            for(int i = allyActors.Count - 1; i >= 0; i--) {
                Actor actor = allyActors[i];
                actor.Clear();
                actorFactory.Reclaim(actor);
            }
            allyActors.Clear();

            for (int i = enemyActors.Count - 1; i >= 0; i--) {
                Actor actor = enemyActors[i];
                actor.Clear();
                actorFactory.Reclaim(actor);
            }
            enemyActors.Clear();
        }

        private void AddActor(int x, int z, bool isAlly, ActorId actorId) {
            Actor newActor = actorFactory.Get(actorId);
            HexCell startingCell = grid.GetCell(x, z);
            HexDirection startingDir = isAlly ? HexDirection.NE : HexDirection.SW;

            newActor.Initialize(isAlly, startingCell, startingDir, GetActorList, grid.FindPath);

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
                    actor.Clear();
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