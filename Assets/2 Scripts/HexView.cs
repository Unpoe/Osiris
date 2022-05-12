using UnityEngine;
using TMPro;

namespace Osiris
{
    public class HexView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI coordinatesLabel = default;

        public void Initialize(HexCell cell) {
            transform.localPosition = cell.worldPosition;
            transform.localScale = Vector3.one * (HexMetrics.outerRadius * 2f);
            coordinatesLabel.text = cell.coordinates.ToStringOnSeparateLines();
        }
    }
}