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
        private RawMaterial _carriedMaterial;
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
            _materialsManager.RequestMaterial(materialType, OnFoundMaterial);
        }

        private void OnFoundMaterial(RawMaterial material)
        {
            _pathRequestManager.RequestPath(transform.position, material.transform.position, OnFoundPathToMaterial);
            _carriedMaterial = material;
        }

        private void OnFoundPathToMaterial(Vector3[] pathWaypoints, bool success)
        {
            OnPathFound(pathWaypoints, success, () =>
            {
                _pathRequestManager.RequestPath(transform.position, _assignedConstructionSite.MaterialDropOffPoint, OnFoundPathToConstructionSite);
            });
        }

        private void OnFoundPathToConstructionSite(Vector3[] pathWayPoints, bool success)
        {
            OnPathFound(pathWayPoints, success, OnReachedConstructionSite);
        }

        private void OnReachedConstructionSite()
        {
            _assignedConstructionSite.ReceiveConstructionMaterial(_carriedMaterial, this);
            _carriedMaterial = null;
        }
    }
}
