using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace TimberCottage.Pathfinding
{
    public class ConstructionManager : MonoBehaviour
    {
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private StructureHouse structureHousePrefab;
        [SerializeField] private PathFindingGrid pathFindingGrid;
        [SerializeField] private Transform underConstructionRootTransform;
        [SerializeField] private Transform constructedRootTransform;
        
        private IEnumerator _placeStructureRoutine;
        private Dictionary<EStructureType, StructureBase> _structureTypeToGameObject;
        private bool _canStructureBePlaced;

        public enum EStructureType
        {
            Undefined = 0,
            House
        }

        private void Awake()
        {
            _structureTypeToGameObject = new Dictionary<EStructureType, StructureBase>
            {
                { EStructureType.House, structureHousePrefab }
            };
        }

        /// <summary>
        /// Start placing structure in the world
        /// </summary>
        /// <param name="structureType">Type of structure which is being placed</param>
        public void StartPlacingStructure(EStructureType structureType)
        {
            if (_structureTypeToGameObject.TryGetValue(structureType, out StructureBase structure) == false)
            {
                return;
            }

            if (_placeStructureRoutine != null)
            {
                return;
            }

            _placeStructureRoutine = PlaceStructureCoroutine(structure);
            StartCoroutine(_placeStructureRoutine);
        }

        private IEnumerator PlaceStructureCoroutine(StructureBase structure)
        {
            bool firstRayHit = false;
            bool structurePlaced = false;
            Camera cam = Camera.main;
            StructureBase toSpawn = null;
            _canStructureBePlaced = true;
            
            while (structurePlaced == false)
            {
                yield return new WaitForEndOfFrame();
                
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 1000f,groundLayer) == false)
                {
                    continue;
                }
                
                if (firstRayHit == false)
                {
                    toSpawn = Instantiate(structure, hit.point, Quaternion.identity);
                    toSpawn.OnCollisionEvent += OnStructureCollisionEvent;
                    firstRayHit = true;
                }

                if (toSpawn == null)
                {
                    continue;
                }
                    
                toSpawn.transform.position = hit.point;
                // TODO: investigate why MouseButtonUp/Down doesn't register here
                if (_canStructureBePlaced && Input.GetMouseButton(0))
                {
                    structurePlaced = true;
                    toSpawn.transform.SetParent(underConstructionRootTransform);
                }
            }

            toSpawn.OnCollisionEvent -= OnStructureCollisionEvent;
            toSpawn.OnPlaced();

            Vector2Int maxSize = new Vector2Int(pathFindingGrid.GridSizeX, pathFindingGrid.GridSizeY);
            pathFindingGrid.CalculateNodes(Vector2Int.zero, maxSize);
            _placeStructureRoutine = null;
        }

        private void OnStructureCollisionEvent(bool colliding)
        {
            _canStructureBePlaced = !colliding;
        }
    }
}