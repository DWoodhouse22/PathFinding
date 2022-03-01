using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace TimberCottage.Pathfinding
{
    [RequireComponent(typeof(PathFindingGrid), typeof(PathRequestManager))]
    public class PathFinding : MonoBehaviour
    {
        [SerializeField] private int diagonalBaseCost = 14;
        [SerializeField] private int adjacentBaseCost = 10;
        private PathRequestManager _requestManager;
        private PathFindingGrid _pathFindingGrid;
        private void Awake()
        {
            _requestManager = GetComponent<PathRequestManager>();
            _pathFindingGrid = GetComponent<PathFindingGrid>();
        }

        public void StartFindPath(Vector3 startPos, Vector3 targetPos)
        {
            StartCoroutine(FindPath(startPos, targetPos));
        }

        IEnumerator FindPath(Vector3 startPos, Vector3 targetpos)
        {
            Vector3[] waypoints = Array.Empty<Vector3>();
            Node startNode = _pathFindingGrid.NodeFromWorldPoint(startPos);
            Node targetNode = _pathFindingGrid.NodeFromWorldPoint(targetpos);
            
            if (startNode.Walkable == false || targetNode.Walkable == false)
            {
                _requestManager.FinishedProcessingPath(waypoints, false);
                yield break;
            }
            
            bool pathFound = false;
            Heap<Node> openSet = new Heap<Node>(_pathFindingGrid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    pathFound = true;
                    break;
                }

                foreach (Node neighbour in _pathFindingGrid.GetNeighbours(currentNode))
                {
                    if (!neighbour.Walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbour) + neighbour.MovementPenalty;
                    if (newMovementCostToNeighbour >= neighbour.GCost && openSet.Contains(neighbour))
                    {
                        continue;
                    }

                    neighbour.GCost = newMovementCostToNeighbour;
                    neighbour.HCost = GetDistance(neighbour, targetNode);
                    neighbour.ParentNode = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                    else
                    {
                        openSet.UpdateItem(neighbour);
                    }
                }
            }
            
            if (pathFound)
            {
                waypoints = RetracePath(startNode, targetNode);
                pathFound = waypoints.Length > 0;
            }

            _requestManager.FinishedProcessingPath(waypoints, pathFound);

            yield return null;
        }

        private Vector3[] RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;
            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.ParentNode;
            }

            Vector3[] waypoints = SimplifyPath(path);
            Array.Reverse(waypoints);
            return waypoints;
        }

        Vector3[] SimplifyPath(List<Node> path)
        {
            List<Vector3> waypoints = new List<Vector3>();
            Vector2 directionOld = Vector2.zero;

            for (int i = 1; i < path.Count; i++)
            {
                Node prevNode = path[i - 1];
                Node currNode = path[i];
                Vector2 directionNew = new Vector2(prevNode.GridX - currNode.GridX, prevNode.GridY - currNode.GridY);
                if (directionNew != directionOld)
                {
                    waypoints.Add(currNode.WorldPosition);
                }

                directionOld = directionNew;
            }

            return waypoints.ToArray();
        }
        
        private int GetDistance(Node nodeA, Node nodeB)
        {
            int distX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
            int distY = Mathf.Abs(nodeA.GridY - nodeB.GridY);
            
            if (distX > distY)
            {
                return diagonalBaseCost * distY + adjacentBaseCost * (distX - distY);
            }

            return diagonalBaseCost * distX + adjacentBaseCost * (distY - distX);
        }
    }
}