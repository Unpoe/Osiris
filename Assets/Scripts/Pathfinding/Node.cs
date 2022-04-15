using UnityEngine;

namespace Osiris.Pathfinding
{
    public class Node : IHeapItem<Node>
    {
        public int x, y;

        public bool walkable;
        public Vector3 worldPosition;

        public int gCost;
        public int hCost;
        public int fCost => gCost + hCost;

        public Node parent;

        public int HeapIndex { get; set; }

        public Node(int x, int y, bool walkable, Vector3 worldPosition) {
            this.x = x;
            this.y = y;

            this.walkable = walkable;
            this.worldPosition = worldPosition;

            gCost = 0;
            hCost = 0;
        }

        public int GetDistance(Node other) {
            int xDist = Mathf.Abs(x - other.x);
            int yDist = Mathf.Abs(y - other.y);

            if(xDist > yDist) {
                return 14 * yDist + 10 * (xDist - yDist);
            } else {
                return 14 * xDist + 10 * (yDist - xDist);
            }
        }

        public int CompareTo(Node other) {
            int compare = fCost.CompareTo(other.fCost);
            if(compare == 0) {
                compare = hCost.CompareTo(other.hCost);
            }

            // int comparison works the opposite way of node comparison
            return -compare;
        }
    }
}