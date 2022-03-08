using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace TimberCottage.Pathfinding
{
    public class VillagerBuilder : VillagerBase
    {
        private VillagerBuilderBehaviour _villagerBuilderBehaviour;
        protected override void InitBehaviour()
        {
            _villagerBuilderBehaviour = gameObject.AddComponent<VillagerBuilderBehaviour>();
            _villagerBehaviour = _villagerBuilderBehaviour;
        }
    }
}
