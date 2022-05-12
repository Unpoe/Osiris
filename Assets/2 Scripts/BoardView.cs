using UnityEngine;

namespace Osiris
{
    public class BoardView : MonoBehaviour
    {
        [SerializeField] private HexView hexViewPrefab = default;

        private HexGrid grid = null;

        private HexView[] hexViewInstances = null;

        public void Initialize(HexGrid grid) {
            this.grid = grid;

            hexViewInstances = new HexView[grid.cells.Length];
            for (int i = 0; i < grid.cells.Length; i++) {
                HexCell cell = grid.cells[i];

                HexView hexView = hexViewInstances[i] = Instantiate(hexViewPrefab, transform);
                hexView.Initialize(cell);
            }
        }
    }
}