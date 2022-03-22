using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Animations;
using Debug = UnityEngine.Debug;

namespace TimberCottage.Pathfinding
{
    public class PathFindingGrid : MonoBehaviour
    {
        [SerializeField] private LayerMask unwalkableMask;
        [SerializeField] private Vector2 gridWorldSize;
        [SerializeField] private float nodeRadius;
        [SerializeField] private bool drawGizmos;
        [SerializeField] private TerrainType[] walkableRegions;
        [SerializeField] private int obstacleProximityPenalty = 10;

        private Node[,] _grid;
        private float _nodeDiameter;
        private int _gridSizeX;
        private int _gridSizeY;
        private readonly Dictionary<int, int> _walkableRegionDictionary = new Dictionary<int, int>();
        private LayerMask _walkableMask;
        
        public int MaxSize => _gridSizeX * _gridSizeY;
        public int GridSizeX => _gridSizeX;
        public int GridSizeY => _gridSizeY;
        
        // For debug only
        private int _penaltyMin = int.MaxValue;
        private int _penaltyMax = int.MinValue;

        private void Awake()
        {
            _nodeDiameter = nodeRadius * 2;
            _gridSizeX = Mathf.RoundToInt(gridWorldSize.x / _nodeDiameter);
            _gridSizeY = Mathf.RoundToInt(gridWorldSize.y / _nodeDiameter);

            foreach (TerrainType region in walkableRegions)
            {
                _walkableMask.value |= region.TerrainMask.value;
                _walkableRegionDictionary.Add((int)Mathf.Log(region.TerrainMask.value, 2), region.TerrainPenalty);
            }
            
            CreateGrid();
        }
        
        private void CreateGrid()
        {
            _grid = new Node[_gridSizeX, _gridSizeY];
            CalculateNodes();
        }

        public void CalculateNodes()
        {
            CalculateNodes(Vector2Int.zero, new Vector2Int(_gridSizeX, _gridSizeY));
        }

        /// <summary>
        /// Calculate movement costs for nodes for specified grid section
        /// </summary>
        /// <param name="origin">Bottom left corner</param>
        /// <param name="size">Rectangle extents</param>
        public void CalculateNodes(Vector2Int origin, Vector2Int size)
        {
            if (origin.x < 0 || origin.y < 0)
            {
                Debug.LogError("CalculateNodes: Origin must be at least Vector2.Zero");
                return;
            }
            Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

            int maxX = origin.x + (size.x * 2) > _gridSizeX ? _gridSizeX : origin.x + (size.x * 2);
            int maxY = origin.y + (size.y * 2) > _gridSizeY ? _gridSizeY : origin.y + (size.y * 2);
            
            for (int x = origin.x; x < maxX; x++)
            {
                for (int y = origin.y; y < maxY; y++)
                {
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * _nodeDiameter + nodeRadius) + Vector3.forward * (y * _nodeDiameter + nodeRadius);
                    int movementPenalty = 0;
                    Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
                    if (Physics.Raycast(ray, out RaycastHit hit, 100, _walkableMask))
                    {
                        _walkableRegionDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                    }

                    bool walkable = !Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask);
                    if (!walkable)
                    {
                        movementPenalty += obstacleProximityPenalty;
                    }
                    
                    _grid[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);
                }
            }
            
            BlurPenaltyMap(3, _gridSizeX, _gridSizeY);
        }

        private void BlurPenaltyMap(int blurSize, int sizeX, int sizeY)
        {
            int kernelSize = blurSize * 2 + 1;
            int kernelExtents = (kernelSize - 1) / 2;
            int[,] penaltiesHorizontalPass = new int[sizeX, sizeY];
            int[,] penaltiesVerticalPass = new int[sizeX, sizeY];

            for (int y = 0; y < sizeY; y++)
            {
                for (int x = -kernelExtents; x <= kernelExtents; x++)
                {
                    int sampleX = Mathf.Clamp(x, 0, kernelExtents);
                    penaltiesHorizontalPass[0, y] += _grid[sampleX, y].MovementPenalty;
                }

                for (int x = 1; x < sizeX; x++)
                {
                    int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, sizeX);
                    int addIndex = Mathf.Clamp(x + kernelExtents, 0, sizeX - 1);

                    penaltiesHorizontalPass[x, y] = penaltiesHorizontalPass[x - 1, y] - _grid[removeIndex, y].MovementPenalty + _grid[addIndex, y].MovementPenalty;
                }
            }
            
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = -kernelExtents; y <= kernelExtents; y++)
                {
                    int sampleY = Mathf.Clamp(y, 0, kernelExtents);
                    penaltiesVerticalPass[x, 0] += penaltiesHorizontalPass[x, sampleY];
                }
                
                int blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, 0] / (kernelSize * kernelSize));
                _grid[x, 0].MovementPenalty = blurredPenalty;

                for (int y = 1; y < sizeY; y++)
                {
                    int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, sizeY);
                    int addIndex = Mathf.Clamp(y + kernelExtents, 0, sizeY - 1);

                    penaltiesVerticalPass[x, y] = penaltiesVerticalPass[x, y - 1] - penaltiesHorizontalPass[x, removeIndex] + penaltiesHorizontalPass[x, addIndex];
                    blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, y] / (kernelSize * kernelSize));
                    _grid[x, y].MovementPenalty = blurredPenalty;

                    if (blurredPenalty > _penaltyMax)
                    {
                        _penaltyMax = blurredPenalty;
                    }

                    if (blurredPenalty < _penaltyMin)
                    {
                        _penaltyMin = blurredPenalty;
                    }
                }
            }
        }

        public List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }

                    int checkX = node.GridX + x;
                    int checkY = node.GridY + y;
                    if (IsValidGridPosition(checkX, checkY) == false)
                    {
                        continue;
                    }
                    
                    neighbours.Add(_grid[checkX, checkY]);
                }
            }
            
            return neighbours;
        }

        private bool IsValidGridPosition(int posX, int posY)
        {
            return posX >= 0 && posX < _gridSizeX && posY >= 0 && posY < _gridSizeY;
        }

        public Node NodeFromWorldPoint(Vector3 worldPosition)
        {
            Vector2Int gridPosition = GridPositionFromWorldPoint(worldPosition);
            return _grid[gridPosition.x, gridPosition.y];
        }

        public Vector2Int GridPositionFromWorldPoint(Vector3 worldPosition)
        {
            float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
            float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            int x = Mathf.RoundToInt((_gridSizeX - 1) * percentX);
            int y = Mathf.RoundToInt((_gridSizeY - 1) * percentY);

            return new Vector2Int(x, y);
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

            if (_grid == null || drawGizmos == false)
            {
                return;
            }

            foreach (Node node in _grid)
            {
                if (node == null) continue;
                Gizmos.color = Color.Lerp(Color.white, Color.black,Mathf.InverseLerp(_penaltyMin, _penaltyMax, node.MovementPenalty));
                
                Gizmos.color = node.Walkable ? Gizmos.color : Color.red;
                Gizmos.DrawCube(node.WorldPosition, Vector3.one * _nodeDiameter);
            }
        }
    }

    [Serializable]
    public class TerrainType
    {
        public LayerMask TerrainMask;
        public int TerrainPenalty;
    }
}