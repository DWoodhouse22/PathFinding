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
            bool structureBuilt = false;
            Camera cam = Camera.main;
            StructureBase toSpawn = null;
            _canStructureBePlaced = true;
            
            while (structureBuilt == false)
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
                    structureBuilt = true;
                }
            }

            toSpawn.OnCollisionEvent -= OnStructureCollisionEvent;
            toSpawn.OnConstructed();
            
            Vector2Int originGridPosition = pathFindingGrid.GridPositionFromWorldPoint(toSpawn.BottomLeft);
            Vector2Int size = new Vector2Int(Mathf.RoundToInt(toSpawn.Extents.x), Mathf.RoundToInt(toSpawn.Extents.z));
            pathFindingGrid.CalculateNodes(originGridPosition, size);
            _placeStructureRoutine = null;
        }

        private void OnStructureCollisionEvent(bool colliding)
        {
            _canStructureBePlaced = !colliding;
        }
    }
}