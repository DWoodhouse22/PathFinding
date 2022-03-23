using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TimberCottage.Pathfinding
{
    /// <summary>
    /// An individual spot within a zone for planting trees
    /// </summary>
    public class TreePlantingSpot : MonoBehaviour
    {
        [SerializeField] private float radius;
        [SerializeField] private Tree treePrefab;
        private float _randomRange => radius * .5f;
        private PathFindingGrid _pathFindingGrid;

        private void Start()
        {
            _pathFindingGrid = FindObjectOfType<PathFindingGrid>();
        }

        private Vector3 GetPlantingLocation()
        {
            Vector2 deviation = Random.insideUnitCircle * _randomRange;
            return transform.position + new Vector3(deviation.x, 0, deviation.y);
        }

        /// <summary>
        /// Plants a tree in this spot. Gives some random deviation to the location with this spot's bounds
        /// </summary>
        public void PlantTree()
        {
            Instantiate(treePrefab, GetPlantingLocation(), Quaternion.identity, transform);
            _pathFindingGrid.CalculateNodes();
        }

        public void DrawGizmos()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
