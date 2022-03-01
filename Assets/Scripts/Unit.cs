using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace TimberCottage.Pathfinding
{
    public class Unit : MonoBehaviour
    {
        private const float MIN_PATH_UPDATE_TIME = 0.2f;
        private const float PATH_UPDATE_MOVE_THRESHOLD = 0.5f;
        public Transform Target;
        public float speed = 20;
        public float turnSpeed = 3;
        public float turnDistance = 5;
        public float stoppingDistance = 10;
        private Path path;
        private IEnumerator _followPathRoutine = null;

        private void Start()
        {
            StartCoroutine(UpdatePath());
        }

        void OnPathFound(Vector3[] waypoints, bool success)
        {
            if (success)
            {
                if (_followPathRoutine != null)
                {
                    StopCoroutine(_followPathRoutine);
                }

                path = new Path(waypoints, transform.position, turnDistance, stoppingDistance);
                _followPathRoutine = FollowPath();
                StartCoroutine(_followPathRoutine);
            }
        }

        IEnumerator UpdatePath()
        {
            if (Time.timeSinceLevelLoad < 0.3f)
            {
                yield return new WaitForSeconds(.3f);
            }

            PathRequestManager.RequestPath(transform.position, Target.position, OnPathFound);
            float sqrMoveThreshold = PATH_UPDATE_MOVE_THRESHOLD * PATH_UPDATE_MOVE_THRESHOLD;
            Vector3 targetPosOld = Target.position;
            while (true)
            {
                yield return new WaitForSeconds(MIN_PATH_UPDATE_TIME);
                if ((Target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
                {
                    PathRequestManager.RequestPath(transform.position, Target.position, OnPathFound);
                    targetPosOld = Target.position;
                }
            }
        }

        IEnumerator FollowPath()
        {
            bool followingPath = true;
            int pathIndex = 0;
            transform.LookAt(path.LookPoints[0]);

            float speedPercent = 1;
            
            while (followingPath)
            {
                Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);
                while (path.TurnBoundaries[pathIndex].HasCrossedLine(pos2D))
                {
                    if (pathIndex == path.FinishLineIndex)
                    {
                        followingPath = false;
                        break;
                    }

                    pathIndex++;
                }

                if (followingPath)
                {
                    if (pathIndex > -path.SlowDownIndex && stoppingDistance > 0)
                    {
                        speedPercent = Mathf.Clamp01(path.TurnBoundaries[path.FinishLineIndex].DistanceFromPoint(pos2D) / stoppingDistance);
                        if (speedPercent < 0.01f)
                        {
                            followingPath = false;
                        }
                    }
                    Quaternion targetRotation =
                        Quaternion.LookRotation(path.LookPoints[pathIndex] - transform.position);
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                    transform.Translate(Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self);
                }
                yield return null;
            }
        }

        private void OnDrawGizmos()
        {
            if (path != null)
            {
                path.DrawWithGizmos();
            }
        }
    }
}
