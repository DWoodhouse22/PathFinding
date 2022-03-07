using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TimberCottage.Pathfinding
{
    /// <summary>
    /// Base class for all structures in the game
    /// </summary>
    public class StructureBase : MonoBehaviour
    {
        [SerializeField] private bool spawnedAtGameStart;
        [SerializeField] private Transform bottomLeft;
        [SerializeField] private Transform meshRoot;
        [SerializeField] private Transform constructedMeshRoot;
        [SerializeField] private Transform underConstructionMeshRoot;
        [SerializeField] private bool drawGizmos;
        [SerializeField] private Vector2 extents;
        
        public Vector2 Extents => extents;
        public Vector3 BottomLeft => bottomLeft.position;
        public event Action<bool> OnCollisionEvent;
        
        protected VillagerManager _villagerManager;

        private ConstructionSite _constructionSite;
        private HashSet<Collider> _collisions;
        private bool _placedInWorld;
        private readonly float _rotationStep = 30f;
        
        private void Awake()
        {
            _constructionSite = GetComponent<ConstructionSite>();
            _collisions = new HashSet<Collider>();
            _villagerManager = FindObjectOfType<VillagerManager>();
            constructedMeshRoot.gameObject.SetActive(false);
            underConstructionMeshRoot.gameObject.SetActive(true);
        }

        private void Start()
        {
            if (spawnedAtGameStart)
            {
                OnConstructed();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_placedInWorld)
            {
                return;
            }
            
            _collisions.Add(other);
            
            // TODO: Minor improvement - only fire event if not previously colliding
            OnCollisionEvent?.Invoke(true);
        }

        private void OnTriggerExit(Collider other)
        {
            if (_placedInWorld)
            {
                return;
            }
            
            _collisions.Remove(other);
            if (_collisions.Count <= 0)
            {
                OnCollisionEvent?.Invoke(false);
            }
        }

        /// <summary>
        /// Called when structure is placed into the world
        /// </summary>
        public virtual void OnPlaced()
        {
            _constructionSite.InitConstructionSite(this);
            _placedInWorld = true;
            _collisions = null;
        }
        
        /// <summary>
        /// Called when construction is complete
        /// </summary>
        public virtual void OnConstructed()
        {
            ShowConstructedMesh();
            Destroy(_constructionSite);
        }

        private void Update()
        {
            if (_placedInWorld)
            {
                return;
            }
            
            // TODO: move this logic to construction manager
            if (Input.GetKeyDown(KeyCode.R))
            {
                Rotate(1);
            }
        }

        /// <summary>
        /// Rotate this structure during construction
        /// </summary>
        /// <param name="direction">Should be -1 or +1</param>
        public void Rotate(int direction)
        {
            if (_placedInWorld)
            {
                return;
            }
            
            meshRoot.transform.rotation *= Quaternion.Euler(0f, _rotationStep * direction, 0f); 
        }

        private void ShowConstructedMesh()
        {
            underConstructionMeshRoot.gameObject.SetActive(false);
            constructedMeshRoot.gameObject.SetActive(true);
        }

        private void OnDrawGizmos()
        {
            if (drawGizmos == false)
            {
                return;
            }
            
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(transform.position, new Vector3(extents.x, 2f, extents.y));
        }
    }
}