using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TimberCottage.Pathfinding
{
    public class VillagerCarrier : VillagerBase
    {
        private VillagerCarrierBehaviour _villagerCarrierBehaviour;
        
        protected override void InitBehaviour()
        {
            _villagerCarrierBehaviour = gameObject.AddComponent<VillagerCarrierBehaviour>();
            _villagerBehaviour = _villagerCarrierBehaviour;
        }
    }
}
