using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TimberCottage.Pathfinding
{
    [Serializable]
    public struct ConstructionCost
    {
        [SerializeField] private MaterialsManager.EMaterialType material;
        [SerializeField] private int numMaterial;

        public MaterialsManager.EMaterialType Material => material;
        public int NumMaterial => numMaterial;
    }
    
    public class ConstructionSite : MonoBehaviour
    {
        [SerializeField] private ConstructionCost[] constructionCosts;
        [SerializeField] private Transform _materialDropOffPoint;

        private StructureBase _structureToConstruct;
        private List<RawMaterial> _deliveredMaterials;
        private List<MaterialsManager.EMaterialType> _consumedMaterials;
        private VillagerManager _villagerManager;

        private int _totalNumRequiredMaterials;

        public ConstructionCost[] ConstructionCosts => constructionCosts;
        public Vector3 MaterialDropOffPoint => _materialDropOffPoint.position;

        private void Awake()
        {
            _villagerManager = FindObjectOfType<VillagerManager>();
            
            foreach (ConstructionCost cost in constructionCosts)
            {
                _totalNumRequiredMaterials += cost.NumMaterial;
            }
        }

        public void InitConstructionSite(StructureBase toConstruct)
        {
            _deliveredMaterials = new List<RawMaterial>();
            _consumedMaterials = new List<MaterialsManager.EMaterialType>();
            _structureToConstruct = toConstruct;

            foreach (ConstructionCost cost in constructionCosts)
            {
                MaterialsManager.EMaterialType materialType = cost.Material;
                for (int i = 0; i < cost.NumMaterial; i++)
                {
                    _villagerManager.RequestVillager(VillagerManager.EVillagerType.Carrier,
                        villager => { OnCarrierAssigned(villager, materialType); });
                }
            }
        }

        private void OnCarrierAssigned(VillagerBase villager, MaterialsManager.EMaterialType materialToGather)
        {
            VillagerCarrier carrier = villager as VillagerCarrier;
            if (carrier == null)
            {
                Debug.LogError($"Cannot convert {villager.name} to a carrier");
                return;
            }
            
            carrier.FetchMaterial(this, materialToGather);
        }

        private void OnConstructionComplete()
        {
            _structureToConstruct.OnConstructed();
        }
        
        /// <summary>
        /// Call when a carrier delivers a material
        /// </summary>
        /// <param name="material">Material to be delivered</param>
        /// <param name="courier">Villager which delivered the material</param>
        public void ReceiveConstructionMaterial(RawMaterial material, VillagerCarrier courier)
        {
            _villagerManager.ReturnVillagerToThePool(courier);
            _deliveredMaterials.Add(material);

            if (_deliveredMaterials.Count == _totalNumRequiredMaterials)
            {
                Debug.Log("All materials received, starting construction");
                // start construction (builders start hammering etc...)
                // automatically complete construction for now
                foreach (RawMaterial mat in _deliveredMaterials)
                {
                    ConsumeConstructionMaterial(mat);
                }
            }
        }

        /// <summary>
        /// Called when a material has been consumed (used for the construction)
        /// </summary>
        /// <param name="material">Material which has been consumed</param>
        public void ConsumeConstructionMaterial(RawMaterial material)
        {
            _consumedMaterials.Add(material.MaterialType);

            if (_consumedMaterials.Count == _totalNumRequiredMaterials)
            {
                OnConstructionComplete();
            }
        }
    }
}