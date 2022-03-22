using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TimberCottage.Pathfinding
{
    /// <summary>
    /// Class to the global pool of materials
    /// </summary>
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

        private void Update()
        {
            // This is fine in update for now.. should probably be a coroutine?
            if (_materialRequests.Count == 0)
            {
                return;
            }

            MaterialRequest request = _materialRequests.Peek();
            switch (request.MaterialType)
            {
                case EMaterialType.Plank:
                    RequestMaterial<Plank>(request);
                    break;
                case EMaterialType.Log:
                    RequestMaterial<Log>(request);
                    break;
                case EMaterialType.Rock:
                    RequestMaterial<Rock>(request);
                    break;
                case EMaterialType.CutStone:
                    RequestMaterial<CutStone>(request);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Processes a MaterialRequest
        /// </summary>
        /// <param name="request">struct containing data for this request</param>
        /// <typeparam name="T">Type of material to request</typeparam>
        private void RequestMaterial<T>(MaterialRequest request) where T : RawMaterial
        {
            Debug.Log($"Requesting {request.MaterialType}...");
            RawMaterial mat = _availableMaterials.Find(m => m.MaterialType == request.MaterialType);
            if (mat == null)
            {
                return;
            }

            T material = mat as T;
            if (material == null)
            {
                Debug.LogError("material is null");
                return;
            }

            _availableMaterials.Remove(material);
            _materialRequests.Dequeue();
            request.OnMaterialFound(material);
        }
    }
}
