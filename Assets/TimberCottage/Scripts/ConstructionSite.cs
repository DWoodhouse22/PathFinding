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

        private StructureBase _structureToConstruct;
        private List<MaterialsManager.EMaterialType> _requiredMaterials;
        private List<MaterialsManager.EMaterialType> _deliveredMaterials;
        private List<MaterialsManager.EMaterialType> _consumedMaterials;
        private VillagerManager _villagerManager;
        
        public ConstructionCost[] ConstructionCosts => constructionCosts;

        private void Awake()
        {
            _villagerManager = FindObjectOfType<VillagerManager>();
        }

        public void InitConstructionSite(StructureBase toConstruct)
        {
            _requiredMaterials = new List<MaterialsManager.EMaterialType>();
            _deliveredMaterials = new List<MaterialsManager.EMaterialType>();
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
            
            Debug.Log($"Assigned carrier {carrier.name}");
            carrier.AssignJob();
        }

        public void OnConstructionComplete()
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
            _deliveredMaterials.Add(material.MaterialType);
            _requiredMaterials.Remove(material.MaterialType);
            
            courier.UnAssignJob();
            if (_deliveredMaterials.Count == constructionCosts.Length)
            {
                // start construction (builders start hammering etc...)
            }
        }

        /// <summary>
        /// Called when a material has been consumed (used for the construction)
        /// </summary>
        /// <param name="material">Material which has been consumed</param>
        public void ConsumeConstructionMaterial(RawMaterial material)
        {
            _consumedMaterials.Add(material.MaterialType);

            // consumed all required materials...
            if (_consumedMaterials.Count == constructionCosts.Length)
            {
                _structureToConstruct.OnConstructed();
            }
        }

        /// <summary>
        /// Automatically handle the construction
        /// </summary>
        /// <returns></returns>
        private IEnumerator DebugDoConstruction()
        {
            yield return null;
        }
    }
}