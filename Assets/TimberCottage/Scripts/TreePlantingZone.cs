using System;
using System.Collections;
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
        private Queue<PlantingSpotRequest> _plantingSpotRequests;

        private void Awake()
        {
            _plantingSpotRequests = new Queue<PlantingSpotRequest>();
            _vacantPlantingSpots = new List<TreePlantingSpot>(_allPlantingSpots);
        }

        private TreePlantingSpot ChooseRandomPlantingSpot()
        {
            if (_vacantPlantingSpots == null || _vacantPlantingSpots.Count == 0)
            {
                return null;
            }
            
            int randIndex = Random.Range(0, _vacantPlantingSpots.Count);
            TreePlantingSpot plantingSpot = _vacantPlantingSpots[randIndex];
            _vacantPlantingSpots.RemoveAt(randIndex);
            return plantingSpot;
        }

        private void Update()
        {
            if (_plantingSpotRequests.Count == 0)
            {
                return;
            }

            PlantingSpotRequest request = _plantingSpotRequests.Peek();
            TreePlantingSpot spot = ChooseRandomPlantingSpot();
            if (spot == null)
            {
                return;
            }
            
            _plantingSpotRequests.Dequeue();
            request.Callback(spot);
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

        /// <summary>
        /// Request a vacant planting spot
        /// </summary>
        /// <param name="onFoundPlantingSpot"></param>
        public void RequestPlantingSpot(Action<TreePlantingSpot> onFoundPlantingSpot)
        {
            PlantingSpotRequest request = new PlantingSpotRequest(onFoundPlantingSpot);
            _plantingSpotRequests.Enqueue(request);
        }

        private readonly struct PlantingSpotRequest
        {
            public Action<TreePlantingSpot> Callback { get; }

            public PlantingSpotRequest(Action<TreePlantingSpot> callback)
            {
                Callback = callback;
            }
        }
    }
}