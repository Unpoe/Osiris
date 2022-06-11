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
        [SerializeField] private BattleEditorUI battleEditorUI = default;
        [SerializeField] private Camera mainCamera = default;
        [SerializeField] private LayerMask groundLayer = default;
        [Space]
        public int mapWidth = 6;
        public int mapHeight = 6;
        [Space]
        public int initialGold = 5;

        private Gold gold = null;

        private bool gameRunning = false;

        private List<Actor> allyActors = new List<Actor>();
        private List<Actor> enemyActors = new List<Actor>();

        private int lastBattleId = 0;

        private static readonly List<Actor> EMPTY_ACTOR_LIST = new List<Actor>();

        private void Awake() {
            CustomRandom.SetSeed(0);

            gold = new Gold(initialGold);
            grid.Initialize(mapWidth, mapHeight);
            actorFactory.Initialize();

            battleEditorUI.Initialize();

            NewGame();
        }

        private void Update() {
            if (Input.GetMouseButtonDown(0)) {
                Plane floorPlane = new Plane(Vector3.up, Vector3.zero);
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                
                if(Physics.Raycast(ray, out RaycastHit hitInfo, float.MaxValue, groundLayer)) {
                    HexCoordinates coordinates = HexCoordinates.FromPosition(hitInfo.point);
                    HexCell startingCell = grid.GetCell(coordinates);
                    if(startingCell != null) {
                        AddActor(startingCell, battleEditorUI.ally, battleEditorUI.selectedActorId);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                gameRunning = true;
                return;
            } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
                ClearGame();
                NewGame();
                return;
            }

            float dt = Time.deltaTime * timeScale;

            UpdateActorList(dt, ref allyActors);
            UpdateActorList(dt, ref enemyActors);
        }

        private void NewGame() {
            gameRunning = false;
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
    }
}