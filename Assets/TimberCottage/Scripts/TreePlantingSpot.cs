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
        [SerializeField] private ForresterTree treePrefab;

        private float _randomRange => radius * .5f;
        private PathFindingGrid _pathFindingGrid;
        private ForresterTree _tree;

        public bool CanTreeBeGathered { private set; get; }

        private void Start()
        {
            CanTreeBeGathered = false;
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
            _tree = Instantiate(treePrefab, GetPlantingLocation(), Quaternion.identity, transform);
            _pathFindingGrid.CalculateNodes();
            _tree.OnTreeGrown += TreeGrown;
        }

        public void DrawGizmos()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        private void TreeGrown()
        {
            Debug.Log("Tree is grown!");
            _tree.OnTreeGrown -= TreeGrown;
            CanTreeBeGathered = true;
        }

        private void OnTreeGathered()
        {
            CanTreeBeGathered = false;
        }
    }
}
