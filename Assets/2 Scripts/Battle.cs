﻿using System.Collections.Generic;

namespace Osiris
{
    public class Battle
    {
        public bool running { get; private set; }

        private ActorFactory actorFactory;
        public HexGrid grid; // This is public so actor can access it

        private Gold gold;

        private List<Actor> allyActors;
        private List<Actor> enemyActors;
        public IReadOnlyList<Actor> AllyActors => allyActors;
        public IReadOnlyList<Actor> EnemyActors => enemyActors;

        private int lastBattleId;

        private static readonly List<Actor> EMPTY_ACTOR_LIST = new List<Actor>();

        public Battle(ActorFactory actorFactory, HexGrid grid, int initialGold) {
            this.actorFactory = actorFactory;
            this.grid = grid;

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
                actorFactory.Reclaim(actor);
            }
            allyActors.Clear();

            for (int i = enemyActors.Count - 1; i >= 0; i--) {
                Actor actor = enemyActors[i];
                actor.Clear();
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
            UpdateActorList(dt, ref allyActors);
            UpdateActorList(dt, ref enemyActors);
        }

        public void AddActor(HexCell startingCell, bool isAlly, ActorId actorId) {
            Actor newActor = actorFactory.Get(actorId);
            HexDirection startingDir = isAlly ? HexDirection.NE : HexDirection.SW;

            newActor.Initialize(lastBattleId, isAlly, startingCell, startingDir, this);
            lastBattleId++;

            List<Actor> actors = isAlly ? allyActors : enemyActors;
            actors.Add(newActor);
        }

        public void RemoveActor(HexCell cell) {
            Actor actor = cell.Actor;
            if (actor != null) {
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