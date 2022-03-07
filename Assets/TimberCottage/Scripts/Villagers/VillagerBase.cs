using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace TimberCottage.Pathfinding
{
    public class VillagerBase : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float speed = 20;
        [SerializeField] private float turnSpeed = 3;
        [SerializeField] private float turnDistance = 5;
        [SerializeField] private float stoppingDistance = 10;
        
        private PathRequestManager _pathRequestManager;
        private const float MinPathUpdateTime = 0.2f;
        private const float PathUpdateMoveThreshold = 0.5f;
        private Path _path;
        private IEnumerator _followPathRoutine;
        private IEnumerator _updatePathRoutine;
        private VillagerManager.EVillagerType _villagerType;
        private VillagerBaseBehaviour _villagerBehaviour;
        
        private void Awake()
        {
            _villagerBehaviour = gameObject.AddComponent<VillagerBaseBehaviour>();
            _villagerBehaviour.InitBehaviour(this, transform.position);
            _pathRequestManager = FindObjectOfType<PathRequestManager>();
        }

        private void Start()
        {
            _villagerBehaviour.StartBehaviour();
        }

        public void Init(VillagerManager.EVillagerType villagerType)
        {
            _villagerType = villagerType;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (_updatePathRoutine == null)
                {
                    _updatePathRoutine = UpdatePath();
                    StartCoroutine(_updatePathRoutine);
                }
            }
        }

        public void OnPathFound(Vector3[] waypoints, bool success)
        {
            if (!success)
            {
                return;
            }
            
            if (_followPathRoutine != null)
            {
                StopCoroutine(_followPathRoutine);
            }

            _path = new Path(waypoints, transform.position, turnDistance, stoppingDistance);
            _followPathRoutine = FollowPath();
            StartCoroutine(_followPathRoutine);
        }

        private IEnumerator UpdatePath()
        {
            Vector3 targetPosition = target.position;
            _pathRequestManager.RequestPath(transform.position, targetPosition, OnPathFound);
            const float sqrMoveThreshold = PathUpdateMoveThreshold * PathUpdateMoveThreshold;
            Vector3 targetPosOld = targetPosition;
            while (true)
            {
                yield return new WaitForSeconds(MinPathUpdateTime);
                if ((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold == false)
                {
                    continue;
                }

                targetPosition = target.position;
                _pathRequestManager.RequestPath(transform.position, targetPosition, OnPathFound);
                targetPosOld = targetPosition;
            }
        }

        private IEnumerator FollowPath()
        {
            bool followingPath = true;
            int pathIndex = 0;
            transform.LookAt(_path.LookPoints[0]);

            float speedPercent = 1;
            
            while (followingPath)
            {
                Vector3 position = transform.position;
                Vector2 pos2D = new Vector2(position.x, position.z);
                while (_path.TurnBoundaries[pathIndex].HasCrossedLine(pos2D))
                {
                    if (pathIndex == _path.FinishLineIndex)
                    {
                        followingPath = false;
                        break;
                    }

                    pathIndex++;
                }

                if (followingPath == false)
                {
                    break;
                }
                
                if (pathIndex > -_path.SlowDownIndex && stoppingDistance > 0)
                {
                    speedPercent = Mathf.Clamp01(_path.TurnBoundaries[_path.FinishLineIndex].DistanceFromPoint(pos2D) / stoppingDistance);
                    if (speedPercent < 0.01f)
                    {
                        followingPath = false;
                    }
                }
                
                Quaternion targetRotation = Quaternion.LookRotation(_path.LookPoints[pathIndex] - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                transform.Translate(Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self);
                
                yield return null;
            }
        }

        // private void OnDrawGizmos()
        // {
        //     _path?.DrawWithGizmos();
        // }
        
        public IEnumerator BehaviourRoutine { get; }
        public IEnumerator BehaviourCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
