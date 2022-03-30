using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TimberCottage.Pathfinding
{
    public class WoodCuttingRegion : MonoBehaviour
    {
        [SerializeField] private float radius = 5f;
        [SerializeField] private LayerMask treePlantingSpotLayerMask;

        private List<TreePlantingSpot> plantingSpots;

        public void Initialise()
        {
            plantingSpots = FindTreePlantingSpotsWithinRange();
            Debug.Log(plantingSpots.Count);
        }

        private List<TreePlantingSpot> FindTreePlantingSpotsWithinRange()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius, treePlantingSpotLayerMask);

            List<TreePlantingSpot> spots = new List<TreePlantingSpot>();
            foreach (Collider collider in colliders)
            {
                var spot = collider.GetComponent<TreePlantingSpot>();
                if (spot == null)
                {
                    continue;
                }

                spots.Add(spot);
            }

            return spots;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}