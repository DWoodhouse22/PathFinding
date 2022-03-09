using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace TimberCottage.Pathfinding
{
    public class VillagerCarrier : VillagerBase
    {
        private VillagerCarrierBehaviour _villagerCarrierBehaviour;
        private MaterialsManager _materialsManager;

        private ConstructionSite _assignedConstructionSite;
        
        protected override void Awake()
        {
            base.Awake();

            _materialsManager = FindObjectOfType<MaterialsManager>();
        }

        protected override void InitBehaviour()
        {
            _villagerCarrierBehaviour = gameObject.AddComponent<VillagerCarrierBehaviour>();
            _villagerBehaviour = _villagerCarrierBehaviour;
        }

        public void FetchMaterial(ConstructionSite constructionSite, MaterialsManager.EMaterialType materialType)
        {
            _assignedConstructionSite = constructionSite;
            _materialsManager.RequestMaterial(materialType, OnMaterialAvailable);
        }

        private void OnMaterialAvailable(RawMaterial material)
        {
            _assignedConstructionSite.ReceiveConstructionMaterial(material, this);
        }
    }
}
