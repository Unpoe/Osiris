using UnityEngine;

namespace Unpoe
{
    public class Node
    {
        public bool walkable;
        public Vector3 worldPosition;

        public Node(bool walkable, Vector3 worldPosition) {
            this.walkable = walkable;
            this.worldPosition = worldPosition;
        }
    }
}