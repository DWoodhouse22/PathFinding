using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TimberCottage.Pathfinding
{
    public class StructureForrester : StructureBase
    {
        [SerializeField] private TreePlantingZone plantingZone;

        private VillagerForrester _assignedVillager;

        public override void OnConstructed()
        {
            _villagerManager.RequestVillager(VillagerManager.EVillagerType.Base, OnFoundVillager);
        }

        private void OnFoundVillager(VillagerBase villager)
        {
            VillagerForrester forrester = _villagerManager.ConvertVillagerTo<VillagerForrester>(villager, VillagerManager.EVillagerType.Forrester);
            forrester.transform.parent = transform;
        }
    }
}