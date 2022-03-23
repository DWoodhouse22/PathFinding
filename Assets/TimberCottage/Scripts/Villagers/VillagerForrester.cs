using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TimberCottage.Pathfinding
{
    /// <summary>
    /// Forrester villager type. Works in a forrester hut and plants trees
    /// TODO: Bulk of logic to be moved to forrester behaviour script
    /// </summary>
    public class VillagerForrester : VillagerBase
    {
        private VillagerForresterBehaviour _villagerForresterBehaviour;
        private StructureForrester _assignedForresterHut;
        private IEnumerator _plantingRoutine;
        private readonly float _delayBetweenPlanting = 5f;
        private TreePlantingSpot _currentPlantingSpot;

        protected override void InitBehaviour()
        {
            _villagerForresterBehaviour = gameObject.AddComponent<VillagerForresterBehaviour>();
            _villagerBehaviour = _villagerForresterBehaviour;
        }

        public void AssignToHut(StructureForrester forresterHut)
        {
            _assignedForresterHut = forresterHut;
            FindPathToHut();
        }

        private void StartPlantingTree()
        {
            if (_plantingRoutine != null)
            {
                Debug.LogError("Already planting a tree");
                return;
            }
            
            _plantingRoutine = FindPlantingSpotCoroutine();
            StartCoroutine(_plantingRoutine);
        }
        
        private IEnumerator FindPlantingSpotCoroutine()
        {
            yield return new WaitForSeconds(_delayBetweenPlanting);
            
            _assignedForresterHut.PlantingZone.RequestPlantingSpot(OnFoundPlantingSpot);
        }

        private void OnFoundPlantingSpot(TreePlantingSpot plantingSpot)
        {
            _currentPlantingSpot = plantingSpot;
            _pathRequestManager.RequestPath(transform.position, _currentPlantingSpot.transform.position, OnFoundPathToPlantingSpot);
        }

        private void OnFoundPathToPlantingSpot(Vector3[] path, bool success)
        {
            OnPathFound(path, success, OnReachedPlantingSpot);
        }

        private void OnReachedPlantingSpot()
        {
            // Find path back to the hut before planting the tree to avoid villager now standing on a blocked route
            FindPathToHut();
        }

        private void FindPathToHut()
        {
            _pathRequestManager.RequestPath(transform.position, _assignedForresterHut.StructureEntrance.position, OnFoundPathToHut);
        }

        private void OnFoundPathToHut(Vector3[] path, bool success)
        {
            OnPathFound(path, success, OnReachedHut);

            if (_currentPlantingSpot == null)
            {
                return;
            }
            _currentPlantingSpot.PlantTree();
            _currentPlantingSpot = null;
        }

        private void OnReachedHut()
        {
            _plantingRoutine = null;
            StartPlantingTree();
        }
    }
}