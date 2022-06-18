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

        private Battle battle;

        private PersistentStorage storage;

        private void Awake() {
            // App level initialization
            storage = new PersistentStorage();
            CustomRandom.SetSeed(0);

            // Initialize battle dependencies
            actorFactory.Initialize();
            grid.Initialize(mapWidth, mapHeight);

            // Initialize battle
            battle = new Battle(actorFactory, grid, initialGold);
            battleEditorUI.Initialize(battle.Clear, battle.Start, delegate { storage.Load(this); });

            storage.Load(this);
        }

        private void Update() {
            // Input to add or remove heroes from the grid (only in edit mode)
            if (Input.GetMouseButtonDown(0) && !battle.running) {
                Plane floorPlane = new Plane(Vector3.up, Vector3.zero);
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                
                if(Physics.Raycast(ray, out RaycastHit hitInfo, float.MaxValue, groundLayer)) {
                    HexCoordinates coordinates = HexCoordinates.FromPosition(hitInfo.point);
                    HexCell selectedCell = grid.GetCell(coordinates);
                    if(selectedCell != null) {
                        // ActorId.None is used for the erase actor button
                        if(battleEditorUI.selectedActorId == ActorId.None) {
                            battle.RemoveActor(selectedCell);
                        } else {
                            if(selectedCell.Actor == null) { // Only add an actor if the cell does not have one
                                battle.AddActor(selectedCell, battleEditorUI.ally, battleEditorUI.selectedActorId);
                            }
                        }
                        storage.Save(this);
                    }
                }
            }

            float dt = Time.deltaTime * battleEditorUI.timeScale;

            battle.GameUpdate(dt);
        }

        public void Save(GameDataWriter writer) {
            // Save ally actors
            IReadOnlyList<Actor> allyActors = battle.AllyActors;
            writer.Write(allyActors.Count);
            for(int i = 0; i < allyActors.Count; i++) {
                Actor actor = allyActors[i];
                writer.Write(actor.ally);
                writer.Write((int)actor.Id);
                writer.Write(actor.Coordinates);
            }

            // Save enemy actors
            IReadOnlyList<Actor> enemyActors = battle.EnemyActors;
            writer.Write(enemyActors.Count);
            for (int i = 0; i < enemyActors.Count; i++) {
                Actor actor = enemyActors[i];
                writer.Write(actor.ally);
                writer.Write((int)actor.Id);
                writer.Write(actor.Coordinates);
            }
        }

        public void Load(GameDataReader reader) {
            battle.Clear();

            // Load ally actors
            int allyCount = reader.ReadInt();
            for(int i = 0; i < allyCount; i++) {
                bool ally = reader.ReadBool();
                ActorId id = (ActorId)reader.ReadInt();
                HexCoordinates coordinates = reader.ReadHexCoordinates();
                HexCell startingCell = grid.GetCell(coordinates);

                battle.AddActor(startingCell, ally, id);
            }

            // Load enemy actors
            int enemyCount = reader.ReadInt();
            for (int i = 0; i < enemyCount; i++) {
                bool ally = reader.ReadBool();
                ActorId id = (ActorId)reader.ReadInt();
                HexCoordinates coordinates = reader.ReadHexCoordinates();
                HexCell startingCell = grid.GetCell(coordinates);

                battle.AddActor(startingCell, ally, id);
            }
        }
    }
}