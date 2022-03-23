using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TimberCottage.Pathfinding
{
    public class StructureForrester : StructureBase
    {
        [SerializeField] private TreePlantingZone plantingZone;
        [SerializeField] private Transform structureEntrance;

        private VillagerForrester _assignedVillager;

        public TreePlantingZone PlantingZone => plantingZone;
        public Transform StructureEntrance => structureEntrance;
        
        public override void OnConstructed()
        {
            base.OnConstructed();
            _villagerManager.RequestVillager(VillagerManager.EVillagerType.Base, OnFoundVillager);
        }

        private void OnFoundVillager(VillagerBase villager)
        {
            _assignedVillager = _villagerManager.ConvertVillagerTo<VillagerForrester>(villager, VillagerManager.EVillagerType.Forrester);
            if (_assignedVillager == null)
            {
                return;
            }

            _assignedVillager.transform.parent = transform;
            _assignedVillager.AssignToHut(this);
        }
    }
}