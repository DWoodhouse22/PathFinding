using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TimberCottage.Pathfinding
{
    public class StructureForrester : StructureBase
    {
        [SerializeField] private TreePlantingZone plantingZone;

        public override void OnConstructed()
        {
            _villagerManager.RequestVillager(VillagerManager.EVillagerType.Base, OnFoundVillager);
        }

        private void OnFoundVillager(VillagerBase villager)
        {
            Debug.Log($"Allocated {villager.name}");
        }
    }
}