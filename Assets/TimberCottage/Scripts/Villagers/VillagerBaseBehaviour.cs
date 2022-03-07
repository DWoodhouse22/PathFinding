using System.Collections;
using System.Runtime.Versioning;
using UnityEngine;

namespace TimberCottage.Pathfinding
{
    public class VillagerBaseBehaviour : MonoBehaviour, IVillagerBehaviour
    {
        public IEnumerator BehaviourRoutine { set; get; }

        private Vector2 _spawnPosition;
        private readonly float _moveDelayTime = 2f;
        private PathRequestManager _pathRequestManager;
        private VillagerBase _villagerBase;
        
        public void InitBehaviour(VillagerBase villagerBase, Vector3 spawnPosition)
        {
            _pathRequestManager = FindObjectOfType<PathRequestManager>();
            _villagerBase = villagerBase;
            _spawnPosition = new Vector2(spawnPosition.x, spawnPosition.z);
        }
        
        public void StartBehaviour()
        {
            if (BehaviourRoutine != null)
            {
                return;
            }

            BehaviourRoutine = BehaviourCoroutine();
            StartCoroutine(BehaviourRoutine);
        }

        public void StopBehaviour()
        {
            if (BehaviourRoutine == null)
            {
                return;
            }
            
            StopCoroutine(BehaviourRoutine);
        }
        
        public IEnumerator BehaviourCoroutine()
        {
            while (true)
            {
                Vector2 randPosition = _spawnPosition + Random.insideUnitCircle * 10;
                _pathRequestManager.RequestPath(transform.position, new Vector3(randPosition.x, 0f, randPosition.y),
                    _villagerBase.OnPathFound);
                yield return new WaitForSeconds(_moveDelayTime);
            }
        }
    }
}