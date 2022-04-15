using UnityEngine;

namespace Unpoe
{
    public class Grid : MonoBehaviour
    {
        public Vector2 gridWorldSize = Vector2.one;
        public float nodeRadius = 1f;
        public LayerMask unwalkableMask = default;
        [Space]
        [SerializeField] private bool showGizmos = false;

        private Node[,] grid = null;

        private float nodeDiameter = 0f;
        private int gridSizeX, gridSizeY = 0;

        private void Awake() {
            nodeDiameter = nodeRadius * 2f;
            gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
            gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

            CreateGrid();
        }

        public Node NodeFromWorldPoint(Vector3 worldPosition) {
            float percentX = Mathf.Clamp01((worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x);
            float percentY = Mathf.Clamp01((worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y);

            int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
            int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

            return grid[x, y];
        }

        private void CreateGrid() {
            grid = new Node[gridSizeX, gridSizeY];
            Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

            for(int x = 0; x < gridSizeX; x++) {
                for(int y = 0; y < gridSizeY; y++) {
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                    bool walkable = !Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask);

                    grid[x, y] = new Node(walkable, worldPoint);
                }
            }
        }

        private void OnDrawGizmos() {
            if (!showGizmos) {
                return;
            }

            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1f, gridWorldSize.y) * nodeRadius);

            if(grid != null) {
                foreach(Node node in grid) {
                    Gizmos.color = node.walkable ? Color.green : Color.red;
                    Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
                }
            }
        }
    }
}