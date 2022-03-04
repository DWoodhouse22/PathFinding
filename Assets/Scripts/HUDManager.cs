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
        [SerializeField] private VillagerManager villagerManager;
        [SerializeField] private Button buildHouseButton;
        [SerializeField] private Button allocateBuilderButton;
        [SerializeField] private Button deAllocateBuilderButton;
        [SerializeField] private Button allocateCarrierButton;
        [SerializeField] private Button deAllocateCarrierButton;
        private void Awake()
        {
            buildHouseButton.onClick.AddListener(BuildHouseButtonClicked);
            allocateBuilderButton.onClick.AddListener(villagerManager.AllocateBuilder);
            deAllocateBuilderButton.onClick.AddListener(villagerManager.DeAllocateBuilder);
            allocateCarrierButton.onClick.AddListener(villagerManager.AllocateCarrier);
            deAllocateCarrierButton.onClick.AddListener(villagerManager.DeAllocateCarrier);
        }

        private void BuildHouseButtonClicked()
        {
            constructionManager.StartPlacingStructure(ConstructionManager.EStructureType.House);
        }
    }
}
