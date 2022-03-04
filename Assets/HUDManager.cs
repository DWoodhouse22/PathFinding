using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TimberCottage.Pathfinding
{
    public class HUDManager : MonoBehaviour
    {
        [SerializeField] private ConstructionManager constructionManager;
        [SerializeField] private Button buildHouseButton;

        private void Awake()
        {
            buildHouseButton.onClick.AddListener(BuildHouseButtonClicked);
        }

        private void BuildHouseButtonClicked()
        {
            constructionManager.StartPlacingStructure(ConstructionManager.EStructureType.House);
        }
    }
}
