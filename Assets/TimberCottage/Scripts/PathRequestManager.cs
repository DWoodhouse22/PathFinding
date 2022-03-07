using System;
using System.Collections.Generic;
using UnityEngine;

namespace TimberCottage.Pathfinding
{
    [RequireComponent(typeof(PathFinding))]
    public class PathRequestManager : MonoBehaviour {

        private readonly Queue<PathRequest> _pathRequestQueue = new Queue<PathRequest>();
        private PathRequest _currentPathRequest;
        private PathFinding _pathfinding;
        private bool _isProcessingPath;

        private void Awake() 
        {
            _pathfinding = GetComponent<PathFinding>();
        }

        public void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
        {
            PathRequest newRequest = new PathRequest(pathStart,pathEnd,callback);
            _pathRequestQueue.Enqueue(newRequest);
            TryProcessNext();
        }

        private void TryProcessNext()
        {
            if (_isProcessingPath || _pathRequestQueue.Count <= 0)
            {
                return;
            }

            _currentPathRequest = _pathRequestQueue.Dequeue();
            _isProcessingPath = true;
            _pathfinding.StartFindPath(_currentPathRequest.PathStart, _currentPathRequest.PathEnd);
        }

        public void FinishedProcessingPath(Vector3[] path, bool success)
        {
            _currentPathRequest.Callback(path,success);
            _isProcessingPath = false;
            TryProcessNext();
        }

        private readonly struct PathRequest 
        {
            public Vector3 PathStart { get; }
            public Vector3 PathEnd { get; }
            public Action<Vector3[], bool> Callback { get; }

            public PathRequest(Vector3 start, Vector3 end, Action<Vector3[], bool> callback) 
            {
                PathStart = start;
                PathEnd = end;
                Callback = callback;
            }
        }
    }
}
