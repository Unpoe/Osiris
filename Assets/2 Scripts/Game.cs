using System.Collections.Generic;
using UnityEngine;

namespace Osiris
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private BoardView boardView = default;
        [SerializeField] private ActorFactory actorFactory = default;
        [Space]
        public int mapWidth = 6;
        public int mapHeight = 6;
        [Space]
        public int initialGold = 5;

        private HexGrid grid = null;
        private Gold gold = null;

        private List<Actor> actors = new List<Actor>();

        private void Awake() {
            grid = new HexGrid(mapWidth, mapHeight);
            gold = new Gold(initialGold);

            boardView.Initialize(grid);

            AddActor(0, 0, false);
            AddActor(3, 5, true);
        }

        private void Update() {
            float dt = Time.deltaTime;

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

        private void AddActor(int x, int z, bool isEnemy) {
            Actor newActor = actorFactory.Get();
            HexCell startingCell = grid.GetCell(x, z);
            HexDirection startingDir = isEnemy ? HexDirection.SW : HexDirection.NE;

            newActor.Initialize(10, startingCell, startingDir);

            actors.Add(newActor);
        }
    }
}