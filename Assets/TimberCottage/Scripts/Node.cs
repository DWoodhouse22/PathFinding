using UnityEngine;

namespace TimberCottage.Pathfinding
{
    public class Node : IHeapItem<Node>
    {
        public bool Walkable { get; }
        public Vector3 WorldPosition { get; }
        public int GCost;
        public int HCost;
        private int FCost => GCost + HCost;
        public int GridX { get; }
        public int GridY { get; }
        public Node ParentNode;
        public int MovementPenalty;
        public int HeapIndex { get; set; }

        public Node(bool walkable, Vector3 worldPosition, int gridX, int gridY, int movementPenalty)
        {
            Walkable = walkable;
            WorldPosition = worldPosition;
            GridX = gridX;
            GridY = gridY;
            MovementPenalty = movementPenalty;
        }

        public int CompareTo(Node other)
        {
            int compare = FCost.CompareTo(other.FCost);
            if (compare == 0)
            {
                compare = HCost.CompareTo(other.HCost);
            }

            return -compare;
        }
    }
}