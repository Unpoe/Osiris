using System.Collections.Generic;

namespace Osiris
{
    public class Battle
    {
        private HexGrid grid;

        private List<Actor> playerActors;
        private List<Actor> enemyActors;

        public Battle(HexGrid grid) {
            this.grid = grid;

            playerActors = new List<Actor>();
            enemyActors = new List<Actor>();
        }

        private void AddActor(Actor actor, bool fromPlayer, int x, int z) {
            if (fromPlayer) {
                playerActors.Add(actor);
            } else {
                enemyActors.Add(actor);
            }

            grid.cells[x + z * grid.height].actor = actor;
        }

        public State GameUpdate(float dt) {


            return State.Running;
        }

        public enum State
        {
            Running,
            Win,
            Lose
        }
    }
}