using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
        }

        private List<TreePlantingSpot> FindTreePlantingSpotsWithinRange()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius, treePlantingSpotLayerMask);
            List<TreePlantingSpot> spots = new List<TreePlantingSpot>();
            foreach (Collider collider in colliders)
            {
                TreePlantingSpot spot = collider.GetComponent<TreePlantingSpot>();
                if (spot == null)
                {
                    continue;
                }

                spots.Add(spot);
            }

            return spots;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                GetRandomValidTreePlantingSpot();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        public TreePlantingSpot GetRandomValidTreePlantingSpot()
        {
            if (plantingSpots == null || plantingSpots.Count == 0)
            {
                return null;
            }

            List<TreePlantingSpot> validSpots = plantingSpots.Where(x => x.CanTreeBeGathered).ToList();
            if (validSpots.Count == 0)
            {
                return null;
            }

            int randIndex = Random.Range(0, validSpots.Count);
            return validSpots[randIndex];
        }
    }
}