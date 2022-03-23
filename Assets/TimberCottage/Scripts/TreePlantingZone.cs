using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TimberCottage.Pathfinding
{
    /// <summary>
    /// A zone defined for planting trees
    /// </summary>
    public class TreePlantingZone : MonoBehaviour
    {
        [SerializeField] private bool drawGizmos;
        [SerializeField] private float radius;
        [SerializeField] private TreePlantingSpot[] _allPlantingSpots;

        private List<TreePlantingSpot> _vacantPlantingSpots;

        private void Awake()
        {
            _vacantPlantingSpots = new List<TreePlantingSpot>(_allPlantingSpots);
        }

        private TreePlantingSpot ChooseRandomPlantingSpot()
        {
            if (_vacantPlantingSpots == null || _vacantPlantingSpots.Count == 0)
            {
                Debug.LogError("No available vacant planting spots");
                return null;
            }
            
            int randIndex = Random.Range(0, _vacantPlantingSpots.Count);
            TreePlantingSpot plantingSpot = _vacantPlantingSpots[randIndex];
            _vacantPlantingSpots.RemoveAt(randIndex);
            return plantingSpot;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                TreePlantingSpot plantingSpot = ChooseRandomPlantingSpot();
                if (plantingSpot != null)
                {
                    plantingSpot.PlantTree();
                    //Debug.Log($"Chose {plantingSpot.name}");
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (drawGizmos == false)
            {
                return;
            }

            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, radius);

            if (_allPlantingSpots == null || _allPlantingSpots.Length == 0)
            {
                return;
            }

            foreach (TreePlantingSpot spot in _allPlantingSpots)
            {
                spot.DrawGizmos();
            }
        }
    }
}