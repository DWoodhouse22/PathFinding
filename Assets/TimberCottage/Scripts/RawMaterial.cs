using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TimberCottage.Pathfinding
{
    /// <summary>
    /// Base class for all raw materials (Logs, Stones etc.)
    /// </summary>
    public class RawMaterial : MonoBehaviour
    {
        [SerializeField] protected MaterialsManager.EMaterialType _materialType;

        public MaterialsManager.EMaterialType MaterialType => _materialType;
    }
}