using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TimberCottage.Pathfinding
{
    public class Path
    {
        public readonly Vector3[] LookPoints;
        public readonly Line[] TurnBoundaries;
        public readonly int FinishLineIndex;
        public readonly int SlowDownIndex;
        
        public Path(Vector3[] waypoints, Vector3 startPos, float turnDistance, float stoppingDistance)
        {
            LookPoints = waypoints;
            TurnBoundaries = new Line[LookPoints.Length];
            FinishLineIndex = TurnBoundaries.Length - 1;

            Vector2 previousPoint = V3toV2(startPos);
            for (int i = 0; i < LookPoints.Length; i++)
            {
                Vector2 currentPoint = V3toV2(LookPoints[i]);
                Vector2 dirToCurrentPoint = (currentPoint - previousPoint).normalized;
                Vector2 turnBoundaryPoint = i == FinishLineIndex ? currentPoint : currentPoint - dirToCurrentPoint * turnDistance;
                TurnBoundaries[i] = new Line(turnBoundaryPoint, previousPoint - dirToCurrentPoint * turnDistance);
                previousPoint = turnBoundaryPoint;
            }

            float distanceFromEndPoint = 0;
            for (int i = LookPoints.Length - 1; i > 0; i--)
            {
                distanceFromEndPoint += Vector3.Distance(LookPoints[i], LookPoints[i - 1]);
                if (distanceFromEndPoint > stoppingDistance)
                {
                    SlowDownIndex = i;
                    break;
                }
            }
        }

        Vector2 V3toV2(Vector3 v3)
        {
            return new Vector2(v3.x, v3.z);
        }

        public void DrawWithGizmos()
        {
            Gizmos.color = Color.black;
            foreach (Vector3 p in LookPoints)
            {
                Gizmos.DrawCube(p + Vector3.up, Vector3.one);
            }
            
            Gizmos.color = Color.white;
            foreach (Line l in TurnBoundaries)
            {
                l.DrawWithGizmos(10);
            }
        }
    }
}
