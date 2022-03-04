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
        
        public Vector3 BottomLeft => bottomLeft.position;
        public Vector3 Extents => _boxCollider.size;
        public event Action<bool> OnCollisionEvent;

        private HashSet<Collider> _collisions;
        private bool _constructed;
        private BoxCollider _boxCollider;
        private readonly float _rotationStep = 30f;
        
        private void Awake()
        {
            _collisions = new HashSet<Collider>();
            _boxCollider = GetComponent<BoxCollider>();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (_constructed)
            {
                return;
            }
            
            _collisions.Add(other);
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
    }
}