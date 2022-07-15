using System.Collections.Generic;
using UnityEngine;

namespace Osiris
{
    public class Battle
    {
        // Only used for debugging
        private float timeScale;

        public bool running { get; private set; }

        private ActorFactory actorFactory;
        public HexGrid grid { get; private set; }
        private LifebarGroup lifebarGroup;
        public FloatingNumbersGroup floatingNumbersGroup { get; private set; }
        private Catalog catalog;

        private Gold gold;

        private List<Actor> allyActors;
        private List<Actor> enemyActors;
        public IReadOnlyList<Actor> AllyActors => allyActors;
        public IReadOnlyList<Actor> EnemyActors => enemyActors;

        private int lastBattleId;

        private static readonly List<Actor> EMPTY_ACTOR_LIST = new List<Actor>();

        public Battle(ActorFactory actorFactory, HexGrid grid, LifebarGroup lifebarGroup, FloatingNumbersGroup floatingNumbersGroup, Catalog catalog, int initialGold) {
            this.actorFactory = actorFactory;
            this.grid = grid;
            this.lifebarGroup = lifebarGroup;
            this.floatingNumbersGroup = floatingNumbersGroup;
            this.catalog = catalog;

            timeScale = 1f;

            gold = new Gold(initialGold);

            allyActors = new List<Actor>();
            enemyActors = new List<Actor>();

            lastBattleId = 0;

            running = false;
        }

        public void Clear() {
            for (int i = allyActors.Count - 1; i >= 0; i--) {
                Actor actor = allyActors[i];
                actor.Clear();
                lifebarGroup.OnActorRemoved(actor);
                actorFactory.Reclaim(actor);
            }
            allyActors.Clear();

            for (int i = enemyActors.Count - 1; i >= 0; i--) {
                Actor actor = enemyActors[i];
                actor.Clear();
                lifebarGroup.OnActorRemoved(actor);
                actorFactory.Reclaim(actor);
            }
            enemyActors.Clear();

            grid.Clear();

            running = false;
        }

        public void Start() {
            running = true;
        }

        public void GameUpdate(float dt) {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (Input.GetKeyDown(KeyCode.F1)) {
                timeScale = 1f;
            } else if (Input.GetKeyDown(KeyCode.F2)) {
                timeScale = 2f;
            } else if(Input.GetKeyDown(KeyCode.F3)) {
                timeScale = 4f;
            } else if (Input.GetKeyDown(KeyCode.F4)) {
                timeScale = 0.5f;
            } else if (Input.GetKeyDown(KeyCode.F5)) {
                timeScale = 0.25f;
            } else if (Input.GetKeyDown(KeyCode.F6)) {
                timeScale = 0f;
            }

            dt *= timeScale;
#endif

            UpdateActorList(dt, ref allyActors);
            UpdateActorList(dt, ref enemyActors);

            lifebarGroup.GameUpdate();
            floatingNumbersGroup.GameUpdate(dt);
        }

        public void AddActor(HexCell startingCell, bool isAlly, ActorId actorId) {
            Actor newActor = actorFactory.Get(actorId);
            HexDirection startingDir = isAlly ? HexDirection.NE : HexDirection.SW;
            ActorDefinition actorDef = catalog.ActorTable.GetDefinition(actorId);

            newActor.Initialize(lastBattleId, actorDef, isAlly, startingCell, startingDir, this);
            lastBattleId++;

            List<Actor> actors = isAlly ? allyActors : enemyActors;
            actors.Add(newActor);

            lifebarGroup.OnActorAdded(newActor);
        }

        public void RemoveActor(HexCell cell) {
            Actor actor = cell.Actor;
            if (actor != null) {
                lifebarGroup.OnActorRemoved(actor);

                List<Actor> actors = actor.ally ? allyActors : enemyActors;
                actors.Remove(actor);
                actorFactory.Reclaim(actor);
                cell.Clear();
            }
        }

        public List<Actor> GetActorList(bool isAlly) {
            if (!running) {
                return EMPTY_ACTOR_LIST;
            }

            return isAlly ? allyActors : enemyActors;
        }

        private void UpdateActorList(float dt, ref List<Actor> actors) {
            for (int i = 0; i < actors.Count; i++) {
                Actor actor = actors[i];
                if (!actor.GameUpdate(dt)) {
                    actor.Clear();
                    lifebarGroup.OnActorRemoved(actor);
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