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
        [SerializeField] private Transform bottomLeft;
        [SerializeField] private Transform structureMesh;
        [SerializeField] private bool drawGizmos;
        [SerializeField] private Vector2 extents;
        
        public Vector2 Extents => extents;
        public Vector3 BottomLeft => bottomLeft.position;
        public event Action<bool> OnCollisionEvent;

        protected VillagerManager _villagerManager;
        
        private HashSet<Collider> _collisions;
        private bool _constructed;
        private readonly float _rotationStep = 30f;
        
        private void Awake()
        {
            _collisions = new HashSet<Collider>();
            _villagerManager = FindObjectOfType<VillagerManager>();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (_constructed)
            {
                return;
            }
            
            _collisions.Add(other);
            
            // TODO: Minor improvement - only fire event if not previously colliding
            OnCollisionEvent?.Invoke(true);
        }

        private void OnTriggerExit(Collider other)
        {
            if (_constructed)
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
        public virtual void OnConstructed()
        {
            _constructed = true;
            _collisions = null;
        }

        private void Update()
        {
            if (_constructed)
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
            if (_constructed)
            {
                return;
            }
            
            structureMesh.transform.rotation *= Quaternion.Euler(0f, _rotationStep * direction, 0f); 
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