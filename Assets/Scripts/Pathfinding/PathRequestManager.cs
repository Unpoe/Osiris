using System.Collections.Generic;
using UnityEngine;

namespace Osiris.Pathfinding
{
    public class PathRequestManager : MonoBehaviour
    {
        [SerializeField] private Pathfinding pathFinding = default;

        private Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
        private PathRequest currentPathRequest;

        private bool isProcessingPath = false;

        public delegate void RequestCallback(Vector3[] path, bool success);

        public void RequestPath(Vector3 pathStart, Vector3 pathEnd, RequestCallback callback) {
            PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
            pathRequestQueue.Enqueue(newRequest);
            TryProcessNext();
        }

        private void TryProcessNext() {
            if (!isProcessingPath && pathRequestQueue.Count > 0) {
                currentPathRequest = pathRequestQueue.Dequeue();
                isProcessingPath = true;
                pathFinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd, FinishedProcessingPath);
            }
        }

        private void FinishedProcessingPath(Vector3[] path, bool success) {
            currentPathRequest.callback?.Invoke(path, success);
            isProcessingPath = false;
            TryProcessNext();
        }

        private struct PathRequest
        {
            public Vector3 pathStart;
            public Vector3 pathEnd;
            public RequestCallback callback;

            public PathRequest(Vector3 pathStart, Vector3 pathEnd, RequestCallback callback) {
                this.pathStart = pathStart;
                this.pathEnd = pathEnd;
                this.callback = callback;
            }
        }
    }
}