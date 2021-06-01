using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bezier : MonoBehaviour
{
    public List<Vector3> controlPoints;
    public List<Vector3> points;

    public int segments;
    
    public bool GizmosOn=true;

     void Awake()
     {
        controlPoints = new List<Vector3>();
        points = new List<Vector3>();
     }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            if (GizmosOn)
            {
                
                for (int i = 0; i < points.Count; i++)
                {
                    Gizmos.color = Color.grey;
                    Gizmos.DrawSphere(points[i], 0.1f);
                }
                for (int i = 0; i < controlPoints.Count; i++)
                {
                    Gizmos.color = new Color(1,0.4f,0);
                    Gizmos.DrawSphere(controlPoints[i], 0.1f);
                }
            }
        }
    }

    public Vector3[] GenerateBezeirCurve(Vector3 P1, Vector3 P2, Vector3 P3)
    {
        points.Clear();
        controlPoints.Clear();

        int ind = 0;

        float interval = 1.0f / (segments);

        for (float percent = 0; percent < 1; percent += interval, ind++)
        {
            points.Add( GetQuadraticPoint(P1, P2, P3, percent));
        }

        controlPoints.Add(P1);
        controlPoints.Add(P2);
        controlPoints.Add(P3);

        return points.ToArray();
    }

    public Vector3[] GenerateBezeirCurveFromLast(Vector3 P2, Vector3 P3)
    {
        int ind = 0;

        float interval = 1.0f / segments;
        for (float percent = 0; percent < 1; percent += interval, ind++)
        {
            points.Add(GetQuadraticPoint(controlPoints[controlPoints.Count-1], P2, P3, percent));
        }
        
        controlPoints.Add(P2);
        controlPoints.Add(P3);

        return points.ToArray();
    }

    Vector3 GetQuadraticPoint(Vector3 P1, Vector3 P2, Vector3 P3, float percent)
    {
        Vector3 point = GetLinearPoint(GetLinearPoint(P1, P2, percent), GetLinearPoint(P2, P3, percent), percent);

        return point;
    }

    Vector3 GetLinearPoint(Vector3 P1, Vector3 P2, float percent)
    {
        Vector3 point = new Vector3(0, 0, 0);

        point = P1 + (P2 - P1) * percent;
       
        return point;
    }
}
