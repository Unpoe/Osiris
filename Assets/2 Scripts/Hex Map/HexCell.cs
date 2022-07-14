using UnityEngine;
using TMPro;

namespace Osiris
{
    public class HexCell : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI coordinatesLabel = default;
        [SerializeField] private Renderer cellRenderer = default;

        public HexCoordinates coordinates { get; private set; }
        public Vector3 worldPosition { get; private set; }
        public int index { get; private set; }

        private HexCell[] neighbors;

        private Actor actor;
        public Actor Actor {
            get {
                return actor;
            }
            set {
                actor = value;
                //SetColor(actor != null ? Color.green : Color.white);
            }
        }

        // Pathfinding
        public int Distance { get; set; } // Distance between this cell and the origin one
        public HexCell PathFrom { get; set; } // The cell from which we set the distance, used to reconstruct the path after the pathfinding
        public int SearchHeuristic { get; set; } // This heuristic represents the best guess of the remaining distance
        public int SearchPriority {
            get {
                return Distance + SearchHeuristic;
            }
        }
        public HexCell NextWithSamePriority { get; set; }

        private static int colorPropertyId = Shader.PropertyToID("_Color");
        private static MaterialPropertyBlock sharedPropertyBlock;

        public void Initialize(int x, int z, int i, bool debugMode) {
            coordinates = HexCoordinates.FromOffsetCoordinates(x, z);

            Vector3 position;
            position.x = (x + z * 0.5f - z / 2) * HexMetrics.innerRadius * 2f;
            position.y = 0f;
            position.z = z * HexMetrics.outerRadius * 1.5f;

            worldPosition = position;

            neighbors = new HexCell[6]; // One for each HexDirection

            Actor = null;

            transform.localPosition = worldPosition;
            transform.localScale = Vector3.one * (HexMetrics.outerRadius * 2f);
            coordinatesLabel.text = coordinates.ToStringOnSeparateLines();
            coordinatesLabel.gameObject.SetActive(debugMode);
        }

        public void Clear() {
            Actor = null;
        }

        public HexCell GetNeighbor(HexDirection direction) {
            return neighbors[(int)direction];
        }

        public void SetNeighbor(HexDirection direction, HexCell cell) {
            neighbors[(int)direction] = cell;
            cell.neighbors[(int)direction.Opposite()] = this;
        }

        public void SetColor(Color color) {
            if (sharedPropertyBlock == null) {
                sharedPropertyBlock = new MaterialPropertyBlock();
            }
            sharedPropertyBlock.SetColor(colorPropertyId, color);
            cellRenderer.SetPropertyBlock(sharedPropertyBlock);
        }

        public static HexDirection GetDirection(HexCell fromCell, HexCell toCell) {
            for(int i = 0; i < fromCell.neighbors.Length; i++) {
                HexCell neighbor = fromCell.neighbors[i];
                if(neighbor != null && neighbor == toCell) {
                    return (HexDirection)i;
                }
            }

            return HexDirection.NE;
        }
    }
}