using UnityEngine;

namespace Osiris
{
    public class HexCell
    {
        public readonly HexCoordinates coordinates;
        public readonly Vector3 worldPosition;
        public readonly int index;

        private HexCell[] neighbors;

        public Actor actor;

        public HexCell(int x, int z, int i) {
            coordinates = HexCoordinates.FromOffsetCoordinates(x, z);

            Vector3 position;
            position.x = (x + z * 0.5f - z / 2) * HexMetrics.innerRadius * 2f;
            position.y = 0f;
            position.z = z * HexMetrics.outerRadius * 1.5f;

            worldPosition = position;

            neighbors = new HexCell[6]; // One for each HexDirection

            actor = null;
        }

        public HexCell GetNeighbor(HexDirection direction) {
            return neighbors[(int)direction];
        }

        public void SetNeighbor(HexDirection direction, HexCell cell) {
            neighbors[(int)direction] = cell;
            cell.neighbors[(int)direction.Opposite()] = this;
        }

        public bool IsEmpty() {
            return actor == null;
        }
    }
}