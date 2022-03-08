using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TimberCottage.Pathfinding
{
    public class MaterialsManager : MonoBehaviour
    {
        private readonly struct MaterialRequest
        {
            public EMaterialType MaterialType { get; }
            public Action<RawMaterial> OnMaterialFound { get; }

            public MaterialRequest(EMaterialType materialType, Action<RawMaterial> onMaterialFound)
            {
                MaterialType = materialType;
                OnMaterialFound = onMaterialFound;
            }
        }
        
        public enum EMaterialType
        {
            Undefined = 0,
            Log,
            Plank,
            Rock,
            CutStone
        }

        private List<RawMaterial> _availableMaterials;
        private Queue<MaterialRequest> _materialRequests;

        private void Awake()
        {
            _availableMaterials = new List<RawMaterial>();
            _materialRequests = new Queue<MaterialRequest>();
            
            InitMaterialsManager();
        }

        /// <summary>
        /// Called at game start to populate with materials available at start of game
        /// </summary>
        private void InitMaterialsManager()
        {
            RawMaterial[] startingMaterials = FindObjectsOfType<RawMaterial>();
            foreach (RawMaterial mat in startingMaterials)
            {
                _availableMaterials.Add(mat);
            }
            
            Debug.Log(_availableMaterials.Count);
        }

        /// <summary>
        /// Once a material is generated, add it to the global pool so it can be consumed
        /// </summary>
        /// <param name="toAdd">Material to add to global pool</param>
        public void AddMaterialToGlobalPool(RawMaterial toAdd)
        {
            _availableMaterials.Add(toAdd);
        }

        /// <summary>
        /// Request a material. When it becomes available, onMaterialFound will be called
        /// </summary>
        /// <param name="materialType">The type of material being requested</param>
        /// <param name="onMaterialFound">Callback when material has been found</param>
        public void RequestMaterial(EMaterialType materialType, Action<RawMaterial> onMaterialFound)
        {
            MaterialRequest request = new MaterialRequest(materialType, onMaterialFound);
            _materialRequests.Enqueue(request);
        }

        public RawMaterial GetMaterial(EMaterialType materialType)
        {
            var mat = _availableMaterials.Find(m => m.MaterialType == materialType);
            if (mat == null)
            {
                return null;
            }
            
            _availableMaterials.Remove(mat);
            return mat;
        }
    }
}
