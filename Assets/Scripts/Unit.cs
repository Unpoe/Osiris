using UnityEngine;
using Osiris.Pathfinding;

namespace Osiris
{
    public class Unit : MonoBehaviour
    {
        [SerializeField] private PathRequestManager pathRequestManager = default;
        [SerializeField] private Transform target = default;
        [Space]
        [SerializeField] private float speed = 5f;

        private Vector3[] path;
        private int targetIndex;
        private Vector3 positionFrom, positionTo;
        private float progress;

        private void Start() {
            pathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
        }

        private void Update() {
            CustomUpdate(Time.deltaTime);
        }

        private bool CustomUpdate(float dt) {
            if(path == null) {
                return false;
            }

            progress += dt * speed;

            // First check if we arrived to the end of the waypoint
            while(progress >= 1f) {
                if(targetIndex == path.Length - 1) {
                    // Path is completed
                    path = null;
                    return false;
                }

                positionFrom = positionTo;
                positionTo = path[targetIndex + 1];
                targetIndex++;
                progress -= 1f;
            }

            // Do the actual movement
            transform.localPosition = Vector3.LerpUnclamped(positionFrom, positionTo, progress);

            return true;
        }

        private void OnPathFound(Vector3[] newPath, bool success) {
            if (!success) {
                return;
            }

            path = newPath;
            targetIndex = 0;
            positionFrom = path[targetIndex];
            positionTo = path[targetIndex + 1];
            progress = 0f;
        }
    }
}