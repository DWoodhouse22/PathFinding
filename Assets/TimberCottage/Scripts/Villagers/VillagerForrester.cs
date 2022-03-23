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
        
        protected override void InitBehaviour()
        {
            _villagerForresterBehaviour = gameObject.AddComponent<VillagerForresterBehaviour>();
            _villagerBehaviour = _villagerForresterBehaviour;
        }
    }
}