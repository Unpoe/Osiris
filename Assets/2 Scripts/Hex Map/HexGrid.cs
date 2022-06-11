using System.Collections.Generic;
using UnityEngine;

namespace Osiris
{
    public class HexGrid : MonoBehaviour
    {
        [SerializeField] private HexCell hexPrefab = default;

        public int width { get; private set; }
        public int height { get; private set; }

        private HexCell[] cells = null;

        private HexCellPriorityQueue searchFrontier;

        public void Initialize(int width, int height) {
            this.width = width;
            this.height = height;

            cells = new HexCell[width * height];

            for (int z = 0, i = 0; z < height; z++) {
                for (int x = 0; x < width; x++) {
                    HexCell cell = cells[i] = Instantiate(hexPrefab, transform);
                    cell.Initialize(x, z, i);

                    // Connecting Neighbors
                    if(x > 0) { // Connecting from east to west
                        cell.SetNeighbor(HexDirection.W, cells[i - 1]);
                    }

                    if (z > 0) {
                        if ((z & 1) == 0) { // Connecting from NW to SE on even rows
                            cell.SetNeighbor(HexDirection.SE, cells[i - width]);
                            if (x > 0) { // Connect the SW as well
                                cell.SetNeighbor(HexDirection.SW, cells[i - width - 1]);
                            }
                        } else {
                            // Odd rows follow the same logic as even but mirrored
                            cell.SetNeighbor(HexDirection.SW, cells[i - width]);
                            if (x < width - 1) {
                                cell.SetNeighbor(HexDirection.SE, cells[i - width + 1]);
                            }
                        }
                    }

                    i++;
                }
            }
        }

        public void Clear() {
            for(int i = 0; i < cells.Length; i++) {
                HexCell cell = cells[i];
                cell.Clear();
            }
        }

        public HexCell GetCell(HexCoordinates coordinates) {
            for (int i = 0; i < cells.Length; i++) {
                HexCell cell = cells[i];
                if (cell.coordinates.Equals(coordinates)) {
                    return cell;
                }
            }

            return null;
        }

        public bool FindPath(HexCell fromCell, HexCell toCell, Actor actor, ref List<HexCell> path) {
            // The frontier is a collection to keep track of which cells we have to visit
            if (searchFrontier == null) {
                searchFrontier = new HexCellPriorityQueue();
            } else {
                searchFrontier.Clear();
            }

            path.Clear();
            for (int i = 0; i < cells.Length; i++) {
                cells[i].Distance = int.MaxValue;
            }

            // We add the first cell into the frontier
            fromCell.Distance = 0;
            searchFrontier.Enqueue(fromCell);
            // We loop until we do not have anything else to check
            while (searchFrontier.Count > 0) {
                HexCell current = searchFrontier.Dequeue();

                if(current == toCell) { // We have found the path
                    path.Add(current);
                    current = current.PathFrom;
                    while (current != fromCell) {
                        path.Add(current);
                        current = current.PathFrom;
                    }
                    path.Add(fromCell);
                    path.Reverse();
                    return true;
                }

                // We loop through the neighbours and add them to the frontier (if needed)
                for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
                    HexCell neighbor = current.GetNeighbor(d);

                    if (neighbor == null) {
                        continue;
                    }

                    // Skip cells with other actors in it, except if that cell is our destination
                    if (neighbor.Actor != null && neighbor.Actor != actor && neighbor != toCell) {
                        continue;
                    }

                    int distance = current.Distance + 1;

                    if (neighbor.Distance == int.MaxValue) { // If the neighbor have not been visited
                        neighbor.Distance = distance;
                        neighbor.PathFrom = current;
                        neighbor.SearchHeuristic = neighbor.coordinates.DistanceTo(toCell.coordinates);
                        searchFrontier.Enqueue(neighbor);
                    } else if (distance < neighbor.Distance) { // If we found a shortest path, update it
                        int oldPriority = neighbor.SearchPriority;
                        neighbor.Distance = distance;
                        neighbor.PathFrom = current;
                        searchFrontier.Change(neighbor, oldPriority);
                    }
                }
            }

            return false;
        }
    }
}