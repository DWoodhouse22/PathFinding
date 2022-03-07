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
    }
    
    public class ConstructionSite : MonoBehaviour
    {
        [SerializeField] private ConstructionCost[] constructionCosts;

        private StructureBase _structureToConstruct;
        private List<MaterialsManager.EMaterialType> _requiredMaterials;
        private List<MaterialsManager.EMaterialType> _deliveredMaterials;
        private List<MaterialsManager.EMaterialType> _consumedMaterials;
        
        public ConstructionCost[] ConstructionCosts => constructionCosts;

        public void InitConstructionSite(StructureBase toConstruct)
        {
            _requiredMaterials = new List<MaterialsManager.EMaterialType>();
            _deliveredMaterials = new List<MaterialsManager.EMaterialType>();
            _consumedMaterials = new List<MaterialsManager.EMaterialType>();
            _structureToConstruct = toConstruct;
        }

        public void OnConstructionComplete()
        {
            _structureToConstruct.OnConstructed();
        }
        
        /// <summary>
        /// Call when a carrier delivers a material
        /// </summary>
        /// <param name="material">Material to be delivered</param>
        public void ReceiveConstructionMaterial(RawMaterial material)
        {
            _deliveredMaterials.Add(material.MaterialType);
            _requiredMaterials.Remove(material.MaterialType);
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