using System.Collections.Generic;
using UnityEngine;
using Osiris.Persistance;

namespace Osiris
{
    public class Game : MonoBehaviour, IPersistableObject
    {
        [SerializeField] private HexGrid grid = default;
        [SerializeField] private ActorFactory actorFactory = default;
        [SerializeField] private BattleEditorUI battleEditorUI = default;
        [SerializeField] private Camera mainCamera = default;
        [SerializeField] private LayerMask groundLayer = default;
        [Header("Battle configuration")]
        public int mapWidth = 6;
        public int mapHeight = 6;
        [Space]
        public int initialGold = 5;

        private PersistentStorage storage;

        private Gold gold = null;

        private bool gameRunning = false;

        private List<Actor> allyActors = new List<Actor>();
        private List<Actor> enemyActors = new List<Actor>();

        private int lastBattleId = 0;

        private static readonly List<Actor> EMPTY_ACTOR_LIST = new List<Actor>();

        private void Awake() {
            storage = new PersistentStorage();

            CustomRandom.SetSeed(0);

            gold = new Gold(initialGold);
            grid.Initialize(mapWidth, mapHeight);
            actorFactory.Initialize();

            battleEditorUI.Initialize(ClearGame, StartGame, RestartGame);

            storage.Load(this);
        }

        private void Update() {
            // Input to add or remove heroes from the grid (only in edit mode)
            if (Input.GetMouseButtonDown(0) && !gameRunning) {
                Plane floorPlane = new Plane(Vector3.up, Vector3.zero);
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                
                if(Physics.Raycast(ray, out RaycastHit hitInfo, float.MaxValue, groundLayer)) {
                    HexCoordinates coordinates = HexCoordinates.FromPosition(hitInfo.point);
                    HexCell selectedCell = grid.GetCell(coordinates);
                    if(selectedCell != null) {
                        // ActorId.None is used for the erase actor button
                        if(battleEditorUI.selectedActorId == ActorId.None) {
                            Actor selectedActor = selectedCell.Actor;
                            if(selectedActor != null) {
                                List<Actor> actors = selectedActor.ally ? allyActors : enemyActors;
                                actors.Remove(selectedActor);
                                actorFactory.Reclaim(selectedActor);
                                selectedCell.Clear();
                            }
                        } else {
                            AddActor(selectedCell, battleEditorUI.ally, battleEditorUI.selectedActorId);
                        }
                        storage.Save(this);
                    }
                }
            }

            float dt = Time.deltaTime * battleEditorUI.timeScale;

            UpdateActorList(dt, ref allyActors);
            UpdateActorList(dt, ref enemyActors);
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

            grid.Clear();
        }

        private void RestartGame() {
            storage.Load(this);
        }

        private void StartGame() {
            gameRunning = true;
        }

        private void AddActor(HexCell startingCell, bool isAlly, ActorId actorId) {
            Actor newActor = actorFactory.Get(actorId);
            HexDirection startingDir = isAlly ? HexDirection.NE : HexDirection.SW;

            newActor.Initialize(lastBattleId, isAlly, startingCell, startingDir, GetActorList, grid.FindPath);
            lastBattleId++;

            List<Actor> actors = isAlly ? allyActors : enemyActors;
            actors.Add(newActor);
        }

        private List<Actor> GetActorList(bool isAlly) {
            if (!gameRunning) {
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

        public void Save(GameDataWriter writer) {
            // Save ally actors
            writer.Write(allyActors.Count);
            for(int i = 0; i < allyActors.Count; i++) {
                Actor actor = allyActors[i];
                writer.Write(actor.ally);
                writer.Write((int)actor.Id);
                writer.Write(actor.Coordinates);
            }

            // Save enemy actors
            writer.Write(enemyActors.Count);
            for (int i = 0; i < enemyActors.Count; i++) {
                Actor actor = enemyActors[i];
                writer.Write(actor.ally);
                writer.Write((int)actor.Id);
                writer.Write(actor.Coordinates);
            }
        }

        public void Load(GameDataReader reader) {
            ClearGame();

            // Load ally actors
            int allyCount = reader.ReadInt();
            for(int i = 0; i < allyCount; i++) {
                bool ally = reader.ReadBool();
                ActorId id = (ActorId)reader.ReadInt();
                HexCoordinates coordinates = reader.ReadHexCoordinates();
                HexCell startingCell = grid.GetCell(coordinates);

                AddActor(startingCell, ally, id);
            }

            // Load enemy actors
            int enemyCount = reader.ReadInt();
            for (int i = 0; i < enemyCount; i++) {
                bool ally = reader.ReadBool();
                ActorId id = (ActorId)reader.ReadInt();
                HexCoordinates coordinates = reader.ReadHexCoordinates();
                HexCell startingCell = grid.GetCell(coordinates);

                AddActor(startingCell, ally, id);
            }

            gameRunning = false;
        }
    }
}