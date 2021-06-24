using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bezier : MonoBehaviour
{
    [HideInInspector]
    List<Vector3> points;
    
    [HideInInspector]
    List<Vector3> curvePoints;
    
    [HideInInspector]
    public int segments;
    
    public bool GizmosOn=true;

     void Awake()
     {
        points = new List<Vector3>();
        curvePoints = new List<Vector3>();
        segments = 10;
     }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            if (GizmosOn)
            {
                
                for (int i = 0; i < curvePoints.Count; i++)
                {
                    Gizmos.color = Color.grey;
                    Gizmos.DrawSphere(curvePoints[i], 0.1f);
                }
                for (int i = 0; i < points.Count; i++)
                {
                    Gizmos.color = new Color(1,0.4f,0);
                   Gizmos.DrawSphere(points[i], 0.1f);
                }
            }
        }
    }
    
    public void Intialize(Vector3 P1, Vector3 P2, Vector3 P3,Vector3 P4)
    {
        Reset();

        points.Add(P1);
        points.Add(P2);
        points.Add(P3);
        points.Add(P4); 

        GenerateCurveForNewPoints();
    }

    public void Reset()
    {
        points.Clear();
        curvePoints.Clear();
    }

    public void AddPoint(Vector3 newPoint)
    {
        points.Add(points[points.Count - 1] * 2 - points[points.Count - 2]);
        points.Add((points[points.Count - 1] + newPoint) * .5f);
        points.Add(newPoint);
        GenerateCurveForNewPoints();
    }

    public void RegenerateCurvePoints()
    {
        curvePoints.Clear();
        
        float interval = 1.0f / segments;
        
        for (int i = 0; i < points.Count-3; i+=3)
        {
            for (float percent = 0; percent < 1; percent += interval)
            {
                curvePoints.Add(GetCubicPoint(points[i], points[i+1], points[i+2], points[i+3], percent));
            }
        }
    }

    void GenerateCurveForNewPoints()
    {
        float interval = 1.0f / segments;

        for (float percent = 0; percent < 1; percent += interval)
        {
            curvePoints.Add(GetCubicPoint(points[points.Count - 4], points[points.Count - 3], points[points.Count - 2], points[points.Count - 1], percent));
        }
    }

    Vector3 GetQuadraticPoint(Vector3 P1, Vector3 P2, Vector3 P3, float percent)
    {
        Vector3 point = GetLinearPoint(GetLinearPoint(P1, P2, percent), GetLinearPoint(P2, P3, percent), percent);

        return point;
    } 
    
    Vector3 GetCubicPoint(Vector3 P1, Vector3 P2, Vector3 P3,Vector2 P4, float percent)
    {
        Vector3 curve_1 = GetLinearPoint(GetLinearPoint(P1, P2, percent), GetLinearPoint(P2, P3, percent), percent);
        Vector3 curve_2 = GetLinearPoint(GetLinearPoint(P2, P3, percent), GetLinearPoint(P3, P4, percent), percent);

        return GetLinearPoint(curve_1,curve_2,percent);
    }

    Vector3 GetLinearPoint(Vector3 P1, Vector3 P2, float percent)
    {
        Vector3 point = new Vector3(0, 0, 0);

        point = P1 + (P2 - P1) * percent;
       
        return point;
    }

    public Vector3[] GetCurvePoints()
    {
        return curvePoints.ToArray();
    }

    public Vector3[] GetPoints()
    {
        return points.ToArray();
    }

    public void RemovePoint(int index)
    {
        points.RemoveAt(index);
    }

    public void RemovePoints(int index,int count)
    {
        points.RemoveRange(index, count);
    }

    public void RemoveCurvePoints(int index, int count)
    {
        curvePoints.RemoveRange(index, count);
    }
}
