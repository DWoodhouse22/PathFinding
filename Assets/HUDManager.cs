using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TimberCottage.Pathfinding
{
    public class HUDManager : MonoBehaviour
    {
        [SerializeField] private PathFindingGrid pathFindingGrid;
        [SerializeField] private Button buildHouseButton;
        [SerializeField] private House housePrefab;

        private void Awake()
        {
            if (pathFindingGrid == null)
            {
                pathFindingGrid = FindObjectOfType<PathFindingGrid>();
            }
            buildHouseButton.onClick.AddListener(BuildHouseButtonClicked);
        }

        private void BuildHouseButtonClicked()
        {
            House house = GameObject.Instantiate(housePrefab, Vector3.zero, Quaternion.identity);
            pathFindingGrid.CalculateNodes(Vector2Int.zero, new Vector2Int(100,100));
        }
    }
}
