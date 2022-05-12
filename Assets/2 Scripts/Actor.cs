using UnityEngine;

namespace Osiris
{
    public class Actor : MonoBehaviour
    {
        private int hp;

        private HexCell currentCell, nextCell;
        private HexDirection currentDir, targetDir;

        public void Initialize(int maxHP, HexCell startingCell, HexDirection startingDir) {
            hp = maxHP;

            currentCell = startingCell;
            transform.position = currentCell.worldPosition;

            currentDir = startingDir;
            transform.rotation = Quaternion.Euler(0, currentDir.GetYAngle(), 0);
        }

        public bool GameUpdate(float dt) {
            // Check for death
            if(hp <= 0) {
                return false;
            }

            return true;
        }
    }
}