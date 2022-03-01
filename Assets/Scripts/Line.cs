using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TimberCottage.Pathfinding
{
    public struct Line
    {
        private const float VERTICAL_LINE_GRADIENT = 1e5f;
        
        private float Gradient;
        private float YIntercept;
        private Vector2 PointOnLine_1;
        private Vector2 PointOnLine_2;
        private float GradientPerpendicular;

        private bool approachSide;
        public Line(Vector2 pointOnLine, Vector2 pointPerpendicularToLine)
        {
            float deltaX = pointOnLine.x - pointPerpendicularToLine.x;
            float deltaY = pointOnLine.y - pointPerpendicularToLine.y;

            if (deltaX == 0)
            {
                GradientPerpendicular = VERTICAL_LINE_GRADIENT;
            }
            else
            {
                GradientPerpendicular = deltaY / deltaX;
            }

            if (GradientPerpendicular == 0)
            {
                Gradient = VERTICAL_LINE_GRADIENT;
            }
            else
            {
                Gradient = -1 / GradientPerpendicular;
            }

            YIntercept = pointOnLine.y - Gradient * pointOnLine.x;
            PointOnLine_1 = pointOnLine;
            PointOnLine_2 = pointOnLine + new Vector2(1, Gradient);
            
            approachSide = false;
            approachSide = GetSide(pointPerpendicularToLine);
        }

        bool GetSide(Vector2 point)
        {
            return (point.x - PointOnLine_1.x) * (PointOnLine_2.y - PointOnLine_1.y) >
                   (point.y - PointOnLine_1.y) * (PointOnLine_2.x - PointOnLine_1.x);
        }

        public bool HasCrossedLine(Vector2 point)
        {
            return GetSide(point) != approachSide;
        }

        public float DistanceFromPoint(Vector2 point)
        {
            float yInterceptPerpendicular = point.y - GradientPerpendicular * point.x;
            float intersectX = (yInterceptPerpendicular - YIntercept) / (Gradient - GradientPerpendicular);
            float intersectY = Gradient * intersectX + YIntercept;
            return Vector2.Distance(point, new Vector2(intersectX, intersectY));
        }

        public void DrawWithGizmos(float length)
        {
            Vector3 lineDirection = new Vector3(1, 0, Gradient).normalized;
            Vector3 lineCenter = new Vector3(PointOnLine_1.x, 0, PointOnLine_1.y) + Vector3.up;
            Gizmos.DrawLine(lineCenter - lineDirection * length / 2, lineCenter + lineDirection * length / 2);
        }
    }
}