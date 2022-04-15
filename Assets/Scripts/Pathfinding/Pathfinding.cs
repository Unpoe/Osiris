using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Osiris.Pathfinding
{
    public class Pathfinding : MonoBehaviour
    {
        [SerializeField] private Grid grid = default;
        [Space]
        public bool simplifyPaths = false;

        public delegate void PathfindingFinsihedCallback(Vector3[] path, bool success);

        public void StartFindPath(Vector3 startPos, Vector3 targetPos, PathfindingFinsihedCallback callback) {
            StartCoroutine(FindPath(startPos, targetPos, callback));
        }

        private IEnumerator FindPath(Vector3 startPos, Vector3 targetPos, PathfindingFinsihedCallback callback) {
            Vector3[] wayPoints = null;
            bool success = false;

            Node startNode = grid.NodeFromWorldPoint(startPos);
            Node targetNode = grid.NodeFromWorldPoint(targetPos);

            if(startNode.walkable && targetNode.walkable) {
                Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
                HashSet<Node> closedSet = new HashSet<Node>();
                openSet.Add(startNode);

                while (openSet.Count > 0) {
                    // Find node with the lowest F cost
                    Node currentNode = openSet.RemoveFirst();
                    closedSet.Add(currentNode);

                    if (currentNode == targetNode) {
                        // Target found
                        success = true;
                        break;
                    }

                    List<Node> neighbours = grid.GetNeighbours(currentNode);
                    for (int i = 0; i < neighbours.Count; i++) {
                        Node neighbour = neighbours[i];
                        if (!neighbour.walkable || closedSet.Contains(neighbour)) {
                            continue;
                        }

                        int newMovementCostToNeighbour = currentNode.gCost + currentNode.GetDistance(neighbour);
                        if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                            // Instead of setting the f cost, we just assign the g and h cost (the f cost is the sum of these two)
                            neighbour.gCost = newMovementCostToNeighbour;
                            neighbour.hCost = neighbour.GetDistance(targetNode);

                            // Set "parent"
                            neighbour.parent = currentNode;

                            if (!openSet.Contains(neighbour)) {
                                openSet.Add(neighbour);
                            }
                        }
                    }
                }
            }

            yield return null;

            if (success) {
                wayPoints = RetracePath(startNode, targetNode);
            }
            callback?.Invoke(wayPoints, success);
        }

        private Vector3[] RetracePath(Node startNode, Node endNode) {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while(currentNode != startNode) {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }

            Vector3[] wayPoints = SimplifyPath(path);
            Array.Reverse(wayPoints);

            return wayPoints;
        }

        private Vector3[] SimplifyPath(List<Node> path) {
            List<Vector3> wayPoints = new List<Vector3>();
            Vector2 lastDirection = Vector2.zero;

            for(int i = 1; i < path.Count; i++) {
                if (simplifyPaths) {
                    Vector2 newDir = new Vector2(path[i - 1].x - path[i].x, path[i - 1].y - path[i].y);
                    if (newDir != lastDirection) {
                        // Path changed direction, add it to the waypoints list
                        wayPoints.Add(path[i].worldPosition);
                        lastDirection = newDir;
                    }
                } else {
                    wayPoints.Add(path[i].worldPosition);
                }
            }

            return wayPoints.ToArray();
        }
    }
}