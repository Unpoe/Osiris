namespace Osiris
{
    public class HexGrid
    {
        public readonly int width;
        public readonly int height;

        public readonly HexCell[] cells;

        public HexGrid(int width, int height) {
            this.width = width;
            this.height = height;

            cells = new HexCell[width * height];

            for (int z = 0, i = 0; z < height; z++) {
                for (int x = 0; x < width; x++) {
                    HexCell cell = cells[i] = new HexCell(x, z, i++);

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
                }
            }
        }

        public HexCell GetCell(int x, int z) {
            return cells[x + z * width];
        }
    }
}