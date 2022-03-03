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
        private Dictionary<eStructureType, StructureBase> _structureTypeToGameObject;

        public enum eStructureType
        {
            Undefined = 0,
            House
        }

        private void Awake()
        {
            _structureTypeToGameObject = new Dictionary<eStructureType, StructureBase>
            {
                { eStructureType.House, structureHousePrefab }
            };
        }

        public void StartPlacingStructure(eStructureType structureType)
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
            
            while (structureBuilt == false)
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 1000f,groundLayer))
                {
                    if (firstRayHit == false)
                    {
                        toSpawn = Instantiate(structure, hit.point, Quaternion.identity);
                        firstRayHit = true;
                    }

                    if (toSpawn != null)
                    {
                        toSpawn.transform.position = hit.point;

                        // TODO: investigate why MouseButtonUp/Down doesn't register
                        if (Input.GetMouseButton(0))
                        {
                            structureBuilt = true;
                        }
                    }
                }

                yield return new WaitForEndOfFrame();
            }
            
            // TODO: only calculate the grid surrounding the new structure
            pathFindingGrid.CalculateNodes(Vector2Int.zero, new Vector2Int(pathFindingGrid.GridSizeX, pathFindingGrid.GridSizeY));
            _placeStructureRoutine = null;
        }
    }
}