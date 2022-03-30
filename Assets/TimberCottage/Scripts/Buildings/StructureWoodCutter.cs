using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TimberCottage.Pathfinding
{
    public class StructureWoodCutter : StructureBase
    {
        [SerializeField] private WoodCuttingRegion woodCuttingRegion;

        public override void OnConstructed()
        {
            base.OnConstructed();

            woodCuttingRegion.Initialise();
        }
    }
}