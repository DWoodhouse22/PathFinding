using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TimberCottage.Pathfinding
{
    public class StructureBase : MonoBehaviour
    {
        [SerializeField] private Transform bottomLeft;
        public Vector3 BottomLeft => bottomLeft.position;
        public Vector3 Extents => _boxCollider.size;
        public event Action<bool> OnCollisionEvent;

        private HashSet<Collider> _collisions;
        private bool _constructed = false;
        private BoxCollider _boxCollider;
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

        public void OnConstructed()
        {
            Debug.Log($"{name} constructed");
            _constructed = true;
            _collisions = null;
        }
    }
}