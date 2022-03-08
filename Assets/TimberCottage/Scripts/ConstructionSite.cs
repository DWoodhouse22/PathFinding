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
        private List<RawMaterial> _deliveredMaterials;
        private List<MaterialsManager.EMaterialType> _consumedMaterials;
        private VillagerManager _villagerManager;
        private MaterialsManager _materialsManager;

        private int _totalNumRequiredMaterials
        {
            get
            {
                int requiredMaterials = 0;
                foreach (ConstructionCost cost in constructionCosts)
                {
                    requiredMaterials += cost.NumMaterial;
                }

                return requiredMaterials;
            }
        }
        
        public ConstructionCost[] ConstructionCosts => constructionCosts;

        private void Awake()
        {
            _materialsManager = FindObjectOfType<MaterialsManager>();
            _villagerManager = FindObjectOfType<VillagerManager>();
        }

        public void InitConstructionSite(StructureBase toConstruct)
        {
            _requiredMaterials = new List<MaterialsManager.EMaterialType>();
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
            
            Debug.Log($"Assigned carrier {carrier.name}");
            // Give carrier job to collect a material and return to the construction site

            StartCoroutine(DebugReceiveConstructionMaterials(carrier, materialToGather));
        }

        private IEnumerator DebugReceiveConstructionMaterials(VillagerCarrier carrier, MaterialsManager.EMaterialType materialType)
        {
            yield return new WaitForSeconds(1f);
            var mat = _materialsManager.GetMaterial(materialType);
            if (mat == null)
            {
                Debug.LogError($"Cannot allocate {materialType}");
                yield break;
            }
            ReceiveConstructionMaterial(mat, carrier);
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
            _villagerManager.ReturnVillagerToThePool(courier);
            _deliveredMaterials.Add(material);
            Debug.Log($"Received material {material.MaterialType}");
            if (_deliveredMaterials.Count == _totalNumRequiredMaterials)
            {
                Debug.Log("All materials received, starting construction");
                // start construction (builders start hammering etc...)
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

            // consumed all required materials...
            if (_consumedMaterials.Count == _totalNumRequiredMaterials)
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